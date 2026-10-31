
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Accounting.Domain.Entities;

public class Expense : Entity
{
    public Guid BuildingId { get; private set; }

    public string Title { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public DateOnly ExpenseDate { get; private set; }

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Expense()
    {
    }

    private Expense(
        Guid buildingId,
        string title,
        decimal amount,
        DateOnly expenseDate,
        string? description)
    {
        BuildingId = buildingId;
        Title = title;
        Amount = amount;
        ExpenseDate = expenseDate;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public static Expense Create(
        Guid buildingId,
        string title,
        decimal amount,
        DateOnly expenseDate,
        string? description = null)
    {
        if (buildingId == Guid.Empty)
            throw new ArgumentException("BuildingId is required.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Expense title is required.");

        if (amount <= 0)
            throw new ArgumentException(
                "Expense amount must be greater than zero.");

        return new Expense(
            buildingId,
            title.Trim(),
            amount,
            expenseDate,
            string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim());
    }
}
