namespace BuildingHub.Modules.Community.Application.Persons.Update;

public sealed record UpdatePersonResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    string? NationalId);