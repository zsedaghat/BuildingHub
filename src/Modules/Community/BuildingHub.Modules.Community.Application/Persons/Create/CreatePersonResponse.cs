namespace BuildingHub.Modules.Community.Application.Persons.Create;

public sealed record CreatePersonResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    string? NationalId);