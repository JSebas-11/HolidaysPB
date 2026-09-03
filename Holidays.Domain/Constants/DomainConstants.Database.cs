namespace HolidaysPB.Domain.Constants;

public static partial class DomainConstants {
    public static class Database {
        public static class HolidayType {
            public const string Table = "Tipo";
            public static class Columns {
                public const string Id = "Id";
                public const string Type = "Tipo";
            }
        }
        
        public static class Country {
            public const string Table = "Pais";
            public static class Columns {
                public const string Id = "Id";
                public const string Name = "Nombre";
            }
        }
        
        public static class Holiday {
            public const string Table = "Festivo";
            public static class Columns {
                public const string Id = "Id";
                public const string Name = "Nombre";
                public const string Day = "Dia";
                public const string Month = "Mes";
                public const string EasterDays = "DiasPascua";
                public const string TypeId = "IdTipo";
                public const string CountryId = "IdPais";
            }
        }
    }
}