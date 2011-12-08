namespace MemcacheIt
{
	public enum CasResult
	{
		None = 0,

		Stored = 1,
		NotStored,
		Exists,
		NotFound
	}
}