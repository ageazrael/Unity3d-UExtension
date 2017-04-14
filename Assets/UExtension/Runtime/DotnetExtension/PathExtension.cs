using System.Text;
using System.IO;

namespace UExtension
{
	public class PathExtension 
	{
		public static string Combine(char rDirectoryChar, params string[] rPaths)
		{
			var rReplaceChar = rDirectoryChar == '\\' ? '/' : '\\';
			if (rPaths.Length == 0)
				return string.Empty;
	
            var nFirstIndex = 0;
            for (int nIndex = 0; nIndex < rPaths.Length; ++ nIndex)
            {
                if (!string.IsNullOrEmpty(rPaths[nIndex]))
                {
                    nFirstIndex = nIndex;
                    break;
                }
            }
            var rFirstPath = rPaths[nFirstIndex].Replace(rReplaceChar, rDirectoryChar);
			if (rFirstPath[rFirstPath.Length - 1] == rDirectoryChar)
				rFirstPath = rFirstPath.Substring(0, rFirstPath.Length - 1);

			var rBuilder = new StringBuilder(rFirstPath);
			for (int nIndex = nFirstIndex + 1; nIndex < rPaths.Length; ++ nIndex)
			{
                if (string.IsNullOrEmpty(rPaths[nIndex]))
                    continue;

				var rPath = rPaths[nIndex].Replace(rReplaceChar, rDirectoryChar);
				if (rPath[0] != rDirectoryChar)
					rPath = rDirectoryChar + rPath;
				if (rPath[rPath.Length - 1] == rDirectoryChar)
					rPath = rPath.Substring(0, rPath.Length - 1);
				rBuilder.Append(rPath);
			}
	
			return rBuilder.ToString();
		}
		public static string Combine(params string[] rPaths)
		{
			return Combine('/', rPaths);
		}

        public static string GetPathWithoutExtension(string rPath)
        {
            return Combine(Path.GetDirectoryName(rPath), Path.GetFileNameWithoutExtension(rPath));
        }


	}
}