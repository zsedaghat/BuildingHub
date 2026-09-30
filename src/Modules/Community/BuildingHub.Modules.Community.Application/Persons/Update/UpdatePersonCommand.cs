namespace BuildingHub.Modules.Community.Application.Persons.Update;

public sealed record UpdatePersonCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    string? NationalId);