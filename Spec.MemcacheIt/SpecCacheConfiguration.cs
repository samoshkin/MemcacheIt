using SimpleSpec.NUnit;

namespace Spec.MemcacheIt
{
	[Behavior.Spec]
	public class when_configuring_memcached_client_api_from_config_file
	{
		public when_configuring_memcached_client_api_from_config_file()
		{}

		[Behavior]
		public void should_read_settings_from_default_enyimmemcache_section_of_config_file()
		{
			
		}

		[Behavior]
		public void should_allow_to_override_some_settings_from_config_file_after_read()
		{

		}
	}

	[Scenario.Spec]
	public class when_configuring_memcached_client_api_with_defaults
	{
		[Behavior]
		public void server_list_should_be_empty()
		{
			
		}

		[Behavior]
		public void should_use_binary_protocol()
		{
			
		}

		[Behavior]
		public void other_configuration_options_should_be_configured_with_memcache_defaults()
		{
			
		}

		[Behavior]
		public void performance_monitor_should_not_be_specified()
		{
			
		}
	}
}
