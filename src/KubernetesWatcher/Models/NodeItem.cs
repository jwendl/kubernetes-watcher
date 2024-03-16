namespace KubernetesWatcher.Models
{
	public class NodeItem
		: BaseItem
	{
		public string Status { get; set; } = string.Empty;

		public string Age { get; set; } = string.Empty;
	}
}
