using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Community.Domain.Entities;

public class Person : Entity
{
    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string Mobile { get; private set; } = null!;

    public string? Email { get; private set; }

    public string? NationalId { get; private set; }

    private Person()
    {
    }

    private Person(
        string firstName,
        string lastName,
        string mobile,
        string? email,
        string? nationalId)
    {
        FirstName = firstName;
        LastName = lastName;
        Mobile = mobile;
        Email = email;
        NationalId = nationalId;
    }

    public static Person Create(
        string firstName,
        string lastName,
        string mobile,
        string? email = null,
        string? nationalId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        if (string.IsNullOrWhiteSpace(mobile))
            throw new ArgumentException("Mobile is required.");

        return new Person(
            firstName,
            lastName,
            mobile,
            email,
            nationalId);
    }

    public void Update(
        string firstName,
        string lastName,
        string mobile,
        string? email,
        string? nationalId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        if (string.IsNullOrWhiteSpace(mobile))
            throw new ArgumentException("Mobile is required.");

        FirstName = firstName;
        LastName = lastName;
        Mobile = mobile;
        Email = email;
        NationalId = nationalId;
    }
}