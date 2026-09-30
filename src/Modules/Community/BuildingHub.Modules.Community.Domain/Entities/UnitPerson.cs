using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Entities;

public enum UnitPersonRole
{
    Owner = 1,
    Resident = 2,
    Tenant = 3,
    Other = 4
}

public class UnitPerson : Entity
{
    public Guid UnitId { get; private set; }
    public Guid PersonId { get; private set; }

    public UnitPersonRole Role { get; private set; }

    public bool IsPrimary { get; private set; }

    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    private UnitPerson()
    {
    }

    private UnitPerson(
        Guid unitId,
        Guid personId,
        UnitPersonRole role,
        bool isPrimary,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        UnitId = unitId;
        PersonId = personId;
        Role = role;
        IsPrimary = isPrimary;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static UnitPerson Create(
        Guid unitId,
        Guid personId,
        UnitPersonRole role,
        bool isPrimary = false,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        if (unitId == Guid.Empty)
            throw new ArgumentException("UnitId is required.");

        if (personId == Guid.Empty)
            throw new ArgumentException("PersonId is required.");

        if (endDate.HasValue &&
            startDate.HasValue &&
            endDate < startDate)
        {
            throw new ArgumentException(
                "EndDate cannot be before StartDate.");
        }

        return new UnitPerson(
            unitId,
            personId,
            role,
            isPrimary,
            startDate,
            endDate);
    }

    public void Update(
        UnitPersonRole role,
        bool isPrimary,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        if (endDate.HasValue &&
            startDate.HasValue &&
            endDate < startDate)
        {
            throw new ArgumentException(
                "EndDate cannot be before StartDate.");
        }

        Role = role;
        IsPrimary = isPrimary;
        StartDate = startDate;
        EndDate = endDate;
    }
}