namespace MemcacheIt.Runtime
{
	public class CommandContext
	{
		public CommandContext(object operation, CacheItem targetItem)
		{
			Command = operation;
			Item = targetItem;
		}

		public object		Command { get; private set; }
		public CacheItem	Item { get; private set; }
	}
}