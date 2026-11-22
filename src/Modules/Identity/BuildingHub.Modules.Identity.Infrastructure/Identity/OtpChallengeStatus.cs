
namespace BuildingHub.Modules.Identity.Infrastructure.Identity;

public enum OtpChallengeStatus
{
    Active = 1,
    Consumed = 2,
    Superseded = 3,
    Expired = 4,
    Locked = 5
}
