namespace EukairiaWeb.Helpers
{
    public class TimeFormatHelper
    {

        public static string getTimeFormat(TimeSpan time)
        {

            int totalHours = (int)Math.Floor(time.TotalHours);  // Obtiene las horas totales como un entero
            int minutes = time.Minutes;  // Obtiene los minutos

            return $"{totalHours:00}:{minutes:00}";
        }
    }
}
