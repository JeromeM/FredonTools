using System.Globalization;

namespace SasFredonWPF.Helpers
{
    public static class DateHelper
    {
        public static string CurrentMonth => CultureInfo.GetCultureInfo("fr-FR").TextInfo.ToTitleCase(DateTime.Today.ToString("MMMM", CultureInfo.GetCultureInfo("fr-FR")));

        public static string CurrentYear => DateTime.Today.Year.ToString();
    }
}
