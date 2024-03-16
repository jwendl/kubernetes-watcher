namespace KubernetesWatcher.Models
{
	public class PvcItem
		: BaseItem
	{
		public string Status { get; set; } = string.Empty;

		public string Volume { get; set; } = string.Empty;

		public string Age { get; set; } = string.Empty;
	}
}
