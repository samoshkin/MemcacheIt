using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using MemcacheIt.Commands;
using MemcacheIt.Runtime;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.Runtime.SpecGetCommand
{
	[Scenario.Spec]
	public class when_getting_non_existent_item_from_cache : with_cache_runtime
	{
		private GetCommand result;

		public when_getting_non_existent_item_from_cache()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_does_not_exists_in_cache("simplekey"));
			When(() => result = runtime.Get(item => item.KeyIs("1")));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_query_cache_for_item_with_given_key()
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
		public void when_getting_withouh_cas_should_not_allow_query_unique_id_from_result()
		{
			result.Invoking(r => { var unique = r.UniqueID; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Unique ID could be fetched only when command is configured to fetch unique id", ComparisonMode.Substring);
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

		protected void item_does_not_exists_in_cache(string key)
		{
			memcachedClient.Setup(c => c.Get(key)).Returns(null).Verifiable();
		}
	}

	[Scenario.Spec]
	public class when_getting_existent_item_from_cache : with_cache_runtime
	{
		private GetCommand result;

		public when_getting_existent_item_from_cache()
		{
			Given(() => item_is_handled_and_key_generated("simplekey"));
			Given(() => item_exists_in_cache("simplekey", 1));
			Given(() => value_deserialized_to(1, 2));
			When(() => result = runtime.Get(item => item.KeyIs("nomatterwhatkey")));
		}

		[Behavior]
		public void should_process_item_and_then_build_key()
		{
			itemHandlerPipeline.Verify();
			keyBuilder.Verify();
		}

		[Behavior]
		public void should_specify_query_cache_for_value_then_deserialize_it_and_set_result()
		{
			result.Found.Should().BeTrue();
			result.Value.Should().Be(2);
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
			formattingService.Verify(fs=> fs.Deserialize(
				It.IsAny<object>(),
				context_should_contain_command_of_given_type_and_item(result, result.Item)));
		}

		[Behavior]
		public void when_getting_withouh_cas_should_not_allow_query_unique_id_from_result()
		{
			result.Invoking(r => { var unique = r.UniqueID; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Unique ID could be fetched only when command is configured to fetch unique id", ComparisonMode.Substring);
		}


		protected void item_exists_in_cache(string key, object value)
		{
			memcachedClient.Setup(c => c.Get(key)).Returns(value);
		}
	}

	[Behavior.Spec]
	public class get_command_should_fail_on_creation
	{
		[Scenario]
		public void when_item_is_not_specified()
		{
			Action creation = () => new GetCommand(null, true);
			creation
				.ShouldThrow<ArgumentException>()
				.WithMessage("When getting item, item should be specified", ComparisonMode.Substring);
		}
	}
}