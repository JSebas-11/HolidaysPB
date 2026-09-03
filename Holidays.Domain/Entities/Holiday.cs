using System.ComponentModel.DataAnnotations.Schema;
using HolidaysPB.Domain.Constants;

namespace HolidaysPB.Domain.Entities;

[Table(DomainConstants.Database.Holiday.Table)]
public sealed class Holiday {
    // PROPS
    [Column(DomainConstants.Database.Holiday.Columns.Id)]
    public int Id { get; set; }
    [Column(DomainConstants.Database.Holiday.Columns.Name)]
    public string Name { get; set; } = string.Empty;
    [Column(DomainConstants.Database.Holiday.Columns.Day)]
    public int Day { get; set; }
    [Column(DomainConstants.Database.Holiday.Columns.Month)]
    public int Month { get; set; }
    [Column(DomainConstants.Database.Holiday.Columns.EasterDays)]
    public int EasterDays { get; set; }
    [Column(DomainConstants.Database.Holiday.Columns.TypeId)]
    public int TypeId { get; set; }
    [Column(DomainConstants.Database.Holiday.Columns.CountryId)]
    public int CountryId { get; set; }

    public HolidayType HolidayType { get; set; } = null!;
    public Country Country { get; set; } = null!;
}