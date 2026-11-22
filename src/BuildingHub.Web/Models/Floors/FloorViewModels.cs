using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuildingHub.Web.Models.Floors;

public sealed class FloorListItemViewModel
{
    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class FloorFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "انتخاب ساختمان الزامی است.")]
    public Guid BuildingId { get; set; }

    [Required(ErrorMessage = "نام طبقه الزامی است.")]
    [StringLength(100, ErrorMessage = "نام طبقه نباید بیشتر از ۱۰۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    public int FloorNumber { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد.")]
    public int DisplayOrder { get; set; }

    public List<SelectListItem> Buildings { get; set; } = new();
}
