
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using BuildingHub.Modules.Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Accounting.Infrastructure.Repositories;

public sealed class ExpenseRepository
    : Repository<Expense>, IExpenseRepository
{
    public ExpenseRepository(AccountingDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Expense>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Expenses
            .AsNoTracking()
            .Where(x => x.BuildingId == buildingId)
            .OrderByDescending(x => x.ExpenseDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
