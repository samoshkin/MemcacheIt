using System;
using CuttingEdge.Conditions;

namespace MemcacheIt.Commands
{
	public class GetCommand
	{
		private readonly bool _fetchUniqueID;
		private CacheItem _item;
		private bool _found;

		public GetCommand(CacheItem item, bool fetchUniqueID)
		{
			Condition.Requires(item).IsNotNull(
				"When getting item, item should be specified.");

			_item = item;
			_fetchUniqueID = fetchUniqueID;
		}

		public CacheItem	Item { get { return _item; } }
		public bool			FetchUniqueID { get { return _fetchUniqueID; } }
		
		public object		Value
		{
			get 
			{
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					.Requires(Found).IsTrue("Item '{0}' was not found in cache.".FormatString(Item));
				return _item.Data;
			}
		}
		public ulong		UniqueID
		{
			get
			{
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					.Requires(FetchUniqueID).IsTrue("Unique ID could be fetched only when command is configured to fetch unique id.");
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					.Requires(Found).IsTrue("Item '{0}' was not found in cache. Unique ID for item could not be queried.".FormatString(Item));
				return _item.Cas;
			}
		}
		public bool			Found { get { return _found; } }

		internal void ItemNotFound()
		{
			_found = false;
		}

		internal void ItemFound(CacheItem item)
		{
			Condition.Requires(item.Data).IsNotNull("If item is found, item's value expected to be assigned.");
			_item = item;
			_found = true;
		}
	}
}