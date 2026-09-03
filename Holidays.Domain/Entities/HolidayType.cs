using System.ComponentModel.DataAnnotations.Schema;
using HolidaysPB.Domain.Constants;

namespace HolidaysPB.Domain.Entities;

[Table(DomainConstants.Database.HolidayType.Table)]
public sealed class HolidayType {
    // PROPS
    [Column(DomainConstants.Database.HolidayType.Columns.Id)]
    public int Id { get; set; }
    [Column(DomainConstants.Database.HolidayType.Columns.Type)]
    public string Type { get; set; } = string.Empty;
    
    public ICollection<Holiday> Holidays { get; set; } = [];
}