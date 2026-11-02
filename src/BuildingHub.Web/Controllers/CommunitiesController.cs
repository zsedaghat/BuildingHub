using System.Net;
using System.Net.Http.Json;
using BuildingHub.Web.Models.Communities;
using BuildingHub.Web.Models.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Web.Controllers;

public class CommunitiesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public CommunitiesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
            var result = await client.GetFromJsonAsync<PagedApiResponse<CommunityApiModel>>(
                $"api/communities?pageNumber={pageNumber}&pageSize={pageSize}")
                ?? new PagedApiResponse<CommunityApiModel>();
            var model = new PagedListViewModel<CommunityListItemViewModel>
            {
                Items = result.Items.Select(x => new CommunityListItemViewModel
                    { Id = x.Id, Name = x.Name, Description = x.Description }).ToList(),
                PageNumber = result.PageNumber, PageSize = result.PageSize, TotalCount = result.TotalCount
            };
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.Error = GetFriendlyError(ex);
            return View(new PagedListViewModel<CommunityListItemViewModel> { PageNumber = pageNumber, PageSize = pageSize });
        }
    }

    [HttpGet]
    public IActionResult Create() => View(new CommunityFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommunityFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var response = await CreateApiClient().PostAsJsonAsync("api/communities",
                new CreateCommunityRequest(model.Name.Trim(), model.Description?.Trim()));
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "جامعه با موفقیت ثبت شد.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, await ReadApiErrorAsync(response, "ثبت جامعه انجام نشد."));
        }
        catch (Exception ex) { ModelState.AddModelError(string.Empty, GetFriendlyError(ex)); }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var item = await CreateApiClient().GetFromJsonAsync<CommunityApiModel>($"api/communities/{id}");
            if (item is null) return NotFound();
            return View(new CommunityFormViewModel { Id = item.Id, Name = item.Name, Description = item.Description });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CommunityFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        try
        {
            var response = await CreateApiClient().PutAsJsonAsync($"api/communities/{id}",
                new UpdateCommunityRequest(model.Name.Trim(), model.Description?.Trim()));
            if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "تغییرات جامعه ذخیره شد.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, await ReadApiErrorAsync(response, "ویرایش جامعه انجام نشد."));
        }
        catch (Exception ex) { ModelState.AddModelError(string.Empty, GetFriendlyError(ex)); }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var item = await CreateApiClient().GetFromJsonAsync<CommunityApiModel>($"api/communities/{id}");
            if (item is null) return NotFound();
            return View(new CommunityListItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = GetFriendlyError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var response = await CreateApiClient().DeleteAsync($"api/communities/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "جامعه حذف شد.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = await ReadApiErrorAsync(response,
                "حذف جامعه انجام نشد. اگر برای این جامعه ساختمانی ثبت شده باشد، ابتدا باید وابستگی‌ها بررسی شوند.");
        }
        catch (Exception ex) { TempData["ErrorMessage"] = GetFriendlyError(ex); }
        return RedirectToAction(nameof(Index));
    }

    private static async Task<string> ReadApiErrorAsync(HttpResponseMessage response, string fallback)
    {
        var body = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body) ? $"{fallback} (کد خطا: {(int)response.StatusCode})" : $"{fallback} جزئیات: {body}";
    }

    private static string GetFriendlyError(Exception exception) => exception switch
    {
        HttpRequestException => "ارتباط با سرور API برقرار نشد. مطمئن شوید BuildingHub.Api در حال اجراست.",
        InvalidOperationException => exception.Message,
        _ => "عملیات با خطا مواجه شد: " + exception.Message
    };

    private sealed class PagedApiResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }

    private sealed record CreateCommunityRequest(string Name, string? Description);
    private sealed record UpdateCommunityRequest(string Name, string? Description);

    private sealed class CommunityApiModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
