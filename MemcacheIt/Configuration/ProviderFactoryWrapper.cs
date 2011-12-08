using System;
using System.Collections.Generic;
using Enyim.Caching.Memcached;

namespace MemcacheIt.Configuration
{
	internal class ProviderFactoryWrapper<T> : IProviderFactory<T>
	{
		private readonly Func<T> _providerFactory;

		public ProviderFactoryWrapper(Func<T> providerFactory)
		{
			_providerFactory = providerFactory;
		}

		public T Create()
		{
			return _providerFactory();
		}

		public void Initialize(Dictionary<string, string> parameters)
		{}
	}
}