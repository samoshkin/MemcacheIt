using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using MemcacheIt;
using MemcacheIt.Commands;
using MemcacheIt.Runtime;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.Runtime
{
	[Scenario.Spec]
	public class when_incrementing_item_in_cache_and_operation_succeeds 
		: with_cache_runtime
	{
		public DeltaCommand result;

		public when_incrementing_item_in_cache_and_operation_succeeds()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_delta_in_performed_sucessfully("simplekey", 2, 14));
			When(() => result = DoDelta(item => item.KeyIs("nomatter"), 2));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_specify_incremented_value_and_that_operation_was_successfull()
		{
			result.Succeded.Should().BeTrue();
			result.Result.Should().Be(14);
		}

		[Behavior]
		public void should_not_use_formatter_to_serialize_or_deserialize_value()
		{
			formattingService.Verify(formatter =>
				formatter.Serialize(It.IsAny<object>(), It.IsAny<CommandContext>()), Times.Never());
		}

		[Behavior]
		public void should_supply_command_context_with_get_command_and_given_item()
		{
			itemHandlerPipeline.Verify(pipeline =>
				pipeline.Process(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
			keyBuilder.Verify(kb =>
				kb.Build(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}


		protected virtual void item_delta_in_performed_sucessfully(string key, ulong delta, ulong incremented)
		{
			memcachedClient.Setup(mc => mc.Increment(key, delta)).Returns(incremented);
		}

		protected virtual DeltaCommand DoDelta(Action<IItemBuilderSyntax> item, ulong delta)
		{
			return runtime.Increment(item, delta);
		}
	}
	

	[Scenario.Spec]
	public class when_incrementing_item_in_cache_and_operation_fails 
		: with_cache_runtime
	{
		public DeltaCommand result;

		public when_incrementing_item_in_cache_and_operation_fails()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_delta_fails("simplekey", 2));
			When(() => result = DoDelta(item => item.KeyIs("nomatter"), 2));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_failed()
		{
			result.Succeded.Should().BeFalse();
		}

		[Behavior]
		public void when_operation_failed_should_not_allow_to_get_result()
		{
			result.Invoking(r => { var incremented = r.Result; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Command didn't complete succesfully", ComparisonMode.Substring);
		}

		[Behavior]
		public void should_not_use_formatter_to_serialize_or_deserialize_value()
		{
			formattingService.Verify(formatter =>
				formatter.Serialize(It.IsAny<object>(), It.IsAny<CommandContext>()), Times.Never());
		}

		[Behavior]
		public void should_supply_command_context_with_get_command_and_given_item()
		{
			itemHandlerPipeline.Verify(pipeline =>
				pipeline.Process(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
			keyBuilder.Verify(kb =>
				kb.Build(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}


		protected virtual void item_delta_fails(string key, ulong delta)
		{
			memcachedClient.Setup(mc => mc.Increment(key, delta)).Returns((ulong?)null);
		}

		protected virtual DeltaCommand DoDelta(Action<IItemBuilderSyntax> item, ulong delta)
		{
			return runtime.Increment(item, delta);
		}
	}

	[Behavior.Spec]
	public class delta_command_should_fail_to_create
		: specification
	{
		public delta_command_should_fail_to_create()
		{
			CouldFailWith<ArgumentException>();
			Verify(ShouldFail);
		}

		[Scenario]
		public void when_item_is_not_specified()
		{
			When(() => new DeltaCommand(null, DeltaMode.Increment, 1));
		}

		[Scenario]
		public void when_delta_mode_is_not_specified()
		{
			When(() => new DeltaCommand(new CacheItem(), DeltaMode.None, 1));
		}
	}
}