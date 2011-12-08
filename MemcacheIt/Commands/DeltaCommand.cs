using System;
using CuttingEdge.Conditions;

namespace MemcacheIt.Commands
{
	public class DeltaCommand
	{
		private readonly ulong _delta;
		private readonly DeltaMode _deltaMode;
		private readonly CacheItem _item;
		private ulong _result;
		private bool _succeded;

		public DeltaCommand(CacheItem item, DeltaMode mode, ulong delta)
		{
			Condition.Requires(item).IsNotNull(
				"When executing increment/decrement command, cache item should be specified.");
			Condition.Requires(mode).IsNotEqualTo(DeltaMode.None, 
				"When executing increment/decrement command against item '{0}', delta mode should be specified."
					.FormatString(item));

			_item = item;
			_deltaMode = mode;
			_delta = delta;
		}
		
		public ulong		Delta { get { return _delta; } }
		public CacheItem	Item { get { return _item; } }
		public DeltaMode	Mode { get { return _deltaMode; } }

		public bool		Succeded { get { return _succeded; } }
		public ulong	Result
		{
			get
			{
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					   .Requires(Succeded).IsTrue("Command didn't complete succesfully.");
				return _result;
			}
		}

		public void SetResult(ulong? result)
		{
			if(result != null)
			{
				_succeded = true;
				_result = result.Value;
			}
			else
			{
				_succeded = false;
			}
		}
	}

	public enum DeltaMode
	{
		None = 0,

		Increment,
		Decrement
	}

}