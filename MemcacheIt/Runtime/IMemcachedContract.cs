using System;
using MemcacheIt.Commands;

namespace MemcacheIt.Runtime
{
	public interface IMemcachedContract
	{
		object Get(string key);
		object Gets(string key, out ulong unique);
		bool Store(StoreMode storeMode, string key, object data, TimeToLive timeToLive);
		CasResult Cas(string key, object data, ulong uniqueID, TimeToLive timeToLive);
		ulong? Increment(string key, ulong data);
		ulong? Decrement(string key, ulong data);
		bool Delete(string key);
		void FlushAll(TimeSpan delay);
	}
}