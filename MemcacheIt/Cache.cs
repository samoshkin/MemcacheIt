using System.Collections.Generic;

namespace MemcacheIt
{
	public class Cache
	{
		private readonly IList<ICacheScope> _scopes = new List<ICacheScope>();

		private Cache()
		{}

		public IEnumerable<ICacheScope> Scopes {  get { return _scopes; } }

		public static Cache CreateDefault()
		{
			return new Cache();
		}

		public void Per(ICacheScope cacheScope)
		{
			
			_scopes.Add(cacheScope);
		}
	}
}