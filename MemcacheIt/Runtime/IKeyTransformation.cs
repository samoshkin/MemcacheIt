namespace MemcacheIt.Runtime
{
	public interface IKeyTransformation
	{
		Key Transform(Key key, CacheItem item, CommandContext commandContext);
	}
}