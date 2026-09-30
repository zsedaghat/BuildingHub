namespace BuildingHub.Modules.Community.Application.Persons.GetByMobile;

public sealed record GetPersonByMobileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    string? NationalId);