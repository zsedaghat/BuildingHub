using System.ComponentModel.DataAnnotations;

namespace BuildingHub.Web.Models.Communities;

public sealed class CommunityListItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class CommunityFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "نام جامعه الزامی است.")]
    [StringLength(150, ErrorMessage = "نام جامعه نباید بیشتر از ۱۵۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "توضیحات نباید بیشتر از ۵۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }
}

public sealed class CommunityIndexViewModel
{
    public IReadOnlyList<CommunityListItemViewModel> Items { get; set; } = Array.Empty<CommunityListItemViewModel>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public int FirstItem => TotalCount == 0 ? 0 : ((Page - 1) * PageSize) + 1;
    public int LastItem => Math.Min(Page * PageSize, TotalCount);
}
