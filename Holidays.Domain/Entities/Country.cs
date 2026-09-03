using System.ComponentModel.DataAnnotations.Schema;
using HolidaysPB.Domain.Constants;

namespace HolidaysPB.Domain.Entities;

[Table(DomainConstants.Database.Country.Table)]
public sealed class Country {
    // PROPS
    [Column(DomainConstants.Database.Country.Columns.Id)]
    public int Id { get; set; }
    [Column(DomainConstants.Database.Country.Columns.Name)]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Holiday> Holidays { get; set; } = [];
}