using FitnessEf;
using Microsoft.EntityFrameworkCore;

await new FitnessConsoleApp().RunAsync();

internal sealed class FitnessConsoleApp
{
    public async Task RunAsync()
    {
        Console.WriteLine("FitnessEf: управление клиентами фитнес-клуба");

        while (true)
        {
            PrintMenu();
            var choice = Console.ReadLine()?.Trim();

            if (choice == "0")
            {
                return;
            }

            try
            {
                switch (choice)
                {
                    case "1":
                        await ListClientsAsync();
                        break;
                    case "2":
                        await FindClientAsync();
                        break;
                    case "3":
                        await AddClientAsync();
                        break;
                    case "4":
                        await UpdateClientAsync();
                        break;
                    case "5":
                        await DeleteClientAsync();
                        break;
                    case "6":
                        await ShowStatisticsAsync();
                        break;
                    case "7":
                        await SearchClientsAsync();
                        break;
                    case "8":
                        await CheckDatabaseAsync();
                        break;
                    case "9":
                        await ShowSqlAsync();
                        break;
                    default:
                        Console.WriteLine("Неизвестный пункт меню.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка ввода: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка конфигурации или операции: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Не удалось сохранить изменения в базе данных.");
                Console.WriteLine($"Диагностика: {ex.GetBaseException().Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("1. Список клиентов");
        Console.WriteLine("2. Найти клиента по ID");
        Console.WriteLine("3. Добавить клиента");
        Console.WriteLine("4. Изменить телефон или статус");
        Console.WriteLine("5. Удалить клиента");
        Console.WriteLine("6. Статистика");
        Console.WriteLine("7. Поиск по фамилии");
        Console.WriteLine("8. Проверить подключение");
        Console.WriteLine("9. Показать SQL активных клиентов");
        Console.WriteLine("0. Выход");
        Console.Write("Выбор: ");
    }

    private static async Task ListClientsAsync()
    {
        await using var db = new FitnessContext();
        var clients = await ClientQueries.ProjectClientsAsync(db);

        if (clients.Count == 0)
        {
            Console.WriteLine("Клиенты отсутствуют.");
            return;
        }

        foreach (var client in clients)
        {
            Console.WriteLine(
                $"{client.ClientId}: {client.FullName}; телефон: {client.Phone ?? "не указан"}");
        }
    }

    private static async Task FindClientAsync()
    {
        var id = ReadPositiveId("ID клиента: ");
        await using var db = new FitnessContext();
        var client = await ClientQueries.FindByIdAsync(db, id);

        if (client is null)
        {
            Console.WriteLine("Клиент не найден.");
            return;
        }

        PrintClient(client);
        Console.WriteLine($"Посещений: {client.Visits.Count}");
    }

    private static async Task AddClientAsync()
    {
        Console.Write("Имя: ");
        var firstName = Console.ReadLine() ?? string.Empty;
        Console.Write("Фамилия: ");
        var lastName = Console.ReadLine() ?? string.Empty;
        Console.Write("Дата рождения (ГГГГ-ММ-ДД, можно пусто): ");
        var birthDate = ReadOptionalDate();
        Console.Write("Телефон (можно пусто): ");
        var phone = Console.ReadLine();
        Console.Write("Email (можно пусто): ");
        var email = Console.ReadLine();

        await using var db = new FitnessContext();
        var client = await ClientCommands.AddClientAsync(
            db,
            firstName,
            lastName,
            birthDate,
            phone,
            email);

        Console.WriteLine($"Клиент добавлен. ID: {client.ClientId}");
    }

    private static async Task UpdateClientAsync()
    {
        var id = ReadPositiveId("ID клиента: ");
        Console.Write("Новый телефон (пусто - оставить без изменений): ");
        var phone = Console.ReadLine();
        Console.Write("Новый статус active/inactive (пусто - оставить без изменений): ");
        var status = Console.ReadLine();

        phone = string.IsNullOrWhiteSpace(phone) ? null : phone;
        status = string.IsNullOrWhiteSpace(status) ? null : status;

        await using var db = new FitnessContext();
        var updated = await ClientCommands.UpdateClientAsync(db, id, phone, status);
        Console.WriteLine(updated ? "Клиент изменён." : "Клиент не найден.");
    }

    private static async Task DeleteClientAsync()
    {
        var id = ReadPositiveId("ID клиента: ");
        Console.Write("Подтвердить удаление (да/нет): ");
        if (!string.Equals(Console.ReadLine()?.Trim(), "да", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Удаление отменено.");
            return;
        }

        try
        {
            await using var db = new FitnessContext();
            var deleted = await ClientCommands.DeleteClientAsync(db, id);
            Console.WriteLine(deleted ? "Клиент удалён." : "Клиент не найден.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Клиента нельзя удалить: у него могут быть связанные посещения.");
            Console.WriteLine($"Диагностика: {ex.GetBaseException().Message}");
        }
    }

    private static async Task ShowStatisticsAsync()
    {
        await using var db = new FitnessContext();
        var statistics = await ClientQueries.GetStatisticsAsync(db);
        Console.WriteLine($"Всего клиентов: {statistics.Total}");
        Console.WriteLine($"Активных клиентов: {statistics.Active}");
        Console.WriteLine(statistics.HasInactive
            ? "Есть неактивные клиенты."
            : "Неактивные клиенты отсутствуют.");
    }

    private static async Task SearchClientsAsync()
    {
        Console.Write("Фрагмент фамилии: ");
        var term = Console.ReadLine() ?? string.Empty;
        await using var db = new FitnessContext();
        var clients = await ClientQueries.SearchByLastNameAsync(db, term);

        if (clients.Count == 0)
        {
            Console.WriteLine("Совпадений нет.");
            return;
        }

        foreach (var client in clients)
        {
            Console.WriteLine($"{client.ClientId}: {client.FirstName} {client.LastName}");
        }
    }

    private static async Task CheckDatabaseAsync()
    {
        Console.WriteLine(await DatabaseDiagnostics.CheckConnectionAsync());
    }

    private static async Task ShowSqlAsync()
    {
        await using var db = new FitnessContext();
        Console.WriteLine(DatabaseDiagnostics.GetActiveClientsSql(db));
        await Task.CompletedTask;
    }

    private static int ReadPositiveId(string prompt)
    {
        Console.Write(prompt);
        if (!int.TryParse(Console.ReadLine(), out var id) || id < 1)
        {
            throw new ArgumentException("ID должен быть положительным целым числом.");
        }

        return id;
    }

    private static DateOnly? ReadOptionalDate()
    {
        var value = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (!DateOnly.TryParse(value, out var date))
        {
            throw new ArgumentException("Дата должна быть в формате ГГГГ-ММ-ДД.");
        }

        return date;
    }

    private static void PrintClient(Client client)
    {
        Console.WriteLine($"{client.ClientId}: {client.FirstName} {client.LastName}");
        Console.WriteLine($"Дата рождения: {client.BirthDate?.ToString("yyyy-MM-dd") ?? "не указана"}");
        Console.WriteLine($"Телефон: {client.Phone ?? "не указан"}");
        Console.WriteLine($"Email: {client.Email ?? "не указан"}");
        Console.WriteLine($"Статус: {client.Status}");
    }
}
