using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BuildingHub.Web.Models.Pagination;
using BuildingHub.Web.Models.Units;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuildingHub.Web.Controllers;

public sealed class UnitsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public UnitsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    private HttpClient CreateApiClient()
    {
        var baseUrl = _configuration["ApiSettings:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl) || !Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
            throw new InvalidOperationException("ApiSettings:BaseUrl در appsettings.json معتبر نیست.");

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(uri.ToString().TrimEnd('/') + "/");
        return client;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = pageSize is 10 or 20 or 50 or 100 ? pageSize : 10;

        try
        {
            var client = CreateApiClient();
            var unitsTask = client.GetFromJsonAsync<PagedApiResponse<UnitApiModel>>(
                $"api/units?pageNumber={pageNumber}&pageSize={pageSize}");
            var buildingsTask = GetBuildingsAsync(client);
            await Task.WhenAll(unitsTask!, buildingsTask);

            var result = await unitsTask ?? new PagedApiResponse<UnitApiModel>();
            var buildings = await buildingsTask;
            var buildingNames = buildings.ToDictionary(x => x.Id, x => x.Name);

            var floorsById = new Dictionary<Guid, string>();
            foreach (var building in buildings)
            {
                var floors = await GetFloorsByBuildingAsync(client, building.Id);
                foreach (var floor in floors)
                    floorsById[floor.Id] = floor.Name;
            }

            var items = result.Items.Select(unit => new UnitListItemViewModel
            {
                Id = unit.Id,
                BuildingId = unit.BuildingId,
                FloorId = unit.FloorId,
                UnitNumber = unit.UnitNumber,
                Type = unit.Type,
                TypeName = GetUnitTypeName(unit.Type),
                Area = unit.Area,
                IsActive = unit.IsActive,
                BuildingName = buildingNames.TryGetValue(unit.BuildingId, out var buildingName)
                    ? buildingName : "ساختمان نامشخص",
                FloorName = unit.FloorId.HasValue && floorsById.TryGetValue(unit.FloorId.Value, out var floorName)
                    ? floorName : unit.FloorId.HasValue ? "طبقه نامشخص" : "بدون طبقه"
            }).ToList();

            return View(new PagedListViewModel<UnitListItemViewModel>
            {
                Items = items,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            });
        }
        catch (Exception ex)
        {
            ViewBag.Error = GetFriendlyError(ex);
            return View(new PagedListViewModel<UnitListItemViewModel>
            { PageNumber = pageNumber, PageSize = pageSize });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new UnitFormViewModel();
        await LoadFormOptionsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UnitFormViewModel model)
    {
        if (model.BuildingId == Guid.Empty)
            ModelState.AddModelError(nameof(model.BuildingId), "انتخاب ساختمان الزامی است.");

        if (!ModelState.IsValid)
        {
            await LoadFormOptionsAsync(model);
            return View(model);
        }

        try
        {
            var request = new CreateUnitRequest(
                model.BuildingId,
                model.FloorId,
                model.UnitNumber.Trim(),
                model.Type,
                model.Area);

            var response = await CreateApiClient().PostAsJsonAsync("api/units", request);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "واحد با موفقیت ثبت شد.";
                return RedirectToAction(nameof(Index));
            }

            var apiError = await ReadApiErrorAsync(response, "ثبت واحد انجام نشد.");
            if (apiError.Contains("شماره واحد قبلاً", StringComparison.Ordinal))
                ModelState.AddModelError(nameof(model.UnitNumber), apiError);
            else
                ModelState.AddModelError(string.Empty, apiError);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, GetFriendlyError(ex));
        }

        await LoadFormOptionsAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var client = CreateApiClient();
            var unit = await client.GetFromJsonAsync<UnitApiModel>($"api/units/{id}");
            if (unit is null) return NotFound();

            var buildings = await GetBuildingsAsync(client);
            var buildingName = buildings.FirstOrDefault(x => x.Id == unit.BuildingId)?.Name ?? "ساختمان نامشخص";
            var floors = await GetFloorsByBuildingAsync(client, unit.BuildingId);
            var model = new UnitFormViewModel
            {
                Id = unit.Id,
                BuildingId = unit.BuildingId,
                FloorId = unit.FloorId,
                UnitNumber = unit.UnitNumber,
                Type = unit.Type,
                Area = unit.Area,
                BuildingName = buildingName,
                FloorName = unit.FloorId.HasValue
                    ? floors.FirstOrDefault(x => x.Id == unit.FloorId.Value)?.Name ?? "طبقه نامشخص"
                    : "بدون طبقه"
            };
            PopulateSelectLists(model, buildings, floors);
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UnitFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await LoadFormOptionsAsync(model);
            return View(model);
        }

        try
        {
            var request = new UpdateUnitRequest(id, model.UnitNumber.Trim(), model.Type, model.Area);
            var response = await CreateApiClient().PutAsJsonAsync($"api/units/{id}", request);

            if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "تغییرات واحد ذخیره شد.";
                return RedirectToAction(nameof(Index));
            }

            var apiError = await ReadApiErrorAsync(response, "ویرایش واحد انجام نشد.");
            if (apiError.Contains("شماره واحد قبلاً", StringComparison.Ordinal))
                ModelState.AddModelError(nameof(model.UnitNumber), apiError);
            else
                ModelState.AddModelError(string.Empty, apiError);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, GetFriendlyError(ex));
        }

        await LoadFormOptionsAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var client = CreateApiClient();
            var unit = await client.GetFromJsonAsync<UnitApiModel>($"api/units/{id}");
            if (unit is null) return NotFound();

            var buildings = await GetBuildingsAsync(client);
            var buildingName = buildings.FirstOrDefault(x => x.Id == unit.BuildingId)?.Name ?? "ساختمان نامشخص";
            var floors = await GetFloorsByBuildingAsync(client, unit.BuildingId);
            var floorName = unit.FloorId.HasValue
                ? floors.FirstOrDefault(x => x.Id == unit.FloorId.Value)?.Name ?? "طبقه نامشخص"
                : "بدون طبقه";

            return View(new UnitListItemViewModel
            {
                Id = unit.Id,
                BuildingId = unit.BuildingId,
                FloorId = unit.FloorId,
                UnitNumber = unit.UnitNumber,
                Type = unit.Type,
                TypeName = GetUnitTypeName(unit.Type),
                Area = unit.Area,
                IsActive = unit.IsActive,
                BuildingName = buildingName,
                FloorName = floorName
            });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var response = await CreateApiClient().DeleteAsync($"api/units/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "واحد حذف شد.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = await ReadApiErrorAsync(response, "حذف واحد انجام نشد.");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetFloors(Guid buildingId)
    {
        if (buildingId == Guid.Empty) return Json(Array.Empty<object>());
        try
        {
            var floors = await GetFloorsByBuildingAsync(CreateApiClient(), buildingId);
            return Json(floors.Select(x => new { id = x.Id, name = x.Name, floorNumber = x.FloorNumber }));
        }
        catch
        {
            return StatusCode(502, new { message = "دریافت فهرست طبقات امکان‌پذیر نیست." });
        }
    }

    private async Task LoadFormOptionsAsync(UnitFormViewModel model)
    {
        try
        {
            var client = CreateApiClient();
            var buildings = await GetBuildingsAsync(client);
            var floors = model.BuildingId == Guid.Empty
                ? new List<FloorApiModel>()
                : await GetFloorsByBuildingAsync(client, model.BuildingId);
            PopulateSelectLists(model, buildings, floors);
            if (model.Id != Guid.Empty)
            {
                model.BuildingName = buildings.FirstOrDefault(x => x.Id == model.BuildingId)?.Name ?? "ساختمان نامشخص";
                model.FloorName = model.FloorId.HasValue
                    ? floors.FirstOrDefault(x => x.Id == model.FloorId.Value)?.Name ?? "طبقه نامشخص"
                    : "بدون طبقه";
            }

            if (buildings.Count == 0)
                ModelState.AddModelError(string.Empty, "هیچ ساختمانی ثبت نشده است. ابتدا یک ساختمان ایجاد کنید.");
        }
        catch (Exception ex)
        {
            model.Buildings = new();
            model.Floors = new();
            ModelState.AddModelError(string.Empty, GetFriendlyError(ex));
        }
    }

    private static void PopulateSelectLists(
        UnitFormViewModel model,
        List<BuildingApiModel> buildings,
        List<FloorApiModel> floors)
    {
        model.Buildings = buildings.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == model.BuildingId
        }).ToList();

        model.Floors = floors.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Name} (شماره {x.FloorNumber})",
            Selected = x.Id == model.FloorId
        }).ToList();

        model.UnitTypes = new List<SelectListItem>
        {
            new("آپارتمان", "1", model.Type == 1),
            new("پارکینگ", "2", model.Type == 2),
            new("انباری", "3", model.Type == 3),
            new("تجاری", "4", model.Type == 4),
            new("سایر", "5", model.Type == 5)
        };
    }

    private static async Task<List<BuildingApiModel>> GetBuildingsAsync(HttpClient client)
    {
        return await client.GetFromJsonAsync<List<BuildingApiModel>>("api/buildings")
            ?? new List<BuildingApiModel>();
    }


    private static async Task<List<FloorApiModel>> GetFloorsByBuildingAsync(
        HttpClient client,
        Guid buildingId)
    {
        var result = await client.GetFromJsonAsync<PagedApiResponse<FloorApiModel>>(
            $"api/floors/building/{buildingId}?pageNumber=1&pageSize=100");

        return result?.Items ?? new List<FloorApiModel>();
    }


    private static string GetUnitTypeName(int type) => type switch
    {
        1 => "آپارتمان",
        2 => "پارکینگ",
        3 => "انباری",
        4 => "تجاری",
        5 => "سایر",
        _ => "نامشخص"
    };

    private static async Task<string> ReadApiErrorAsync(HttpResponseMessage response, string fallback)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return $"{fallback} (کد خطا: {(int)response.StatusCode})";

        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("detail", out var detail) &&
                detail.ValueKind == JsonValueKind.String)
                return $"{fallback} {detail.GetString()}";

            if (document.RootElement.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String)
                return $"{fallback} {title.GetString()}";
        }
        catch (JsonException) { }

        return $"{fallback} جزئیات: {body}";
    }

    private static string GetFriendlyError(Exception exception) => exception switch
    {
        HttpRequestException => "ارتباط با سرور API برقرار نشد. مطمئن شوید BuildingHub.Api در حال اجراست.",
        InvalidOperationException => exception.Message,
        _ => "عملیات با خطا مواجه شد: " + exception.Message
    };

    private sealed record CreateUnitRequest(Guid BuildingId, Guid? FloorId, string UnitNumber, int Type, decimal? Area);
    private sealed record UpdateUnitRequest(Guid Id, string UnitNumber, int Type, decimal? Area);

    private sealed class PagedApiResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }

    private sealed class UnitApiModel
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public Guid? FloorId { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public int Type { get; set; }
        public decimal? Area { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class BuildingApiModel { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; }
    private sealed class FloorApiModel { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public int FloorNumber { get; set; } }
}
