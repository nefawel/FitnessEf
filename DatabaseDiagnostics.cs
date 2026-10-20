using Microsoft.EntityFrameworkCore;

namespace FitnessEf;

public static class DatabaseDiagnostics
{
    public static async Task<string> CheckConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var db = new FitnessContext();
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                return "PostgreSQL недоступен. Проверьте сервер, порт и строку подключения.";
            }

            var total = await db.Clients.CountAsync(cancellationToken);
            return $"Соединение установлено. Клиентов в базе: {total}.";
        }
        catch (InvalidOperationException ex)
        {
            return $"Ошибка конфигурации: {ex.Message}";
        }
        catch (DbUpdateException ex)
        {
            return $"Ошибка базы данных: {ex.GetBaseException().Message}";
        }
        catch (Exception ex)
        {
            return $"Непредвиденная ошибка: {ex.Message}";
        }
    }

    public static string GetActiveClientsSql(FitnessContext db)
    {
        var query = db.Clients
            .Where(client => client.Status == "active")
            .OrderBy(client => client.LastName);

        return query.ToQueryString();
    }

    public static async Task<Client?> LoadTrackedClientAsync(
        FitnessContext db,
        int clientId,
        CancellationToken cancellationToken = default)
    {
        var client = await db.Clients
            .FirstOrDefaultAsync(item => item.ClientId == clientId, cancellationToken);

        return client;
    }

    public static Task<List<Client>> LoadReadOnlyClientsAsync(
        FitnessContext db,
        CancellationToken cancellationToken = default) =>
        db.Clients
            .AsNoTracking()
            .OrderBy(client => client.ClientId)
            .ToListAsync(cancellationToken);
}
