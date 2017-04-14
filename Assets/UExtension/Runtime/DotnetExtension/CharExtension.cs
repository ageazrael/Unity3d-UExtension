namespace UExtension
{
	/// <summary>
	/// CharExpand
	/// </summary>
	public static class CharExtension
	{
	    public static bool IsAlpha(this char c)
	    {
	        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
	    }
	    public static bool IsDigit(this char c)
	    {
	        return c >= '0' && c <= '9';
	    }
	}
}