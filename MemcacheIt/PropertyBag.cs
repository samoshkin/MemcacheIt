using System;
using System.Collections;

namespace MemcacheIt
{
	public class PropertyBag
	{
		private Hashtable _properties = new Hashtable();

		public void Set(Type type, string name, object property)
		{
		}

		public void Set(Type type, object property)
		{
			throw new NotImplementedException();
		}

		public void Set<T>(string name, object property)
		{
			Set(typeof(T), name, property);
		}

		public void Set<T>(object property)
		{
			Set(typeof(T), property);
		}



		public object Get(Type typeOfProperty)
		{
			throw new NotImplementedException();
		}

		public T Get<T>()
		{
			return (T) Get(typeof (T));
		}

		public object Get(Type propertyType, string propertyName)
		{
			throw new NotImplementedException();
		}
		
		public T Get<T>(string name)
		{
			return (T) Get(typeof (T), name);
		}
	}
}