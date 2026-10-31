
using BuildingHub.Modules.Accounting.Infrastructure.Persistence;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/dev/test-data")]
public sealed class DevTestDataController : ControllerBase
{
    private readonly CommunityDbContext _communityDb;
    private readonly AccountingDbContext _accountingDb;
    private readonly IWebHostEnvironment _environment;

    public DevTestDataController(
        CommunityDbContext communityDb,
        AccountingDbContext accountingDb,
        IWebHostEnvironment environment)
    {
        _communityDb = communityDb;
        _accountingDb = accountingDb;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
            return NotFound();

        var buildings = await _communityDb.Buildings
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                buildingId = x.Id,
                buildingName = x.Name
            })
            .ToListAsync(cancellationToken);

        var units = await _communityDb.Units
            .AsNoTracking()
            .OrderBy(x => x.UnitNumber)
            .Select(x => new
            {
                unitId = x.Id,
                buildingId = x.BuildingId,
                unitNumber = x.UnitNumber,
                area = x.Area,
                isActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        var charges = await _accountingDb.Charges
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                chargeId = x.Id,
                buildingId = x.BuildingId,
                title = x.Title,
                totalAmount = x.TotalAmount,
                calculationMethod = x.CalculationMethod.ToString(),
                category = x.Category.ToString(),
                periodYear = x.PeriodYear,
                periodMonth = x.PeriodMonth
            })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            buildings,
            units,
            charges
        });
    }
}
