namespace HolidaysPB.Domain.Constants;

public static partial class DomainConstants {
    public static class Length {
        public static class HolidayType {
            public const int MaxType = 100;
            public const string MaxTypeErrorMsg = "Holiday type cannot exceed 100 characters.";
            /*public const int MaxCalcMode = 1024;
            public const string MaxCalcModeErrorMsg = "Calculation mode cannot exceed 1024 characters.";*/
        }
        
        public static class Country {
            public const int MaxName = 100;
            public const string MaxNameErrorMsg = "Country name cannot exceed 100 characters.";
        }
        
        public static class Holiday {
            public const int MaxName = 100;
            public const string MaxNameErrorMsg = "Holiday name cannot exceed 100 characters.";
        }
    }
}