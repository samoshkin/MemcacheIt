using System;
using CuttingEdge.Conditions;

namespace MemcacheIt.Commands
{
	public class StoreCommand
	{
		private readonly TimeToLive _timeToLive;
		private readonly StoreMode _storeMode;
		private ulong? _uniqueID;
		private readonly CacheItem _item;

		private bool _succeded;
		private CasResult? _casResult;

		public StoreCommand(CacheItem item, TimeToLive timeToLive, StoreMode storeMode)
		{
			Condition.Requires(item).IsNotNull(
				"When storing item, item should be specified.");
			Condition.Requires(item.Data).IsNotNull(
				"When storing item '{0}', item's value should be specified.".FormatString(item));
			Condition.Requires(timeToLive).IsNotNull(
				"When storing item '{0}', item's time to live should be specified.".FormatString(item));
			Condition.Requires(storeMode).IsNotEqualTo(StoreMode.CheckAndSet,
				"To initialize 'Check and Set' command use another constructor, which accepts unique ID.");

			_item = item;
			_timeToLive = timeToLive;
			_storeMode = storeMode;
			_uniqueID = null;
		}

		public StoreCommand(CacheItem item, TimeToLive timeToLive, ulong uniqueID)
		{
			Condition.Requires(item).IsNotNull(
				"When storing item, item should be specified.");
			Condition.Requires(timeToLive).IsNotNull(
				"When storing item '{0}', item's time to live should be specified".FormatString(item));
			Condition.Requires(item.Data).IsNotNull(
				"When storing item '{0}', item's value should be specified.".FormatString(item));

			_item = item;
			_timeToLive = timeToLive;
			_storeMode = StoreMode.CheckAndSet;
			_uniqueID = uniqueID;
		}

		public TimeToLive	TimeToLive { get { return _timeToLive; } }
		public StoreMode	StoreMode { get {  return _storeMode ; } }
		public CacheItem	Item { get { return _item; } }
		
		public bool Succeded { get { return _succeded; } }
		
		public CasResult? CasResult
		{
			get
			{
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					.Requires(IsCheckAndSet).IsTrue("Cas result is only accessible in 'Check and set' mode.");
				return _casResult;
			}
		}
		public bool IsCheckAndSet { get { return _storeMode == StoreMode.CheckAndSet; } }
		public ulong UniqueID
		{
			get
			{
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					.Requires(IsCheckAndSet).IsTrue("Unique ID is only accessible in 'Check and set' mode.");
				return _uniqueID.Value;
			}
		}

		public void SetResult(bool succeded)
		{
			_succeded = succeded;
		}

		public void SetResult(CasResult casResult)
		{
			Condition.Requires(casResult).IsNotEqualTo(MemcacheIt.CasResult.None,
				"Cas result should be specified. Value of '{0}' is not acceptable.".FormatString(casResult));

			_casResult = casResult;
			_succeded = casResult == MemcacheIt.CasResult.Stored;
		}
	}

	public enum StoreMode
	{
		None = 0,
		Set,
		Append,
		Prepend,
		Add,
		Replace,
		CheckAndSet
	}
}