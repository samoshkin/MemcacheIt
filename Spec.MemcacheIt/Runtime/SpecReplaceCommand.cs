using MemcacheIt.Commands;
using Spec.MemcacheIt.Runtime.SpecSetCommand;

namespace Spec.MemcacheIt.Runtime.SpecReplaceCommand
{
	public class when_replacing_item_in_cache_and_operation_succeeds
		: when_setting_item_in_cache_and_operation_succeeds
	{
		protected override StoreMode GetStoreMode()
		{
			return StoreMode.Replace;
		}
	}

	public class when_replacing_item_in_cache_and_operation_fails
		: when_setting_item_in_cache_and_operation_fails
	{
		protected override StoreMode GetStoreMode()
		{
			return StoreMode.Replace;
		}
	}

}