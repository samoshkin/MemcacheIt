namespace MemcacheIt.Ideas
{
	public interface IItemFormatter
	{
		byte[] Serialize(object data, CacheItem item);
		object Deserialize(byte[] data, CacheItem item);
	}
}