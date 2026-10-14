using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Accounting.Domain.Entities;

public enum ChargeCalculationMethod
{
    Equal = 1,
    ByArea = 2,
    ByPerson = 3,
    ByAreaAndPerson = 4,
    Custom = 5
}

public class Charge : Entity
{
    public Guid BuildingId { get; private set; }

    public string Title { get; private set; } = null!;

    public decimal TotalAmount { get; private set; }

    public ChargeCalculationMethod CalculationMethod { get; private set; }

    public int PeriodYear { get; private set; }

    public int PeriodMonth { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Charge()
    {
    }

    private Charge(
        Guid buildingId,
        string title,
        decimal totalAmount,
        ChargeCalculationMethod calculationMethod,
        int periodYear,
        int periodMonth)
    {
        BuildingId = buildingId;
        Title = title;
        TotalAmount = totalAmount;
        CalculationMethod = calculationMethod;
        PeriodYear = periodYear;
        PeriodMonth = periodMonth;
        CreatedAt = DateTime.UtcNow;
    }

    public static Charge Create(
        Guid buildingId,
        string title,
        decimal totalAmount,
        ChargeCalculationMethod calculationMethod,
        int periodYear,
        int periodMonth)
    {
        if (buildingId == Guid.Empty)
            throw new ArgumentException("BuildingId is required.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Charge title is required.");

        if (totalAmount <= 0)
            throw new ArgumentException(
                "Total amount must be greater than zero.");

        if (periodYear < 2000)
            throw new ArgumentException("Invalid period year.");

        if (periodMonth < 1 || periodMonth > 12)
            throw new ArgumentException("Invalid period month.");

        return new Charge(
            buildingId,
            title,
            totalAmount,
            calculationMethod,
            periodYear,
            periodMonth);
    }

    public void Update(
        string title,
        decimal totalAmount,
        ChargeCalculationMethod calculationMethod,
        int periodYear,
        int periodMonth)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Charge title is required.");

        if (totalAmount <= 0)
            throw new ArgumentException(
                "Total amount must be greater than zero.");

        if (periodYear < 2000)
            throw new ArgumentException("Invalid period year.");

        if (periodMonth < 1 || periodMonth > 12)
            throw new ArgumentException("Invalid period month.");

        Title = title;
        TotalAmount = totalAmount;
        CalculationMethod = calculationMethod;
        PeriodYear = periodYear;
        PeriodMonth = periodMonth;
    }
}