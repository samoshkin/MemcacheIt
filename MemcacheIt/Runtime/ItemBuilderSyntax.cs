using System;
using CuttingEdge.Conditions;

namespace MemcacheIt.Runtime
{
	public class ItemBuilderSyntax : IItemBuilderSyntax
	{
		private CacheItem _buildMe;

		public ItemBuilderSyntax()
		{
			_buildMe = new CacheItem();
		}

		public ItemBuilderSyntax(CacheItem buildMe)
		{
			_buildMe = buildMe;
		}

		public IItemBuilderSyntax ValueOf<T>(T value)
		{
			Condition.Requires(value).Evaluate(typeof (T).IsValueType || !Equals(value, default(T)));
			
			_buildMe.Data = value;
			_buildMe.DataType = value.GetType();

			return this;
		}

		public IItemBuilderSyntax ValueOf(object value)
		{
			Condition.Requires(value).IsNotNull("Item cannot have nullable value.");

			_buildMe.Data = value;
			_buildMe.DataType = value.GetType();

			return this;
		}

		public IItemBuilderSyntax OfType(Type type)
		{
			_buildMe.DataType = type;

			return this;
		}

		public IItemBuilderSyntax OfType<T>()
		{
			_buildMe.DataType = typeof (T);

			return this;
		}

		public IItemBuilderSyntax InCache(ICacheScope cacheScope)
		{
			_buildMe.Cache.Per(cacheScope);

			return this;
		}

		public IItemBuilderSyntax KeyIs(object key)
		{
			_buildMe.Key = key;

			return this;
		}

		public IItemBuilderSyntax KeyIsGeneratedBy(object keyGenerationStrategy)
		{
			throw new NotImplementedException();
		}

		public IItemBuilderSyntax VersionIs(object version)
		{
			_buildMe.Version = version;

			return this;
		}

		public IItemBuilderSyntax VersionIsCalculatedBy(object versionItemHandler)
		{
			throw new NotImplementedException();
		}

		public IItemBuilderSyntax WithProperty(object customProperty)
		{
			throw new NotImplementedException();
		}

		public CacheItem BuildItem()
		{
			var item = _buildMe;

			// prevent any other modifications to item once it is built.
			// other methods should check this condition and fail gracefully
			// however, now they will fail with NullReferenceException if try to call them after call to this method
			_buildMe = null;
			return item;
		}
	}
}