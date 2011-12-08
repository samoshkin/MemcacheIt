using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using MemcacheIt;
using MemcacheIt.Commands;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.Runtime.SpecSetCommand
{
	[Scenario.Spec]
	public class when_setting_item_in_cache_and_operation_succeeds : with_cache_runtime
	{
		public StoreCommand result;

		public when_setting_item_in_cache_and_operation_succeeds()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => value_serialized_to("source_value", "formatted_value"));
			Given(() => item_is_stored_in_cache_sucessfully("simplekey", "formatted_value", TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
			When(() => result = OnStoreItem(GetStoreMode(), item => item.ValueOf("source_value"), TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
		}

		protected virtual StoreMode GetStoreMode()
		{
			return StoreMode.Set;
		}


		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_ask_cache_to_store_item()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_successful_in_result()
		{
			result.Succeded.Should().BeTrue();
		}

		[Behavior]
		public void should_supply_command_context_with_get_command_and_given_item()
		{
			itemHandlerPipeline.Verify(pipeline =>
				pipeline.Process(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
			keyBuilder.Verify(kb =>
				kb.Build(result.Item, context_should_contain_command_of_given_type_and_item(result, result.Item)));
			formattingService.Verify(formatter =>
				formatter.Serialize(It.IsAny<object>(), context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}

		private void item_is_stored_in_cache_sucessfully(string key, object value, TimeToLive timeToLive)
		{
			memcachedClient.Setup(mc => mc.Store(GetStoreMode(), key, value, timeToLive)).Returns(true).Verifiable();
		}
	}

	[Scenario.Spec]
	public class when_setting_item_in_cache_and_operation_fails : with_cache_runtime
	{
		public StoreCommand result;
		TimeToLive ttl = TimeToLive.CreateValidFor(TimeSpan.FromSeconds(2));

		public when_setting_item_in_cache_and_operation_fails()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => value_serialized_to("source_value", "formatted_value"));
			Given(() => item_is_not_stored_in_cache("simplekey", "formatted_value", ttl));
			When(() => result = OnStoreItem(GetStoreMode(), item => item.ValueOf("source_value"), ttl));
		}

		protected virtual StoreMode GetStoreMode()
		{
			return StoreMode.Set;
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_ask_cache_to_store_item()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_failed_in_result()
		{
			result.Succeded.Should().BeFalse();
		}

		[Behavior]
		public void result_should_contain_item_time_to_live()
		{
			result.TimeToLive.Should().Be(ttl);
		}

		[Behavior]
		public void when_command_is_not_in_check_and_set_mode_should_not_allow_to_get_cas_result_and_uniqueid()
		{
			result.Invoking(r => { var casResult = r.CasResult; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Cas result is only accessible in 'Check and set' mode", ComparisonMode.Substring);
			result.Invoking(r => { var uniqueID = r.UniqueID; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Unique ID is only accessible in 'Check and set' mode", ComparisonMode.Substring);
		}


		[Behavior]
		public void should_supply_command_context_with_get_command_and_given_item()
		{
			itemHandlerPipeline.Verify(pipeline => pipeline.Process(
				result.Item,
				context_should_contain_command_of_given_type_and_item(result, result.Item)));
			keyBuilder.Verify(kb => kb.Build(
				result.Item,
				context_should_contain_command_of_given_type_and_item(result, result.Item)));
			formattingService.Verify(formatter => formatter.Serialize(
				It.IsAny<object>(),
				context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}

		private void item_is_not_stored_in_cache(string key, object value, TimeToLive timeToLive)
		{
			memcachedClient.Setup(mc => mc.Store(GetStoreMode(), key, value, timeToLive)).Returns(false).Verifiable();
		}
	}

	[Behavior.Spec]
	public class store_command_should_fail_on_creation
		: specification
	{
		[Scenario]
		public void when_item_is_not_specified()
		{
			Action creation = () => new StoreCommand(null, TimeToLive.CreateNeverExpiring(), StoreMode.Append);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("When storing item, item should be specified.", ComparisonMode.Substring);
		}

		[Scenario]
		public void when_items_value_is_not_specified()
		{
			Action creation = () => new StoreCommand(new CacheItem(), TimeToLive.CreateNeverExpiring(), StoreMode.Append);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("item's value should be specified", ComparisonMode.Substring);
		}

		[Scenario]
		public void when_time_to_live_is_not_specified()
		{
			Action creation = () => new StoreCommand(new CacheItem { Data =  1}, null, StoreMode.Append);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("item's time to live should be specified", ComparisonMode.Substring);
		}

		[Scenario]
		public void when_specifying_check_and_set_store_mode()
		{
			Action creation = () => new StoreCommand(new CacheItem { Data =  1}, TimeToLive.CreateNeverExpiring(), StoreMode.CheckAndSet);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("To initialize 'Check and Set' command use another constructor, which accepts unique ID.", ComparisonMode.Substring);
		}
	}

	[Behavior.Spec]
	public class check_and_set_command_should_fail_on_creation
		: specification
	{
		[Scenario]
		public void when_item_is_not_specified()
		{
			Action creation = () => new StoreCommand(null, TimeToLive.CreateNeverExpiring(), 1);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("When storing item, item should be specified.", ComparisonMode.Substring);
		}

		[Scenario]
		public void when_items_value_is_not_specified()
		{
			Action creation = () => new StoreCommand(new CacheItem(), TimeToLive.CreateNeverExpiring(), 1);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("item's value should be specified", ComparisonMode.Substring);
		}

		[Scenario]
		public void when_time_to_live_is_not_specified()
		{
			Action creation = () => new StoreCommand(new CacheItem { Data = 1 }, null, 1);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("item's time to live should be specified", ComparisonMode.Substring);
		}
	}
}