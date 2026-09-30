namespace BuildingHub.Modules.Community.Application.Persons.GetById;

public sealed record GetPersonByIdResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    string? NationalId);