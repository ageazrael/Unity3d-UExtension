using System.Text;
using System.IO;
using System;

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
		public static string Combine(params string[] rPaths) => Combine('/', rPaths);

        public static string GetPathWithoutExtension(string rPath) => Combine(Path.GetDirectoryName(rPath), Path.GetFileNameWithoutExtension(rPath));
        /// <summary>
        /// 复制文件到指定位置
        ///     example:
        ///         Copy("/git/build/Gen/*.cs", "/git/build/tmp", true);
        ///         Copy("/git/build/Gen", "/git/build/tmp", true);
        ///         Copy("/git/build/Gen/asm.cs", "/git/build/tmp/asm.cs", true);
        /// </summary>
        public static void Copy(string source, string dest, bool overwrite)
		{
			if (File.Exists(source))
			{
				var rDirection = Path.GetDirectoryName(dest);
				if (!Directory.Exists(rDirection))
					Directory.CreateDirectory(rDirection);

				File.Copy(source, dest, overwrite);
			}
			else
			{
				var rSearchPath = source;
				var rSearchPattern = "*.*";
				if (!Directory.Exists(source))
				{
					rSearchPath = Path.GetDirectoryName(source);
					rSearchPattern = Path.GetFileName(source);

					if (!Directory.Exists(rSearchPath))
						throw new ArgumentException(string.Format("source:{0} in pair {1} invalid directory", source, rSearchPath));
				}

				foreach (var rFilePath in Directory.GetFiles(rSearchPath, rSearchPattern, SearchOption.AllDirectories))
				{
					Copy(rFilePath, rFilePath.Replace(rSearchPath, dest), overwrite);
				}
			}
		}
	}
}