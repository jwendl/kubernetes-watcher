using KubernetesWatcher.Models;
using System.Collections.ObjectModel;

namespace KubernetesWatcher.Extensions
{
	public static class ObservableCollectionExtensions
	{
		public static void AddOrUpdateItem<T>(this ObservableCollection<T> observableCollection, T item, Func<T, bool> predicate)
		{
			if (!observableCollection.Any(predicate))
			{
				observableCollection.Add(item);
			}
			else
			{
				var firstOrDefault = observableCollection.FirstOrDefault(predicate);
				if (firstOrDefault != null)
				{
					var itemIndex = observableCollection.IndexOf(firstOrDefault);
					observableCollection[itemIndex] = item;
				}
			}
		}

		public static void CleanUpItems<T>(this ObservableCollection<T> observableCollection, List<string> names)
			where T : BaseItem
		{
			var namesToRemove = observableCollection.Select(oc => oc.Name).Except(names).ToList();
			foreach (var nameToRemove in namesToRemove)
			{
				var itemToRemove = observableCollection.Where(oc => oc.Name == nameToRemove)
					.FirstOrDefault();

				if (itemToRemove != null)
				{
					observableCollection.Remove(itemToRemove);
				}
			}
		}
	}
}
