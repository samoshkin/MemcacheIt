using CuttingEdge.Conditions;

namespace MemcacheIt.Commands
{
	public class RemoveCommand
	{
		private readonly CacheItem _item;
		private bool _removed;

		public RemoveCommand(CacheItem item)
		{
			Condition.Requires(item).IsNotNull("When removing item, item should be specified.");

			_item = item;
		}
		
		public CacheItem Item { get { return _item; } }
		public bool Removed { get { return _removed; } }

		public void SetResult(bool removed)
		{
			_removed = removed;
		}
	}
}