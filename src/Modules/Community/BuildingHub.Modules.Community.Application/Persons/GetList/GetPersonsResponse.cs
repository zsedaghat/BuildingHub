namespace BuildingHub.Modules.Community.Application.Persons.GetList;

public sealed record GetPersonsResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    string? NationalId);