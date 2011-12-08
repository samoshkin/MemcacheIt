using System;
using FluentAssertions;
using MemcacheIt;
using MemcacheIt.Runtime;
using Moq;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.SpecKeyBuilder
{
	public class with_key_builder : specification
	{
		public KeyBuilder keyBuilder;
		public Key key;
		
		protected IKeyGenerator with_key_generator(Func<CacheItem, CommandContext, Key> keyGenerator)
		{
			var mock = new Mock<IKeyGenerator>();
			mock.Setup(keygen => keygen.Generate(It.IsAny<CacheItem>(), It.IsAny<CommandContext>()))
				.Returns(keyGenerator);
			return mock.Object;
		}

		protected IKeyTransformation with_key_transformer(Func<Key, CacheItem, CommandContext, Key> keyTransformer)
		{
			var mock = new Mock<IKeyTransformation>();
			mock.Setup(keytransformation => keytransformation.Transform(It.IsAny<Key>(), It.IsAny<CacheItem>(), It.IsAny<CommandContext>()))
				.Returns(keyTransformer);
			return mock.Object;
		}

		protected void key_builder(IKeyGenerator keyGenerator, params IKeyTransformation[] keyTransformation)
		{
			keyBuilder = new KeyBuilder(keyGenerator, keyTransformation);
		}

	}

	[Scenario.Spec]
	public class when_building_key : with_key_builder
	{
		public when_building_key()
		{
			Given(() => key_builder(
				with_key_generator((item, op) => new Key(item.Key.ToString())),
				with_key_transformer((key, item, op) => key.AddPrefix("usercache")),
				with_key_transformer((key, item, op) => key.AddSuffix("v.10"))));
			When(() => key = keyBuilder.Build(new CacheItem {Key = "simpleKey" }, null));
		}

		[Behavior]
		public void should_generate_key_and_then_transform_it()
		{
			key.ToString().Should().Be("usercache.simpleKey.v.10");
		}
	}

	[Scenario.Spec]
	public class when_building_key_with_no_transformations : with_key_builder
	{
		public when_building_key_with_no_transformations()
		{
			Given(() => key_builder(
			    with_key_generator((item, op) => new Key(item.Key.ToString()))));
			When(() => key = keyBuilder.Build(new CacheItem() { Key = "simpleKey" }, null));
		}

		[Behavior]
		public void should_only_generate_key_and_return_it()
		{
			key.ToString().Should().Be("simpleKey");
		}
	}

	[Scenario.Spec]
	public class when_building_key_and_key_generator_does_not_produce_key : with_key_builder
	{
		public when_building_key_and_key_generator_does_not_produce_key()
		{
			Given(() => key_builder(with_key_generator((item, op) => null)));
			When(() => key = keyBuilder.Build(new CacheItem { Key = "simpleKey" }, null));
			CouldFailWith<CachingException>();
		}

		[Behavior]
		public void should_only_generate_key_and_return_it()
		{
			ShouldFail();
		}
	}

	[Scenario.Spec]
	public class when_building_key_and_key_transformer_does_not_produce_key : with_key_builder
	{
		public when_building_key_and_key_transformer_does_not_produce_key()
		{
			Given(() => key_builder(
				with_key_generator((item, op) => new Key("1")),
				with_key_transformer((key, item, op) => null)));
			When(() => key = keyBuilder.Build(new CacheItem() { Key = "simpleKey" }, null));
			CouldFailWith<CachingException>();
		}

		[Behavior]
		public void should_only_generate_key_and_return_it()
		{
			ShouldFail();
		}
	}
}
