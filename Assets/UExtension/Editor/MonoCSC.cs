using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;
using System.Text;

namespace UExtension
{

    public class MonoCSC
    {
        public enum TargetType
        {
            Library,
            Execute,
        }
        public enum UnityAssembly
        {
            UnityEngine,
            UnityEditor,
        }
        public enum SDKVersionType
        {
            DotNet2,
            DotNet4,
            DotNet4_5
        }
        
        public MonoCSC(TargetType rType, string rOutputFile)
        {
#if UNITY_EDITOR_OSX
            this.Command          = "/bin/sh";
            this.Arguments        = string.Format("{0}/MonoBleedingEdge/bin/mcs", EditorApplication.applicationContentsPath);
#else
            this.Command          = "C:/Windows/System32/cmd.exe";
            this.Arguments        = string.Format("/c {0}/MonoBleedingEdge/bin/mcs.bat", EditorApplication.applicationContentsPath);
#endif

            this.BuildTargetType  = rType;
            this.OutputFile       = rOutputFile;
            this.WorkingDirectory = Application.dataPath + "/../";
        }

        public int Execute()
        {
            var rProcessStartInfo = new ProcessStartInfo();

            rProcessStartInfo.FileName = this.Command;
            rProcessStartInfo.Arguments = this.BuildArguments();
            rProcessStartInfo.WorkingDirectory = this.WorkingDirectory;
            
            var rProcess = Process.Start(rProcessStartInfo);

            rProcess.OutputDataReceived += (sender, e) => {
                UnityEngine.Debug.LogError(e.Data);
            };
            rProcess.Start();
            rProcess.WaitForExit();
            
            return 0;
        }
        public SDKVersionType SDKVersion = SDKVersionType.DotNet2;
        public TargetType     BuildTargetType;

        public void AddCompileFile(string rFile)
        {
            this.CompileFiles.Add(rFile);
        }
        public void AddReference(string rLibrary)
        {
            this.ReferenceLibrarys.Add(rLibrary);
        }
        public void AddReference(UnityAssembly rAssemblyType)
        {
            switch (rAssemblyType)
            {
            case UnityAssembly.UnityEngine:
                this.AddReference(string.Format("{0}/Managed/UnityEngine.dll", EditorApplication.applicationContentsPath));
                break;
            case UnityAssembly.UnityEditor:
                this.AddReference(string.Format("{0}/Managed/UnityEditor.dll", EditorApplication.applicationContentsPath));
                break;
            }
        }
        public string BuildArguments()
        {
            var rArgumentsText = new StringBuilder();
            rArgumentsText.Append(this.Arguments);

            rArgumentsText.AppendFormat(" -out:{0}", this.OutputFile);
            rArgumentsText.AppendFormat(" -target:{0}", this.BuildTargetType == TargetType.Library ? "library" : "exe");
            rArgumentsText.AppendFormat(" -sdk:{0}", this.GetSDKVersionText());

            foreach (var rReferenceLib in this.ReferenceLibrarys)
                rArgumentsText.AppendFormat(" -r:{0}", rReferenceLib);

            foreach (var rCompleFile in this.CompileFiles)
                rArgumentsText.AppendFormat(" {0}", rCompleFile);

            return rArgumentsText.ToString();
        }

        protected string GetSDKVersionText()
        {
            switch (this.SDKVersion)
            {
            case SDKVersionType.DotNet2:
                return "2";

            case SDKVersionType.DotNet4:
                return "4";

            case SDKVersionType.DotNet4_5:
                return "4.5";

            default:
                return "4.5";
            }
        }

        protected string        Command;
        protected string        Arguments;
        protected string        WorkingDirectory;
        protected string        OutputFile;
        protected List<string>  ReferenceLibrarys = new List<string>();
        protected List<string>  CompileFiles      = new List<string>();
    }

}