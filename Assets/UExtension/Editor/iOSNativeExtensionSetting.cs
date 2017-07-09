using UnityEngine;
using System;

namespace UExtension
{

	/// <summary>
	/// 这个货用来解决什么呢，
	///     某些时候我们会引用一些外部的framework，可是可恶啊某些framework包含了js。放到Assets目录中会导致无法编译
	///     PluginImport设置面板中，可不包括dylib库和,tbd库依赖设置哦
	/// so 这个东西来对当前PluginImport不足的地方进行补充。
	/// </summary>
	[CreateAssetMenu()]
    public class iOSNativeExtensionSetting : ScriptableObject
    {
        public enum FrameworkStatus
        {
            Required,
            Optional
        }
        
        [Serializable]
        public class Framework
        {
            [Folder(PathType.ProjectPath, ".framework", Editable = true)]
            public string           Path;
            public FrameworkStatus  Status;

            public bool             IsWeak { 
                get { return this.Status == FrameworkStatus.Optional; }
            }
        }
        [Serializable]
        public class Bundle
        {
            [Folder(PathType.ProjectPath, ".bundle", Editable = true)]
            public string           Path;
        }
		[Serializable]
		public class Library
		{
            [FilePath(PathType.ProjectPath, "Static Library", "a,dylib", Editable = true)]
			public string File;
		}
        [Serializable]
        public class CompileFile
        {
            [FilePath(PathType.ProjectPath, "Source Files", "h,m,mm,cpp,c", Editable = true)]
            public string           File;
            [Folder(PathType.ProjectPath, Editable = true)]
            public string           HeadersDirectory;
            public string           CompileFlags;
        }

        public bool             Enabled;
        public Framework[]      Frameworks;
        public Bundle[]         Bundles;
        public Library[]        Librarys;
        public CompileFile[]    CompileFiles;
        public Framework[]      DependentFrameworks;
        public Library[]        DependentLibrarys;
        public string           OtherLinkerFlags;
    }
}