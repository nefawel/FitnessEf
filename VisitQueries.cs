using Microsoft.EntityFrameworkCore;

namespace FitnessEf;

public static class VisitQueries
{
    public static Task<List<Client>> ListClientsWithVisitsAsync(
        FitnessContext db,
        CancellationToken cancellationToken = default) =>
        db.Clients
            .AsNoTracking()
            .Include(client => client.Visits)
            .OrderBy(client => client.ClientId)
            .ToListAsync(cancellationToken);

    public static async Task<Visit?> AddVisitAsync(
        FitnessContext db,
        int clientId,
        DateOnly visitDate,
        string? note = null,
        CancellationToken cancellationToken = default)
    {
        if (clientId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(clientId), "ID должен быть положительным.");
        }

        if (!await db.Clients.AnyAsync(client => client.ClientId == clientId, cancellationToken))
        {
            return null;
        }

        var visit = new Visit
        {
            ClientId = clientId,
            VisitDate = visitDate,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
        };

        db.Visits.Add(visit);
        await db.SaveChangesAsync(cancellationToken);
        return visit;
    }
}
