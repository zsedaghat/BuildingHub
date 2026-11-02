using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuildingHub.Web.Models.Buildings;

public sealed class BuildingListItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CommunityId { get; set; }
    public string CommunityName { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public sealed class BuildingFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "نام ساختمان الزامی است.")]
    [StringLength(150, ErrorMessage = "نام ساختمان نباید بیشتر از ۱۵۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "توضیحات نباید بیشتر از ۵۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }

    public Guid CommunityId { get; set; }
    public string CommunityName { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد.")]
    public int DisplayOrder { get; set; } = 1;

    public List<SelectListItem> Communities { get; set; } = new();
}
