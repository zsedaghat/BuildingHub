using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BuildingHub.Web.Models.Floors;
using BuildingHub.Web.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuildingHub.Web.Controllers;

public class FloorsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public FloorsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
            var floorsTask = client.GetFromJsonAsync<PagedApiResponse<FloorApiModel>>(
                $"api/floors?pageNumber={pageNumber}&pageSize={pageSize}");
            var buildingsTask = GetBuildingsAsync(client);
            await Task.WhenAll(floorsTask!, buildingsTask);
            var result = await floorsTask ?? new PagedApiResponse<FloorApiModel>();
            var buildings = await buildingsTask;
            var names = buildings.ToDictionary(x => x.Id, x => x.Name);
            var items = result.Items.Select(x => new FloorListItemViewModel
            {
                Id = x.Id, BuildingId = x.BuildingId, Name = x.Name,
                FloorNumber = x.FloorNumber, DisplayOrder = x.DisplayOrder,
                BuildingName = names.TryGetValue(x.BuildingId, out var name) ? name : "ساختمان نامشخص"
            }).ToList();
            return View(new PagedListViewModel<FloorListItemViewModel>
            { Items = items, PageNumber = result.PageNumber, PageSize = result.PageSize, TotalCount = result.TotalCount });
        }
        catch (Exception ex)
        {
            ViewBag.Error = GetFriendlyError(ex);
            return View(new PagedListViewModel<FloorListItemViewModel> { PageNumber = pageNumber, PageSize = pageSize });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new FloorFormViewModel();
        await LoadBuildingsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FloorFormViewModel model)
    {
        if (model.BuildingId == Guid.Empty)
            ModelState.AddModelError(nameof(model.BuildingId), "انتخاب ساختمان الزامی است.");
        if (!ModelState.IsValid)
        {
            await LoadBuildingsAsync(model);
            return View(model);
        }
        try
        {
            var response = await CreateApiClient().PostAsJsonAsync("api/floors",
                new CreateFloorRequest(model.BuildingId, model.Name.Trim(), model.FloorNumber, model.DisplayOrder));
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "طبقه با موفقیت ثبت شد.";
                return RedirectToAction(nameof(Index));
            }
            await AddApiErrorToModelStateAsync(response, "ثبت طبقه انجام نشد.", nameof(model.FloorNumber));
        }
        catch (Exception ex) { ModelState.AddModelError(string.Empty, GetFriendlyError(ex)); }
        await LoadBuildingsAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var client = CreateApiClient();
            var floor = await client.GetFromJsonAsync<FloorApiModel>($"api/floors/{id}");
            if (floor is null) return NotFound();
            var model = new FloorFormViewModel
            {
                Id = floor.Id, BuildingId = floor.BuildingId, Name = floor.Name,
                FloorNumber = floor.FloorNumber, DisplayOrder = floor.DisplayOrder
            };
            await LoadBuildingsAsync(model);
            return View(model);
        }
        catch (Exception ex) { TempData["ErrorMessage"] = GetFriendlyError(ex); return RedirectToAction(nameof(Index)); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FloorFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) { await LoadBuildingsAsync(model); return View(model); }
        try
        {
            var response = await CreateApiClient().PutAsJsonAsync($"api/floors/{id}",
                new UpdateFloorRequest(id, model.Name.Trim(), model.FloorNumber, model.DisplayOrder));
            if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "تغییرات طبقه ذخیره شد.";
                return RedirectToAction(nameof(Index));
            }
            await AddApiErrorToModelStateAsync(response, "ویرایش طبقه انجام نشد.", nameof(model.FloorNumber));
        }
        catch (Exception ex) { ModelState.AddModelError(string.Empty, GetFriendlyError(ex)); }
        await LoadBuildingsAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var client = CreateApiClient();
            var floor = await client.GetFromJsonAsync<FloorApiModel>($"api/floors/{id}");
            if (floor is null) return NotFound();
            var buildings = await GetBuildingsAsync(client);
            return View(new FloorListItemViewModel
            {
                Id = floor.Id, BuildingId = floor.BuildingId, Name = floor.Name,
                FloorNumber = floor.FloorNumber, DisplayOrder = floor.DisplayOrder,
                BuildingName = buildings.FirstOrDefault(x => x.Id == floor.BuildingId)?.Name ?? "ساختمان نامشخص"
            });
        }
        catch (Exception ex) { TempData["ErrorMessage"] = GetFriendlyError(ex); return RedirectToAction(nameof(Index)); }
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var response = await CreateApiClient().DeleteAsync($"api/floors/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "طبقه حذف شد.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = await ReadApiErrorAsync(response, "حذف طبقه انجام نشد.");
        }
        catch (Exception ex) { TempData["ErrorMessage"] = GetFriendlyError(ex); }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadBuildingsAsync(FloorFormViewModel model)
    {
        try
        {
            var buildings = await GetBuildingsAsync(CreateApiClient());
            model.Buildings = buildings.Select(x => new SelectListItem
            { Value = x.Id.ToString(), Text = x.Name, Selected = x.Id == model.BuildingId }).ToList();
            if (model.Buildings.Count == 0)
                ModelState.AddModelError(string.Empty, "هیچ ساختمانی ثبت نشده است. ابتدا یک ساختمان ایجاد کنید.");
        }
        catch (Exception ex) { model.Buildings = new(); ModelState.AddModelError(string.Empty, GetFriendlyError(ex)); }
    }

    private static async Task<List<BuildingApiModel>> GetBuildingsAsync(HttpClient client)
    {
        var response = await client.GetFromJsonAsync<PagedApiResponse<BuildingApiModel>>("api/buildings?pageNumber=1&pageSize=100");
        return response?.Items ?? new List<BuildingApiModel>();
    }

    private async Task AddApiErrorToModelStateAsync(
        HttpResponseMessage response,
        string fallback,
        string floorNumberField)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
        {
            ModelState.AddModelError(string.Empty, $"{fallback} (کد خطا: {(int)response.StatusCode})");
            return;
        }

        string? detail = null;
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("detail", out var detailElement))
            {
                detail = detailElement.GetString();
            }
        }
        catch (JsonException)
        {
            // پاسخ غیر JSON است؛ متن خام پایین‌تر به‌عنوان پیام عمومی نمایش داده می‌شود.
        }

        if (!string.IsNullOrWhiteSpace(detail))
        {
            var isDuplicateFloorNumber =
                detail.Contains("شماره طبقه قبلاً", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("IX_Floors_BuildingId_FloorNumber", StringComparison.OrdinalIgnoreCase);

            if (isDuplicateFloorNumber)
            {
                ModelState.AddModelError(floorNumberField, detail);
                return;
            }

            ModelState.AddModelError(string.Empty, $"{fallback} {detail}");
            return;
        }

        ModelState.AddModelError(string.Empty, $"{fallback} جزئیات: {body}");
    }


    private static async Task<string> ReadApiErrorAsync(
        HttpResponseMessage response,
        string fallback)
    {
        var body = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(body))
            return $"{fallback} (کد خطا: {(int)response.StatusCode})";

        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;

            if (root.TryGetProperty("detail", out var detail) &&
                detail.ValueKind == JsonValueKind.String)
            {
                return $"{fallback} {detail.GetString()}";
            }

            if (root.TryGetProperty("errors", out var errors) &&
                errors.ValueKind == JsonValueKind.Object)
            {
                var messages = errors.EnumerateObject()
                    .SelectMany(x => x.Value.EnumerateArray())
                    .Where(x => x.ValueKind == JsonValueKind.String)
                    .Select(x => x.GetString())
                    .Where(x => !string.IsNullOrWhiteSpace(x));

                var message = string.Join(" ", messages);

                if (!string.IsNullOrWhiteSpace(message))
                    return $"{fallback} {message}";
            }

            if (root.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String)
            {
                return $"{fallback} {title.GetString()}";
            }
        }
        catch (JsonException)
        {
            // پاسخ API JSON معتبر نیست.
        }

        return $"{fallback} جزئیات: {body}";
    }

    private static string GetFriendlyError(Exception exception) => exception switch
    {
        HttpRequestException => "ارتباط با سرور API برقرار نشد. مطمئن شوید BuildingHub.Api در حال اجراست.",
        InvalidOperationException => exception.Message,
        _ => "عملیات با خطا مواجه شد: " + exception.Message
    };

    private sealed record CreateFloorRequest(Guid BuildingId, string Name, int FloorNumber, int DisplayOrder);
    private sealed record UpdateFloorRequest(Guid Id, string Name, int FloorNumber, int DisplayOrder);
    private sealed class PagedApiResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }
    private sealed class FloorApiModel
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public int DisplayOrder { get; set; }
    }
    private sealed class BuildingApiModel { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; }
}
