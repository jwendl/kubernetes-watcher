namespace KubernetesWatcher.Extensions
{
	public static class DateTimeExtensions
	{
		public static string DateTimeToAgeString(this DateTime? dateTime)
		{
			if (dateTime.HasValue)
			{
				return DateTime.UtcNow.Subtract(dateTime.Value).TimeSpanToAgeString();
			}

			return string.Empty;
		}
	}
}
