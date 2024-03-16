namespace KubernetesWatcher.Extensions
{
	public static class TimeSpanExtensions
	{
		public static string TimeSpanToAgeString(this TimeSpan timeSpan)
		{
			int totalDays = (int)timeSpan.TotalDays;
			int totalHours = (int)timeSpan.TotalHours;
			int totalMinutes = (int)timeSpan.TotalMinutes;

			if (totalDays >= 30)
			{
				int totalMonths = totalDays / 30;
				return $"{totalMonths}M {totalDays % 30}D {totalHours % 24}H";
			}
			else if (totalDays > 0)
			{
				return $"{totalDays}D {totalHours % 24}H";
			}
			else if (totalHours > 0)
			{
				return $"{totalHours}H {totalMinutes % 60}M";
			}
			else
			{
				return $"{totalMinutes}M";
			}
		}
	}
}
