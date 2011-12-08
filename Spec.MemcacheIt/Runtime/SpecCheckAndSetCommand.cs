using System;
using FluentAssertions;
using MemcacheIt.Commands;
using Moq;
using SimpleSpec.NUnit;
using Spec.MemcacheIt;

namespace MemcacheIt.Runtime.SpecCheckAndSetCommand
{
	[Scenario.Spec]
	public class when_setting_item_with_check_and_operation_succeeds
		: with_cache_runtime
	{
		StoreCommand result;

		public when_setting_item_with_check_and_operation_succeeds()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => value_serialized_to("source_value", "formatted_value"));
			Given(() => item_is_checked_and_stored_in_cache_sucessfully(
				"simplekey",
				"formatted_value",
				12,
				TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
			When(() => result = runtime.CheckAndSet(
				item => item.ValueOf("source_value"),
				12, 
				TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
		}
		
		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_ask_cache_to_check_and_store_item()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_successful_in_result()
		{
			result.Succeded.Should().BeTrue();
		}

		[Behavior]
		public void should_specify_cas_operation_result()
		{
			result.CasResult.Should().Be(CasResult.Stored);
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

		private void item_is_checked_and_stored_in_cache_sucessfully(string key, object value, ulong uniqueID, TimeToLive timeToLive)
		{
			memcachedClient.Setup(mc => mc.Cas(key, value, uniqueID, timeToLive)).Returns(CasResult.Stored).Verifiable();
		}
	}

	[Scenario.Spec]
	public class when_setting_item_with_check_and_operation_fails
		: with_cache_runtime
	{
		StoreCommand result;

		public when_setting_item_with_check_and_operation_fails()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => value_serialized_to("source_value", "formatted_value"));
			Given(() => item_is_checked_but_is_not_stored_successfully(
				"simplekey",
				"formatted_value",
				12,
				TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
			When(() => result = runtime.CheckAndSet(
				item => item.ValueOf("source_value"),
				12,
				TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_ask_cache_to_check_and_store_item()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_failed_in_result()
		{
			result.Succeded.Should().BeFalse();
		}

		[Behavior]
		public void should_specify_cas_operation_result_as_not_stored()
		{
			result.CasResult.Should().Be(CasResult.NotStored);
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

		private void item_is_checked_but_is_not_stored_successfully(string key, object value, ulong uniqueID, TimeToLive timeToLive)
		{
			memcachedClient.Setup(mc => mc.Cas(key, value, uniqueID, timeToLive)).Returns(CasResult.NotStored).Verifiable();
		}
	}

	[Scenario.Spec]
	public class when_setting_item_with_check_and_item_was_already_updated_by_someone_else
		: with_cache_runtime
	{
		StoreCommand result;

		public when_setting_item_with_check_and_item_was_already_updated_by_someone_else()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => value_serialized_to("source_value", "formatted_value"));
			Given(() => item_was_already_updated_by_someone_else(
				"simplekey",
				"formatted_value",
				12,
				TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
			When(() => result = runtime.CheckAndSet(
				item => item.ValueOf("source_value"),
				12,
				TimeToLive.CreateValidFor(TimeSpan.FromMinutes(2))));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_ask_cache_to_check_and_store_item()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_operation_was_failed_in_result()
		{
			result.Succeded.Should().BeFalse();
		}

		[Behavior]
		public void should_specify_cas_operation_result()
		{
			result.CasResult.Should().Be(CasResult.NotFound);
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


		private void item_was_already_updated_by_someone_else(string key, object value, ulong uniqueID, TimeToLive timeToLive)
		{
			memcachedClient.Setup(mc => mc.Cas(key, value, uniqueID, timeToLive)).Returns(CasResult.NotFound).Verifiable();
		}
	}
}