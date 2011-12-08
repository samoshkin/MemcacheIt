using System;

namespace MemcacheIt
{
	public class CacheItem
	{
		public Type DataType { get; set; }
		public ulong Cas { get; set; }
		public object Version { get; set; }
		public object Key { get; set; }
		public Cache Cache { get; set; }
		public object Data { get; set; }

		public PropertyBag Properties { get; private set; }
	}
}