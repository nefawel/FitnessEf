namespace FitnessEf;

public static class ClientCommands
{
    public static async Task<Client> AddClientAsync(
        FitnessContext db,
        string firstName,
        string lastName,
        DateOnly? birthDate = null,
        string? phone = null,
        string? email = null,
        CancellationToken cancellationToken = default)
    {
        firstName = RequireName(firstName, "Имя");
        lastName = RequireName(lastName, "Фамилия");

        var client = new Client
        {
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Phone = NormalizeOptional(phone),
            Email = NormalizeOptional(email),
            RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
            Status = "active"
        };

        db.Clients.Add(client);
        await db.SaveChangesAsync(cancellationToken);
        return client;
    }

    public static async Task<bool> UpdateClientAsync(
        FitnessContext db,
        int clientId,
        string? phone,
        string? status,
        CancellationToken cancellationToken = default)
    {
        if (clientId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(clientId), "ID должен быть положительным.");
        }

        if (phone is null && status is null)
        {
            throw new ArgumentException("Нужно указать телефон или статус для изменения.");
        }

        var client = await db.Clients.FindAsync([clientId], cancellationToken);
        if (client is null)
        {
            return false;
        }

        if (phone is not null)
        {
            client.Phone = NormalizeOptional(phone);
        }

        if (status is not null)
        {
            var normalizedStatus = status.Trim().ToLowerInvariant();
            if (normalizedStatus is not ("active" or "inactive"))
            {
                throw new ArgumentException("Статус должен быть active или inactive.", nameof(status));
            }

            client.Status = normalizedStatus;
        }

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public static async Task<bool> DeleteClientAsync(
        FitnessContext db,
        int clientId,
        CancellationToken cancellationToken = default)
    {
        if (clientId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(clientId), "ID должен быть положительным.");
        }

        var client = await db.Clients.FindAsync([clientId], cancellationToken);
        if (client is null)
        {
            return false;
        }

        db.Clients.Remove(client);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string RequireName(string? value, string fieldName)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} обязательно.", fieldName);
        }

        if (normalized.Length > 100)
        {
            throw new ArgumentException($"{fieldName} не может быть длиннее 100 символов.", fieldName);
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
