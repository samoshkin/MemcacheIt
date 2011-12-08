using System;
using CuttingEdge.Conditions;

namespace MemcacheIt
{
	public class TimeToLive
	{
		private TimeSpan _validFor;
		private DateTime _expiresAt;
		private bool _neverExpires;
		private bool _isAbsoluteTime;

		private TimeToLive()
		{}

		public static TimeToLive CreateValidFor(TimeSpan value)
		{
			return new TimeToLive
			{
				_neverExpires = false,
				_isAbsoluteTime = false,
				_validFor = value
			};
		}

		public static TimeToLive CreateExpiringAt(DateTime value)
		{
			return new TimeToLive
			{
				_neverExpires = false,
				_isAbsoluteTime = true,
				_expiresAt = value,
			};
		}

		public static TimeToLive CreateNeverExpiring()
		{
			return new TimeToLive
			{
				_neverExpires = true,
				_isAbsoluteTime = false,
			};
		}

		public bool NeverExpires { get { return _neverExpires; } }
		
		public bool ExpiresAtAbsoluteTime
		{
			get { return !_neverExpires && _isAbsoluteTime; }
		}

		public bool ExpiresAtRelativeTime
		{
			get { return !_neverExpires && !_isAbsoluteTime; }
		}
		
		public TimeSpan ValidFor
		{
			get
			{
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					.Requires(ExpiresAtRelativeTime).IsTrue(
					"'{0}' instance is not configured to expire in a specific period of time.".FormatString(this));
				return _validFor;
			}
		}

		public DateTime ExpiresAt
		{
			get
			{
				Condition.WithExceptionOnFailure<InvalidOperationException>()
					.Requires(ExpiresAtAbsoluteTime).IsTrue(
					"'{0}' instance is not configured to expire at a specific moment in future.".FormatString(this));
				return _expiresAt;
			}
		}

		public override string ToString()
		{
			return NeverExpires
				? "Never expiring"
				: ExpiresAtAbsoluteTime
					? "at " + _expiresAt
					: "in " + _validFor;
		}

		public bool Equals(TimeToLive other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._validFor.Equals(_validFor) 
				&& other._expiresAt.Equals(_expiresAt) 
				&& other._neverExpires.Equals(_neverExpires) 
				&& other._isAbsoluteTime.Equals(_isAbsoluteTime);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TimeToLive);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = _validFor.GetHashCode();
				result = (result*397) ^ _expiresAt.GetHashCode();
				result = (result*397) ^ _neverExpires.GetHashCode();
				result = (result*397) ^ _isAbsoluteTime.GetHashCode();
				return result;
			}
		}
	}
}