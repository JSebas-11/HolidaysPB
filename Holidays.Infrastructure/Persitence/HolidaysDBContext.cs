using HolidaysPB.Domain.Constants;
using HolidaysPB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HolidaysPB.Infrastructure.Persitence;

public sealed class HolidaysDBContext : DbContext {
    public HolidaysDBContext(DbContextOptions<HolidaysDBContext> options)
        : base(options)
    { }

    // TABLES
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<HolidayType> HolidayTypes => Set<HolidayType>();
    public DbSet<Country> Countries => Set<Country>();

    // CONFIG
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Tipo
        modelBuilder.Entity<HolidayType>(entity => {
            entity.ToTable(DomainConstants.Database.HolidayType.Table);

            entity.HasKey(x => x.Id)
                .HasName("pkTipo_Id");
            entity.Property(x => x.Id)
                .HasColumnName(DomainConstants.Database.HolidayType.Columns.Id)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Type)
                .HasColumnName(DomainConstants.Database.HolidayType.Columns.Type)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsRequired();
        });

        // Pais
        modelBuilder.Entity<Country>(entity => {
            entity.ToTable(DomainConstants.Database.Country.Table);

            entity.HasKey(x => x.Id)
                .HasName("pkPais_Id");
            entity.Property(x => x.Id)
                .HasColumnName(DomainConstants.Database.Country.Columns.Id)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Name)
                .HasColumnName(DomainConstants.Database.Country.Columns.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsRequired();
        });

        // Festivo
        modelBuilder.Entity<Holiday>(entity => {
            entity.ToTable(DomainConstants.Database.Holiday.Table);

            entity.HasKey(x => x.Id)
                .HasName("pkFestivo_Id");
            entity.Property(x => x.Id)
                .HasColumnName(DomainConstants.Database.Holiday.Columns.Id)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Name)
                .HasColumnName(DomainConstants.Database.Holiday.Columns.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(x => x.Day)
                .HasColumnName(DomainConstants.Database.Holiday.Columns.Day)
                .IsRequired();

            entity.Property(x => x.Month)
                .HasColumnName(DomainConstants.Database.Holiday.Columns.Month)
                .IsRequired();

            entity.Property(x => x.EasterDays)
                .HasColumnName(DomainConstants.Database.Holiday.Columns.EasterDays)
                .IsRequired();

            entity.Property(x => x.TypeId)
                .HasColumnName(DomainConstants.Database.Holiday.Columns.TypeId)
                .IsRequired();

            entity.Property(x => x.CountryId)
                .HasColumnName(DomainConstants.Database.Holiday.Columns.CountryId)
                .IsRequired();

            // FKs
            entity.HasOne(x => x.HolidayType)
                .WithMany(x => x.Holidays)
                .HasForeignKey(x => x.TypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fkFestivo_Tipo");

            entity.HasOne(x => x.Country)
                .WithMany(x => x.Holidays)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fkFestivo_Pais");
        });
    }
}