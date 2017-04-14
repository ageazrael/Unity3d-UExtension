namespace UExtension
{
	/// <summary>
	/// StringExpand
	/// </summary>
	public static class StringExtension
	{
	    public static bool IsAlpha(this string str)
	    {
	        if (string.IsNullOrEmpty(str))
	            return false;
	
	        int nCount = str.Length;
	        for (int nIndex = 0; nIndex < nCount; ++nIndex)
	        {
	            if (!str[nIndex].IsAlpha())
	                return false;
	        }
	
	        return true;
	    }
	    public static bool IsInterger(this string str)
	    {
	        if (string.IsNullOrEmpty(str))
	            return false;
	
	        int nCount = str.Length;
	        for (int nIndex = 0; nIndex < nCount; ++nIndex)
	        {
	            if (!str[nIndex].IsDigit())
	                return false;
	        }
	
	        return true;
	    }
	}
}