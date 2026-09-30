namespace BuildingHub.Modules.Community.Application.Persons.Create;

public sealed record CreatePersonCommand(
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    string? NationalId);