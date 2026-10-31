
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.Create;

public sealed class CreateExpenseCommandHandler
    : IRequestHandler<CreateExpenseCommand, Guid>
{
    private readonly IExpenseRepository _expenseRepository;

    public CreateExpenseCommandHandler(
        IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<Guid> Handle(
        CreateExpenseCommand request,
        CancellationToken cancellationToken)
    {
        var expense = Expense.Create(
            request.BuildingId,
            request.Title,
            request.Amount,
            request.ExpenseDate,
            request.Description);

        await _expenseRepository.AddAsync(
            expense,
            cancellationToken);

        return expense.Id;
    }
}
