using System;
using MemcacheIt.Commands;
using MemcacheIt.Runtime;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.Runtime
{
	[Scenario.Spec]
	public class when_decrementing_item_in_cache_and_operation_succeeds
		: when_incrementing_item_in_cache_and_operation_succeeds
	{
		protected override DeltaCommand DoDelta(Action<IItemBuilderSyntax> item, ulong delta)
		{
			return runtime.Decrement(item, delta);
		}

		protected override void item_delta_in_performed_sucessfully(string key, ulong delta, ulong incremented)
		{
			memcachedClient.Setup(mc => mc.Decrement(key, delta)).Returns(incremented);
		}
	}

	[Scenario.Spec]
	public class when_decrementing_item_in_cache_and_operation_fails
		: when_incrementing_item_in_cache_and_operation_fails
	{
		protected override DeltaCommand DoDelta(Action<IItemBuilderSyntax> item, ulong delta)
		{
			return runtime.Decrement(item, delta);
		}

		protected override void item_delta_fails(string key, ulong delta)
		{
			memcachedClient.Setup(mc => mc.Decrement(key, delta)).Returns((ulong?)null);
		}
	}
}