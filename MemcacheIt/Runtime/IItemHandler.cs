namespace MemcacheIt.Runtime
{
	public interface IItemHandler 
	{
		void Handle(CacheItem cacheItem, CommandContext commandContext);
	}


}