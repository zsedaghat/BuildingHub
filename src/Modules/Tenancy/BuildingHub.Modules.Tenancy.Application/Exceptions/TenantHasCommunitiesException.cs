namespace BuildingHub.Modules.Tenancy.Application.Exceptions;

public sealed class TenantHasCommunitiesException : Exception
{
    public TenantHasCommunitiesException()
        : base("Cannot delete a tenant that has communities.")
    {
    }
}