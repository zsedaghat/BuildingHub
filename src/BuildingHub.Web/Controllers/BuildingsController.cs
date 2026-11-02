using System.Net;
using System.Net.Http.Json;
using BuildingHub.Web.Models.Buildings;
using BuildingHub.Web.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuildingHub.Web.Controllers;

public class BuildingsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BuildingsController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    private HttpClient CreateApiClient()
    {
        var baseUrl = _configuration["ApiSettings:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl) ||
            !Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException(
                "ApiSettings:BaseUrl در appsettings.json معتبر نیست.");
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(uri.ToString().TrimEnd('/') + "/");
        return client;
    }

    // READ: نمایش همه ساختمان‌ها
    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = pageSize is 10 or 20 or 50 or 100 ? pageSize : 10;
        try
        {
            var client = CreateApiClient();
            var buildingsTask = client.GetFromJsonAsync<PagedApiResponse<BuildingApiModel>>(
                $"api/buildings?pageNumber={pageNumber}&pageSize={pageSize}");
            var communitiesTask = client.GetFromJsonAsync<List<CommunityApiModel>>("api/communities");
            await Task.WhenAll(buildingsTask!, communitiesTask!);

            var result = await buildingsTask ?? new PagedApiResponse<BuildingApiModel>();
            var communities = await communitiesTask ?? new List<CommunityApiModel>();
            var communityNames = communities.ToDictionary(x => x.Id, x => x.Name);
            var items = result.Items.Select(building => new BuildingListItemViewModel
            {
                Id = building.Id, Name = building.Name, Description = building.Description,
                CommunityId = building.CommunityId,
                CommunityName = communityNames.TryGetValue(building.CommunityId, out var name) ? name : "جامعه نامشخص",
                DisplayOrder = building.DisplayOrder
            }).ToList();

            return View(new PagedListViewModel<BuildingListItemViewModel>
            { Items = items, PageNumber = result.PageNumber, PageSize = result.PageSize, TotalCount = result.TotalCount });
        }
        catch (Exception ex)
        {
            ViewBag.Error = GetFriendlyError(ex);
            return View(new PagedListViewModel<BuildingListItemViewModel> { PageNumber = pageNumber, PageSize = pageSize });
        }
    }

    // CREATE: نمایش فرم افزودن
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new BuildingFormViewModel();
        await LoadCommunitiesAsync(model);
        return View(model);
    }

    // CREATE: ثبت ساختمان جدید
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BuildingFormViewModel model)
    {
        if (model.CommunityId == Guid.Empty)
        {
            ModelState.AddModelError(
                nameof(model.CommunityId), "انتخاب جامعه الزامی است.");
        }

        if (!ModelState.IsValid)
        {
            await LoadCommunitiesAsync(model);
            return View(model);
        }

        try
        {
            var client = CreateApiClient();
            var request = new CreateBuildingRequest(
                model.CommunityId,
                model.Name.Trim(),
                model.Description,
                model.DisplayOrder);

            var response = await client.PostAsJsonAsync(
                "api/buildings", request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "ساختمان با موفقیت ثبت شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                await ReadApiErrorAsync(response, "ثبت ساختمان انجام نشد."));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, GetFriendlyError(ex));
        }

        await LoadCommunitiesAsync(model);
        return View(model);
    }

    // UPDATE: دریافت اطلاعات برای فرم ویرایش
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var client = CreateApiClient();
            var building = await client.GetFromJsonAsync<BuildingApiModel>(
                $"api/buildings/{id}");

            if (building is null)
                return NotFound();

            var communities = await GetCommunitiesAsync(client);
            var communityName = communities
                .FirstOrDefault(x => x.Id == building.CommunityId)?.Name
                ?? "جامعه نامشخص";

            var model = new BuildingFormViewModel
            {
                Id = building.Id,
                Name = building.Name,
                Description = building.Description,
                CommunityId = building.CommunityId,
                CommunityName = communityName,
                DisplayOrder = building.DisplayOrder
            };

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    // UPDATE: ذخیره تغییرات
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, BuildingFormViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var client = CreateApiClient();
            // طبق قرارداد API فعلی، CommunityId در عملیات Update تغییر نمی‌کند.
            var request = new UpdateBuildingRequest(
                model.Name.Trim(),
                model.Description,
                model.DisplayOrder);

            var response = await client.PutAsJsonAsync(
                $"api/buildings/{id}", request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "تغییرات ساختمان ذخیره شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                await ReadApiErrorAsync(response, "ویرایش ساختمان انجام نشد."));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, GetFriendlyError(ex));
        }

        return View(model);
    }

    // DELETE: صفحه تأیید حذف
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var client = CreateApiClient();
            var building = await client.GetFromJsonAsync<BuildingApiModel>(
                $"api/buildings/{id}");

            if (building is null)
                return NotFound();

            return View(new BuildingListItemViewModel
            {
                Id = building.Id,
                Name = building.Name,
                Description = building.Description,
                CommunityId = building.CommunityId,
                DisplayOrder = building.DisplayOrder
            });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    // DELETE: حذف پس از تأیید کاربر
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var client = CreateApiClient();
            var response = await client.DeleteAsync($"api/buildings/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "ساختمان حذف شد.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = await ReadApiErrorAsync(
                response,
                "حذف ساختمان انجام نشد. ممکن است اطلاعات وابسته‌ای به این ساختمان وجود داشته باشد.");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCommunitiesAsync(BuildingFormViewModel model)
    {
        try
        {
            var client = CreateApiClient();
            var communities = await GetCommunitiesAsync(client);

            model.Communities = communities.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == model.CommunityId
            }).ToList();

            if (model.Communities.Count == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "هیچ جامعه‌ای ثبت نشده است. ابتدا یک جامعه ایجاد کنید.");
            }
        }
        catch (Exception ex)
        {
            model.Communities = new List<SelectListItem>();
            ModelState.AddModelError(string.Empty, GetFriendlyError(ex));
        }
    }

    private static async Task<List<CommunityApiModel>> GetCommunitiesAsync(
        HttpClient client)
    {
        return await client.GetFromJsonAsync<List<CommunityApiModel>>(
            "api/communities") ?? new List<CommunityApiModel>();
    }

    private static async Task<string> ReadApiErrorAsync(
        HttpResponseMessage response,
        string fallback)
    {
        var body = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body)
            ? $"{fallback} (کد خطا: {(int)response.StatusCode})"
            : $"{fallback} جزئیات: {body}";
    }

    private static string GetFriendlyError(Exception exception)
    {
        return exception switch
        {
            HttpRequestException =>
                "ارتباط با سرور API برقرار نشد. مطمئن شوید BuildingHub.Api در حال اجراست.",
            InvalidOperationException => exception.Message,
            _ => "عملیات با خطا مواجه شد: " + exception.Message
        };
    }

    private sealed record CreateBuildingRequest(
        Guid CommunityId,
        string Name,
        string? Description,
        int DisplayOrder);

    private sealed record UpdateBuildingRequest(
        string Name,
        string? Description,
        int DisplayOrder);

    private sealed class PagedApiResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }

    private sealed class BuildingApiModel
    {
        public Guid Id { get; set; }
        public Guid CommunityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }

    private sealed class CommunityApiModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
