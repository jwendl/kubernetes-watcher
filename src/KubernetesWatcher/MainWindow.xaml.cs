using k8s;
using k8s.Models;
using KubernetesWatcher.Extensions;
using KubernetesWatcher.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Timer = System.Timers.Timer;

namespace KubernetesWatcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly string namespaceName = "hermes";

		private readonly Timer timer = new(2000);
		private readonly Kubernetes kubernetes;

		public ObservableCollection<NodeItem> NodeItems = [];
		public ObservableCollection<PodItem> PodItems = [];
		public ObservableCollection<PvcItem> PvcItems = [];
		public ObservableCollection<PvItem> PvItems = [];

		public MainWindow()
		{
			InitializeComponent();

			var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			var kubeConfigPath = Path.Combine(home, ".kube", "config");

			var kubernetesConfiguration = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigPath);
			kubernetes = new Kubernetes(kubernetesConfiguration);

			PodGrid.ItemsSource = PodItems;
			NodeGrid.ItemsSource = NodeItems;
			PvcGrid.ItemsSource = PvcItems;
			PvGrid.ItemsSource = PvItems;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Elapsed += async (sender, e) => await Timer_Elapsed(sender, e);
			timer.Start();
		}

		private async Task Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			var nodes = await kubernetes.ListNodeAsync();
			var pods = await kubernetes.ListNamespacedPodAsync(namespaceName);
			var pvcs = await kubernetes.ListNamespacedPersistentVolumeClaimAsync(namespaceName);
			var pvs = await kubernetes.ListPersistentVolumeAsync();

			Dispatcher.Invoke(() =>
			{
				foreach (var node in nodes)
				{
					var conditions = node.Status.Conditions;
					var lastCondition = conditions.OrderByDescending(c => c.LastTransitionTime).FirstOrDefault();
					NodeItems.AddOrUpdateItem(new NodeItem()
					{
						Name = node.Name(),
						Status = lastCondition?.Type!,
						Age = node.Metadata.CreationTimestamp.DateTimeToAgeString(),
					}, n => n.Name == node.Name());
				}
				NodeItems.CleanUpItems(nodes.Items.Select(n => n.Name()).ToList());

				foreach (var pod in pods)
				{
					var conditions = pod.Status.Conditions;
					var lastCondition = conditions.OrderByDescending(c => c.LastTransitionTime).FirstOrDefault();
					var restartCount = pod.Status.ContainerStatuses?.Sum(cs => cs.RestartCount);
					PodItems.AddOrUpdateItem(new PodItem()
					{
						Name = pod.Name(),
						Status = lastCondition?.Type!,
						Restarts = restartCount?.ToString()!,
						Age = pod.Metadata.CreationTimestamp.DateTimeToAgeString(),
					}, p => p.Name == pod.Name());
				}
				PodItems.CleanUpItems(pods.Items.Select(n => n.Name()).ToList());

				foreach (var pvc in pvcs)
				{
					var phase = pvc.Status.Phase;
					PvcItems.AddOrUpdateItem(new PvcItem()
					{
						Name = pvc.Name(),
						Status = phase,
						Volume = pvc.Spec.VolumeName,
						Age = pvc.Metadata.CreationTimestamp.DateTimeToAgeString(),
					}, p => p.Name == pvc.Name());
				}
				PvcItems.CleanUpItems(pvcs.Items.Select(n => n.Name()).ToList());

				foreach (var pv in pvs)
				{
					PvItems.AddOrUpdateItem(new PvItem()
					{
						Name = pv.Name(),
						Status = pv.Status.Phase,
						Capacity = pv.Spec.Capacity.First().Value.Value,
						Age = pv.Metadata.CreationTimestamp.DateTimeToAgeString(),
					}, p => p.Name == pv.Name());
				}
				PvItems.CleanUpItems(pvs.Items.Select(n => n.Name()).ToList());

				var podNameColumn = PodGrid.Columns.Where(c => c.Header.ToString() == "Name").First();
				podNameColumn.DisplayIndex = 0;
				podNameColumn.SortDirection = ListSortDirection.Ascending;

				var nodeNameColumn = NodeGrid.Columns.Where(c => c.Header.ToString() == "Name").First();
				nodeNameColumn.DisplayIndex = 0;
				nodeNameColumn.SortDirection = ListSortDirection.Descending;

				var pvcNameColumn = PvcGrid.Columns.Where(c => c.Header.ToString() == "Name").First();
				pvcNameColumn.DisplayIndex = 0;
				pvcNameColumn.SortDirection = ListSortDirection.Ascending;

				var pvNameColumn = PvGrid.Columns.Where(c => c.Header.ToString() == "Name").First();
				pvNameColumn.DisplayIndex = 0;
				pvNameColumn.SortDirection = ListSortDirection.Descending;
			});
		}
	}
}
