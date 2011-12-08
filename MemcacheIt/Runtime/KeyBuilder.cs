using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;

namespace MemcacheIt.Runtime
{
	public class KeyBuilder
	{
		private readonly IKeyGenerator _keyGenerator;
		private readonly IList<IKeyTransformation> _keyTransformations;

		public KeyBuilder(
			IKeyGenerator keyGenerator,
			IEnumerable<IKeyTransformation> keyTransformations)
		{
			_keyGenerator = keyGenerator;
			_keyTransformations = keyTransformations.ToList();
		}

		public IKeyGenerator GeneratedBy { get { return _keyGenerator; }}

		public IEnumerable<IKeyTransformation> TransformedBy { get { return _keyTransformations; }}

		public virtual Key Build(CacheItem item, CommandContext commandContext)
		{
			var key = _keyGenerator.Generate(item, commandContext);
			Condition.WithExceptionOnFailure<CachingException>()
				.Requires(key, "generated key")
				.IsNotNull("Key generator failed to produce key.");

			foreach (var keyTransformation in TransformedBy)
			{
				key = keyTransformation.Transform(key, item, commandContext);
				Condition.WithExceptionOnFailure<CachingException>()
					.Requires(key, "transformed key")
					.IsNotNull("Key transformation failed to transform key.");
			}
				
			return key;
		}
	}
}