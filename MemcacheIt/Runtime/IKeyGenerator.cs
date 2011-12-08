namespace MemcacheIt.Runtime
{
	public interface IKeyGenerator
	{
		Key Generate(CacheItem item, CommandContext commandContext);
	}
}