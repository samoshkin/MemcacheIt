using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using MemcacheIt;
using MemcacheIt.Commands;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.Runtime
{
	[Scenario.Spec]
	public class when_removing_item_from_cache_and_operation_succeeds : with_cache_runtime
	{
		public RemoveCommand result;

		public when_removing_item_from_cache_and_operation_succeeds()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_is_removed_from_cache_successfully("simplekey"));
			When(() => result = runtime.Remove(item => item.KeyIs("no_matter")));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_ask_cache_to_delete_item()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_successful_in_result()
		{
			result.Removed.Should().BeTrue();
		}

		[Behavior]
		public void should_supply_command_context_with_get_command_and_given_item()
		{
			itemHandlerPipeline.Verify(pipeline =>
				pipeline.Process(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
			keyBuilder.Verify(kb =>
				kb.Build(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}

		private void item_is_removed_from_cache_successfully(string key)
		{
			memcachedClient.Setup(mc => mc.Delete(key)).Returns(true).Verifiable();
		}
	}

	[Scenario.Spec]
	public class when_removing_item_from_cache_and_operation_fails : with_cache_runtime
	{
		public RemoveCommand result;

		public when_removing_item_from_cache_and_operation_fails()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_is_not_removed_from_cache("simplekey"));
			When(() => result = runtime.Remove(item => item.KeyIs("no_matter")));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_ask_cache_to_delete_item()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_successful_in_result()
		{
			result.Removed.Should().BeFalse();
		}

		[Behavior]
		public void should_supply_command_context_with_get_command_and_given_item()
		{
			itemHandlerPipeline.Verify(pipeline =>
				pipeline.Process(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
			keyBuilder.Verify(kb =>
				kb.Build(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}

		private void item_is_not_removed_from_cache(string key)
		{
			memcachedClient.Setup(mc => mc.Delete(key)).Returns(false).Verifiable();
		}
	}

	[Behavior.Spec]
	public class get_command_should_fail_on_creation
	{
		[Scenario]
		public void when_item_is_not_specified()
		{
			Action creation = () => new RemoveCommand(null);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("When removing item, item should be specified.", ComparisonMode.Substring);
		}
	}
}