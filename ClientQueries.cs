using Microsoft.EntityFrameworkCore;

namespace FitnessEf;

public sealed record ClientListItem(
    int ClientId,
    string FirstName,
    string LastName,
    string Status,
    string? Phone);

public sealed record ClientProjection(
    int ClientId,
    string FullName,
    string? Phone);

public sealed record ClientStatistics(int Total, int Active, bool HasInactive);

public static class ClientQueries
{
    public static Task<List<Client>> ListClientsAsync(
        FitnessContext db,
        CancellationToken cancellationToken = default) =>
        db.Clients
            .AsNoTracking()
            .OrderBy(client => client.ClientId)
            .ToListAsync(cancellationToken);

    public static Task<List<ClientProjection>> ProjectClientsAsync(
        FitnessContext db,
        CancellationToken cancellationToken = default) =>
        db.Clients
            .AsNoTracking()
            .OrderBy(client => client.LastName)
            .ThenBy(client => client.FirstName)
            .Select(client => new ClientProjection(
                client.ClientId,
                client.FirstName + " " + client.LastName,
                client.Phone))
            .ToListAsync(cancellationToken);

    public static Task<List<ClientListItem>> GetPageAsync(
        FitnessContext db,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Номер страницы начинается с 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Размер страницы должен быть положительным.");
        }

        return db.Clients
            .AsNoTracking()
            .OrderBy(client => client.ClientId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(client => new ClientListItem(
                client.ClientId,
                client.FirstName,
                client.LastName,
                client.Status,
                client.Phone))
            .ToListAsync(cancellationToken);
    }

    public static Task<List<ClientListItem>> GetActiveClientsAsync(
        FitnessContext db,
        CancellationToken cancellationToken = default) =>
        db.Clients
            .AsNoTracking()
            .Where(client => client.Status == "active")
            .OrderBy(client => client.LastName)
            .ThenBy(client => client.FirstName)
            .Select(client => new ClientListItem(
                client.ClientId,
                client.FirstName,
                client.LastName,
                client.Status,
                client.Phone))
            .ToListAsync(cancellationToken);

    public static Task<List<ClientListItem>> SearchByLastNameAsync(
        FitnessContext db,
        string term,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            throw new ArgumentException("Фрагмент фамилии не должен быть пустым.", nameof(term));
        }

        var normalizedTerm = term.Trim();
        return db.Clients
            .AsNoTracking()
            .Where(client => client.LastName.Contains(normalizedTerm))
            .OrderBy(client => client.LastName)
            .ThenBy(client => client.FirstName)
            .Select(client => new ClientListItem(
                client.ClientId,
                client.FirstName,
                client.LastName,
                client.Status,
                client.Phone))
            .ToListAsync(cancellationToken);
    }

    public static async Task<Client?> FindByIdAsync(
        FitnessContext db,
        int clientId,
        CancellationToken cancellationToken = default)
    {
        if (clientId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(clientId), "ID должен быть положительным.");
        }

        return await db.Clients.FindAsync([clientId], cancellationToken);
    }

    public static async Task<ClientStatistics> GetStatisticsAsync(
        FitnessContext db,
        CancellationToken cancellationToken = default)
    {
        var total = await db.Clients.CountAsync(cancellationToken);
        var active = await db.Clients.CountAsync(client => client.Status == "active", cancellationToken);
        var hasInactive = await db.Clients.AnyAsync(client => client.Status == "inactive", cancellationToken);
        return new ClientStatistics(total, active, hasInactive);
    }
}
