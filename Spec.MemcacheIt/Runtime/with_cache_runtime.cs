using System;
using System.Linq;
using MemcacheIt;
using MemcacheIt.Commands;
using MemcacheIt.Runtime;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt
{
	public class with_cache_runtime : specification
	{
		public Mock<KeyBuilder> keyBuilder;
		public Mock<ItemHandlerPipeline> itemHandlerPipeline;
		public Mock<FormattingService> formattingService;
		public Mock<IMemcachedContract> memcachedClient;
		public CacheRuntime runtime;

		public with_cache_runtime()
		{
			Given(cache_runtime);
		}

		protected void cache_runtime()
		{
			itemHandlerPipeline = new Mock<ItemHandlerPipeline>(Enumerable.Empty<IItemHandler>());
			keyBuilder = new Mock<KeyBuilder>(Mock.Of<IKeyGenerator>(), Enumerable.Empty<IKeyTransformation>());
			formattingService = new Mock<FormattingService>();
			memcachedClient = new Mock<IMemcachedContract>();

			runtime = new CacheRuntime(
				keyBuilder.Object,
				itemHandlerPipeline.Object,
				formattingService.Object,
				memcachedClient.Object);
		}

		protected void item_is_handled_and_key_generated(string key)
		{
			var sequence = new MockSequence();

			itemHandlerPipeline
				.InSequence(sequence)
				.Setup(pipeline => pipeline.Process(It.IsAny<CacheItem>(), It.IsAny<CommandContext>()))
				.Verifiable();
			keyBuilder
				.InSequence(sequence)
				.Setup(kb => kb.Build(It.IsAny<CacheItem>(), It.IsAny<CommandContext>()))
				.Returns(new Key(key))
				.Verifiable();
		}

		protected CommandContext context_should_contain_command_of_given_type_and_item<TCommand>(TCommand command, CacheItem item)
		{
			return Match.Create<CommandContext>(context =>
				context.Item.Equals(item) && context.Command.Equals(command));
		}

		protected void value_deserialized_to(object from, object to)
		{
			formattingService
				.Setup(formatter => formatter.Deserialize(from, It.IsAny<CommandContext>()))
				.Returns(to);
		}

		protected void value_serialized_to(object from, object to)
		{
			formattingService.Setup(fs => fs.Serialize(from, It.IsAny<CommandContext>())).Returns(to);
		}

		protected StoreCommand OnStoreItem(StoreMode storeMode, Action<ItemBuilderSyntax> item, TimeToLive ttl)
		{
			switch (storeMode)
			{
				case StoreMode.Set:
					return runtime.Set(item, ttl);
				case StoreMode.Append:
					return runtime.Append(item, ttl);
				case StoreMode.Prepend:
					return runtime.Prepend(item, ttl);
				case StoreMode.Replace:
					return runtime.Replace(item, ttl);
				case StoreMode.Add:
					return runtime.Add(item, ttl);
				default:
					throw new NotSupportedException();
			}
		}
	}
}