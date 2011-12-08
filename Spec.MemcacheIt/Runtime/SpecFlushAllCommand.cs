using System;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.Runtime.SpecFlushAllCommand
{
	[Scenario.Spec]
	public class when_flushing_all_items_from_cache_withouh_specifiying_delay_time
		: with_cache_runtime
	{
		public when_flushing_all_items_from_cache_withouh_specifiying_delay_time()
		{
			When(() => runtime.FlushAll());
		}

		[Behavior]
		public void should_ask_cache_to_flush_all_items_passing_zero_time_span_as_indicator_no_delay_time_required()
		{
			memcachedClient.Verify(mc => mc.FlushAll(TimeSpan.Zero), Times.Once());
		}
	}

	[Scenario.Spec]
	public class when_flushing_all_items_from_cache_specifiying_delay_time
		: with_cache_runtime
	{
		public when_flushing_all_items_from_cache_specifiying_delay_time()
		{
			When(() => runtime.FlushAll(TimeSpan.FromSeconds(10)));
		}

		[Behavior]
		public void should_ask_cache_to_flush_all_items_passing_delay_time()
		{
			memcachedClient.Verify(mc => mc.FlushAll(TimeSpan.FromSeconds(10)), Times.Once());
		}
	}
}