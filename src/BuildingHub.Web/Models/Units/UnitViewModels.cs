using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuildingHub.Web.Models.Units;

public sealed class UnitListItemViewModel
{
    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public Guid? FloorId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public string FloorName { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public int Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal? Area { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UnitFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "انتخاب ساختمان الزامی است.")]
    public Guid BuildingId { get; set; }

    public Guid? FloorId { get; set; }

    [Required(ErrorMessage = "شماره واحد الزامی است.")]
    [StringLength(50, ErrorMessage = "شماره واحد نباید بیشتر از ۵۰ کاراکتر باشد.")]
    public string UnitNumber { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "نوع واحد را انتخاب کنید.")]
    public int Type { get; set; } = 1;

    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "متراژ باید بزرگ‌تر از صفر باشد.")]
    public decimal? Area { get; set; }

    public string? BuildingName { get; set; }
    public string? FloorName { get; set; }
    public List<SelectListItem> Buildings { get; set; } = new();
    public List<SelectListItem> Floors { get; set; } = new();
    public List<SelectListItem> UnitTypes { get; set; } = new();
}
