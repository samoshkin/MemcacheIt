using System;

namespace MemcacheIt.Runtime
{
	public class Key
	{
		private readonly string _key;
		private const string DEFAULT_PATH_SEPARATOR = ".";

		public Key(string key)
		{
			_key = key;
		}

		public Key AddSuffix(string keySuffix, string pathSeparator = DEFAULT_PATH_SEPARATOR)
		{
			return new Key(String.Concat(_key, pathSeparator, keySuffix));
		}

		public Key AddPrefix(string keyPrefix, string pathSeparator = DEFAULT_PATH_SEPARATOR)
		{
			return new Key(String.Concat(keyPrefix, pathSeparator, _key));
		}

		public static implicit operator string(Key key)
		{
			return key._key;
		}

		public override string ToString()
		{
			return _key;
		}
	}
}