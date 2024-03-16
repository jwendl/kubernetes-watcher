namespace KubernetesWatcher.Models
{
	public class PvItem
		: BaseItem
	{
		public string Status { get; set; } = string.Empty;

		public string Capacity { get; set; } = string.Empty;

		public string Age { get; set; } = string.Empty;
	}
}
