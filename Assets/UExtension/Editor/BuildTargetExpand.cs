using UnityEngine;
using System.Collections;

namespace UExtension
{
    using UnityEditor;

    public static class BuildTargetExpand
    {
        public static string GetFixedName(this BuildTarget rBuildTarget)
        {
            switch (rBuildTarget)
            {
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "Standalone";

#if UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0
                case BuildTarget.WebPlayer:
                case BuildTarget.WebPlayerStreamed:
                    return "WebPlayer";
#endif

                case BuildTarget.iOS:
                    return "iOS";

                case BuildTarget.Android:
                    return "Android";

                default:
                    return rBuildTarget.ToString();
            }
        }
    }
}