namespace ExpenseManagement.Api.Common
{
    public static class DateTimeDeconstructExtensions
    {

        public static void Deconstruct(this DateOnly source, out int year, out int month, out int day)
        {

            year = source.Year;
            month = source.Month;
            day = source.Day;
        }

        public static void Deconstruct(this DateTime source, out int year, out int month, out int day)
        {
            year = source.Year;
            month = source.Month;
            day = source.Day;
        }

        public static void Deconstruct(this DateTimeOffset source, out int year, out int month, out int day)
        {
            year = source.Year;
            month = source.Month;
            day = source.Day;
        }

        public static void Deconstruct(this DateTimeOffset source, out int year, out int month, out int day, out int hour, out int minute, out int second)
        {
            year = source.Year;
            month = source.Month;
            day = source.Day;
            hour = source.Hour;
            minute = source.Minute;
            second = source.Second;
        }

        public static void Deconstruct(this DateTime source, out int year, out int month, out int day, out int hour, out int minute, out int second)
        {
            year = source.Year;
            month = source.Month;
            day = source.Day;
            hour = source.Hour;
            minute = source.Minute;
            second = source.Second;
        }
    }

    public static class DeconstructExtensions
    {


    }
}
