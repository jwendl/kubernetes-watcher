namespace KubernetesWatcher.Models
{
	public class PodItem
		: BaseItem
	{
		public string Status { get; set; } = string.Empty;

		public string Restarts { get; set; } = string.Empty;

		public string Age { get; set; } = string.Empty;
	}
}
