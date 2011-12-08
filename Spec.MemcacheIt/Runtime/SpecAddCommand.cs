using MemcacheIt.Commands;
using Spec.MemcacheIt.Runtime.SpecSetCommand;

namespace Spec.MemcacheIt.Runtime.SpecAddCommand
{
	public class when_adding_item_to_cache_and_operation_succeeds
		: when_setting_item_in_cache_and_operation_succeeds
	{
		protected override StoreMode GetStoreMode()
		{
			return StoreMode.Add;
		}
	}

	public class when_adding_item_to_cache_and_operation_fails
		: when_setting_item_in_cache_and_operation_fails
	{
		protected override StoreMode GetStoreMode()
		{
			return StoreMode.Add;
		}
	}
}