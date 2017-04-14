using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

namespace UExtension
{
    [TSIgnore]
    public class AutoCSGenerate
    {
        public virtual void CSGenerateProcess(CSGenerate rGenerate) { }
    }
    public class AutoCSGenerateTypes : TypeSearchDefault<AutoCSGenerate> { }

    [InitializeOnLoad]
    public class AutoCSGenerateMain
    {
        static AutoCSGenerateMain()
        {
            var rCSGenerate = new CSGenerate();
            for (int nIndex = 0; nIndex < AutoCSGenerateTypes.Types.Count; ++ nIndex)
            {
                var rType = AutoCSGenerateTypes.Types[nIndex];
                EditorUtility.DisplayProgressBar("AutoCSGenerate...", 
                    string.Format("Process {0}...", rType.FullName), (float)nIndex / (float)AutoCSGenerateTypes.Types.Count);

                var rProcessInstance = ReflectExtension.TConstruct<AutoCSGenerate>(rType);
                if (null == rProcessInstance)
                {
                    Debug.LogErrorFormat("AutoCSGenerateMain => {0} not use. need default construct", rType.FullName);
                    continue;
                }

                try
                {
                    rProcessInstance.CSGenerateProcess(rCSGenerate);
                }
                catch (System.Exception rException)
                {
                    Debug.LogException(rException);
                }
            }
            EditorUtility.ClearProgressBar();

            rCSGenerate.UpdateCSFile( (rText, fRate) => {
                EditorUtility.DisplayProgressBar("AutoCSGenerate", rText, fRate);
            });
            EditorUtility.ClearProgressBar();
        }
    }

    public class CSGenerate
    {
        class CSInfo
        {
            public string Text;
            public string SavePath;
            public string MD5;
        }
        const string CSMD5Path = "Library/CSGenerate/";

        public void Add(string rText, string rSavePath)
        {
            mCSInfo.Add(new CSInfo() { Text = rText, SavePath = rSavePath });
        }
        public void AddHead(string rText, string rSavePath)
        {
            mCSInfo.Insert(0, new CSInfo() { Text = rText, SavePath = rSavePath });
        }
        public void Clear()
        {
            mCSInfo.Clear();
        }
        public event System.Action<List<string>> UpdateCSFileCompleted;

        public List<string> AllGenerateFiles
        {
            get
            {
                var rFiles = new List<string>();
                foreach(var rCSInfo in mCSInfo)
                    rFiles.Add(rCSInfo.SavePath);
                return rFiles;
            }
        }
        public void UpdateCSFile(System.Action<string, float> rFeedback)
        {
            if (!Directory.Exists(CSMD5Path))
                Directory.CreateDirectory(CSMD5Path);

            var rChangeFiles = new List<string>();
            for (int nIndex = 0; nIndex < mCSInfo.Count; ++nIndex)
            {
                var rCSInfo = mCSInfo[nIndex];

                if (null != rFeedback)
                    rFeedback(string.Format("Update {0}", rCSInfo.SavePath), (float)(nIndex + 1)/(float)mCSInfo.Count);

                rCSInfo.MD5 = MD5Generate.ByString(rCSInfo.Text);

                if (!File.Exists(rCSInfo.SavePath + ".md5") ||
                    !File.Exists(rCSInfo.SavePath) ||
                    File.ReadAllText(rCSInfo.SavePath + ".md5") != rCSInfo.MD5)
                {
                    WriteFile(rCSInfo.Text, rCSInfo.SavePath);
                    rChangeFiles.Add(rCSInfo.SavePath);
                }
            }
            for (int nIndex = 0; nIndex < mCSInfo.Count; ++nIndex)
            {
                var rCSInfo = mCSInfo[nIndex];

                WriteFile(rCSInfo.MD5, rCSInfo.SavePath + ".md5");
            }

            if (null != this.UpdateCSFileCompleted)
                this.UpdateCSFileCompleted(rChangeFiles);
        }
        void WriteFile(string rText, string rSavePath)
        {
            var rPath = Path.GetDirectoryName(rSavePath);
            if (!Directory.Exists(rPath))
                Directory.CreateDirectory(rPath);

            File.WriteAllText(rSavePath, rText);
        }

        List<CSInfo> mCSInfo = new List<CSInfo>();
    }
}
