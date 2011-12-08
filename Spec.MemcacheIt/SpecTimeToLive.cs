using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using MemcacheIt;
using SimpleSpec.NUnit;

namespace Spec.MemcacheIt.SpecTimeToLive
{
	[Scenario.Spec]
	public class when_time_to_live_is_set_as_never_expiring
		: specification
	{
		TimeToLive timeToLive;

		public when_time_to_live_is_set_as_never_expiring()
		{
			Given(() => timeToLive = TimeToLive.CreateNeverExpiring());
		}


		[Behavior]
		public void query_to_never_expires_property_should_return_true()
		{
			timeToLive.NeverExpires.Should().BeTrue();
		}

		[Behavior]
		public void should_indicate_it_does_not_expires_at_absolute_time()
		{
			timeToLive.ExpiresAtAbsoluteTime.Should().BeFalse();
		}

		[Behavior]
		public void should_indicated_it_does_not_expires_at_relative_time()
		{
			timeToLive.ExpiresAtRelativeTime.Should().BeFalse();
		}

		[Behavior]
		public void should_fail_to_get_valid_for_value()
		{
			timeToLive.Invoking(ttl => { var validFor = ttl.ValidFor; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("instance is not configured to expire in a specific period of time", ComparisonMode.Substring);
		}

		[Behavior]
		public void should_fail_to_get_expires_at_value()
		{
			timeToLive.Invoking(ttl => { var validFor = ttl.ExpiresAt; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("instance is not configured to expire at a specific moment in future", ComparisonMode.Substring);
		}

		[Behavior]
		public void should_equal_to_another_never_expiring_instance_of_time_to_live()
		{
			timeToLive.Should().Be(TimeToLive.CreateNeverExpiring());
		}

		[Behavior]
		public void should_not_equal_to_time_to_live_which_expire_at_absolute_time_or_relative()
		{
			timeToLive.Should().NotBe(TimeToLive.CreateExpiringAt(new DateTime(2011, 1, 1)));
			timeToLive.Should().NotBe(TimeToLive.CreateValidFor(TimeSpan.FromSeconds(2)));
		}
	}

	[Scenario.Spec]
	public class when_time_to_live_is_set_to_expire_at_specific_moment
		: specification
	{
		TimeToLive timeToLive;
		DateTime expiringAt = new DateTime(2011, 12, 04);

		public when_time_to_live_is_set_to_expire_at_specific_moment()
		{
			Given(() => timeToLive = TimeToLive.CreateExpiringAt(expiringAt));
		}

		[Behavior]
		public void should_indicate_is_not_never_expiring()
		{
			timeToLive.NeverExpires.Should().BeFalse();
		}

		[Behavior]
		public void should_indicate_it_does_expire_at_absolute_time()
		{
			timeToLive.ExpiresAtAbsoluteTime.Should().BeTrue();
		}

		[Behavior]
		public void should_indicated_it_does_not_expires_at_relative_time()
		{
			timeToLive.ExpiresAtRelativeTime.Should().BeFalse();
		}

		[Behavior]
		public void should_fail_to_get_valid_for_value()
		{
			timeToLive.Invoking(ttl => { var validFor = ttl.ValidFor; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("instance is not configured to expire in a specific period of time", ComparisonMode.Substring);
		}

		[Behavior]
		public void should_fail_to_get_expires_at_value()
		{
			timeToLive.ExpiresAt.Should().Be(new DateTime(2011, 12, 04));
		}

		[Behavior]
		public void should_equal_to_another_instance_of_time_to_live_which_expires_at_the_same_moment()
		{
			timeToLive.Should().Be(TimeToLive.CreateExpiringAt(expiringAt));
			timeToLive.Should().NotBe(TimeToLive.CreateExpiringAt(expiringAt.AddDays(1)));
		}

		[Behavior]
		public void should_not_equal_to_time_to_live_which_never_expiring_or_expire_at_relative_time()
		{
			timeToLive.Should().NotBe(TimeToLive.CreateNeverExpiring());
			timeToLive.Should().NotBe(TimeToLive.CreateValidFor(TimeSpan.FromSeconds(2)));
		}
	}

	[Scenario.Spec]
	public class when_time_to_live_is_set_to_be_valid_for_a_specific_period_of_time
		: specification
	{
		TimeToLive timeToLive;
		TimeSpan validFor = TimeSpan.FromMinutes(2);

		public when_time_to_live_is_set_to_be_valid_for_a_specific_period_of_time()
		{
			Given(() => timeToLive = TimeToLive.CreateValidFor(validFor));
		}

		[Behavior]
		public void should_indicate_is_not_never_expiring()
		{
			timeToLive.NeverExpires.Should().BeFalse();
		}

		[Behavior]
		public void should_indicate_it_does_not_expire_at_absolute_time()
		{
			timeToLive.ExpiresAtAbsoluteTime.Should().BeFalse();
		}

		[Behavior]
		public void should_indicated_it_does_expires_at_relative_time()
		{
			timeToLive.ExpiresAtRelativeTime.Should().BeTrue();
		}

		[Behavior]
		public void should_fail_to_get_valid_for_value()
		{
			timeToLive.ValidFor.Should().Be(TimeSpan.FromMinutes(2));
		}

		[Behavior]
		public void should_fail_to_get_expires_at_value()
		{
			timeToLive.Invoking(ttl => { var validFor = ttl.ExpiresAt; })
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("instance is not configured to expire at a specific moment in future", ComparisonMode.Substring);
		}

		[Behavior]
		public void should_equal_to_another_instance_of_time_to_live_which_valid_for_the_same_period_of_time()
		{
			timeToLive.Should().Be(TimeToLive.CreateValidFor(validFor));
			timeToLive.Should().NotBe(TimeToLive.CreateValidFor(validFor.Add(TimeSpan.FromMinutes(1))));
		}

		[Behavior]
		public void should_not_equal_to_time_to_live_which_never_expiring_or_expire_at_absolute_time()
		{
			timeToLive.Should().NotBe(TimeToLive.CreateNeverExpiring());
			timeToLive.Should().NotBe(TimeToLive.CreateExpiringAt(new DateTime(2011, 1, 1)));
		}
	}
}
