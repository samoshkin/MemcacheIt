using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using MemcacheIt.Commands;
using MemcacheIt.Runtime;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.Runtime.SpecGetWithCasCommand
{
	[Scenario.Spec]
	public class when_getting_with_cas_for_not_existent_item_from_cache : with_cache_runtime
	{
		GetCommand result;

		public when_getting_with_cas_for_not_existent_item_from_cache()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_does_not_exists_in_cache("simplekey"));
			When(() => result = runtime.GetWithCas(item => item.KeyIs("1")));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_query_cache_for_item_with_given_key_and_cas()
		{
			memcachedClient.Verify();
		}

		[Behavior]
		public void should_specify_result_as_not_found()
		{
			result.Found.Should().BeFalse();
		}

		[Behavior]
		public void when_result_is_not_found_should_not_allow_to_get_value()
		{
			result.Invoking(r => { var value = r.Value; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("was not found in cache", ComparisonMode.Substring);
		}

		[Behavior]
		public void when_result_is_not_found_should_not_allow_to_get_unique_id()
		{
			result.Invoking(r => { var value = r.UniqueID; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("was not found in cache. Unique ID for item could not be queried", ComparisonMode.Substring);
		}

		[Behavior]
		public void should_not_deserialize_not_found_value()
		{
			formattingService.Verify(fs => fs.Deserialize(It.IsAny<object>(), It.IsAny<CommandContext>()), Times.Never());
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
		}

		private void item_does_not_exists_in_cache(string key)
		{
			ulong cas = 0;
			memcachedClient.Setup(c => c.Gets(key, out cas)).Returns(null).Verifiable();
		}
	}

	[Scenario.Spec]
	public class when_getting_with_cas_existent_item_from_cache : with_cache_runtime
	{
		GetCommand result;

		public when_getting_with_cas_existent_item_from_cache()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_exists_in_cache("simplekey", 1, 12));
			Given(() => value_deserialized_to(1, 5));
			When(() => result = runtime.GetWithCas(item => item.KeyIs("nomatter")));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_query_cache_for_item_then_deserialize_it_and_set_result()
		{
			result.Found.Should().BeTrue();
			result.Value.Should().Be(5);
		}

		[Behavior]
		public void should_specify_unique_id_in_result()
		{
			result.UniqueID.Should().Be(12);
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
			formattingService.Verify(formatter => formatter.Deserialize(
				It.IsAny<object>(),
				context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}


		private void item_exists_in_cache(string key, object value, ulong cas)
		{
			var casValue = cas;
			memcachedClient.Setup(c => c.Gets(key, out casValue)).Returns(value);
		}
	}
}