using System;

namespace MemcacheIt.Runtime
{
	public interface  IItemBuilderSyntax
	{
		IItemBuilderSyntax ValueOf<T>(T value);
		IItemBuilderSyntax ValueOf(object value);
		IItemBuilderSyntax OfType(Type type);
		IItemBuilderSyntax OfType<T>();
		IItemBuilderSyntax InCache(ICacheScope cacheScope);
		IItemBuilderSyntax KeyIs(object key);
		IItemBuilderSyntax KeyIsGeneratedBy(object keyStrategy); // TODO
		IItemBuilderSyntax VersionIs(object version);
		IItemBuilderSyntax VersionIsCalculatedBy(object versionGenerationStrategy); // TODO
		IItemBuilderSyntax WithProperty(object customProperty); // TODO
	}
}