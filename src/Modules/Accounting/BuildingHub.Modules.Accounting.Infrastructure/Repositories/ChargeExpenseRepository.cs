
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using BuildingHub.Modules.Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Accounting.Infrastructure.Repositories;

public sealed class ChargeExpenseRepository
    : Repository<ChargeExpense>, IChargeExpenseRepository
{
    public ChargeExpenseRepository(AccountingDbContext context)
        : base(context)
    {
    }

    public async Task<decimal> GetTotalAllocatedAmountByExpenseIdAsync(
        Guid expenseId,
        CancellationToken cancellationToken = default)
    {
        return await Context.ChargeExpenses
            .Where(x => x.ExpenseId == expenseId)
            .SumAsync(
                x => (decimal?)x.AllocatedAmount,
                cancellationToken) ?? 0m;
    }

    public async Task<bool> ExistsAsync(
        Guid expenseId,
        Guid chargeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.ChargeExpenses
            .AnyAsync(
                x => x.ExpenseId == expenseId &&
                     x.ChargeId == chargeId,
                cancellationToken);
    }

    public async Task AddAllocationSafelyAsync(
        ChargeExpense allocation,
        CancellationToken cancellationToken = default)
    {
        if (allocation.AllocatedAmount <= 0)
        {
            throw new ArgumentException(
                "Allocated amount must be greater than zero.");
        }

        await using var transaction =
            await Context.Database.BeginTransactionAsync(
                cancellationToken);

        var expense = await Context.Expenses
            .FromSqlInterpolated(
                $"SELECT * FROM \"Expenses\" WHERE \"Id\" = {allocation.ExpenseId} FOR UPDATE")
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (expense is null)
        {
            throw new InvalidOperationException(
                "Expense not found.");
        }

        var totalAllocated =
            await GetTotalAllocatedAmountByExpenseIdAsync(
                allocation.ExpenseId,
                cancellationToken);

        if (totalAllocated + allocation.AllocatedAmount >
            expense.Amount)
        {
            throw new InvalidOperationException(
                "Total allocated amount cannot exceed the expense amount.");
        }

        await Context.ChargeExpenses.AddAsync(
            allocation,
            cancellationToken);

        await Context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<ChargeExpense>> GetByExpenseIdAsync(
        Guid expenseId,
        CancellationToken cancellationToken = default)
    {
        return await Context.ChargeExpenses
            .AsNoTracking()
            .Where(x => x.ExpenseId == expenseId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }


    public async Task UpdateAllocationSafelyAsync(
        Guid expenseId,
        Guid allocationId,
        decimal newAmount,
        CancellationToken cancellationToken = default)
    {
        if (newAmount <= 0)
        {
            throw new ArgumentException(
                "Allocated amount must be greater than zero.");
        }

        await using var transaction =
            await Context.Database.BeginTransactionAsync(
                cancellationToken);

        var expense = await Context.Expenses
            .FromSqlInterpolated(
                $"SELECT * FROM \"Expenses\" WHERE \"Id\" = {expenseId} FOR UPDATE")
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (expense is null)
        {
            throw new InvalidOperationException(
                "Expense not found.");
        }

        var allocation = await Context.ChargeExpenses
            .SingleOrDefaultAsync(
                x => x.Id == allocationId &&
                     x.ExpenseId == expenseId,
                cancellationToken);

        if (allocation is null)
        {
            throw new InvalidOperationException(
                "Allocation was not found for this expense.");
        }

        var totalAllocated =
            await GetTotalAllocatedAmountByExpenseIdAsync(
                expenseId,
                cancellationToken);

        var allocatedWithoutCurrent =
            totalAllocated - allocation.AllocatedAmount;

        if (allocatedWithoutCurrent + newAmount > expense.Amount)
        {
            throw new InvalidOperationException(
                "Total allocated amount cannot exceed the expense amount.");
        }

        allocation.UpdateAllocatedAmount(newAmount);

        await Context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }


}
