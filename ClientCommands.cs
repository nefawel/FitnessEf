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
