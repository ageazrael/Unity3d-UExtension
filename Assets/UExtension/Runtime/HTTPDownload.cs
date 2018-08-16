using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace UExtension
{
	/// <summary>
	/// HTTP download.
	/// 	TODO: 大文件对内存的消耗优化
	/// </summary>
	public class HTTPDownload
	{
		public enum State
		{
			Downloading,
			Completed,

			Error,
		}
		public delegate void DelegateDownloadState(string rFilename, State rState, int nIndex, int nTotal);
		public delegate void DelegateDownloadCompleted();

		public static Coroutine Start(List<KeyValuePair<string, string>> rDownloadList,
            DelegateDownloadCompleted rDownloadCompleted = null,
            DelegateDownloadState rDownloadState = null) => CoroutineManager.Start(HandleStart(rDownloadList, rDownloadCompleted, rDownloadState));

        public static IEnumerator HandleStart(List<KeyValuePair<string, string>> rDownloadList,
            DelegateDownloadCompleted rDownloadCompleted = null,
            DelegateDownloadState rDownloadState = null)
		{
			if (null == rDownloadList)
            {
                Debug.LogErrorFormat("HTTPDownload: DownloadList is null!!!");
                rDownloadCompleted?.Invoke();
                yield break;
            }

			for (int nIndex = 0; nIndex < rDownloadList.Count; ++ nIndex)
			{
				var rPair = rDownloadList[nIndex];

                rDownloadState?.Invoke(rPair.Value, State.Downloading, nIndex + 1, rDownloadList.Count);
                using (var rWebRequest = new UnityWebRequest(rPair.Key, 
					UnityWebRequest.kHttpVerbGET, new DownloadHandlerBuffer(), null))
				{
					yield return rWebRequest.SendWebRequest();
					try
					{
                        if (rWebRequest.isNetworkError || !string.IsNullOrEmpty(rWebRequest.error))
						{
							Debug.LogErrorFormat("HTTPDownload: Download {0} save {1} errror => {2}",
								rPair.Key, rPair.Value, rWebRequest.error);
                            rDownloadState?.Invoke(rPair.Value, State.Error, nIndex + 1, rDownloadList.Count);
                        }
						else
						{
							var rDirectoryName = Path.GetDirectoryName(rPair.Value);
							if (!string.IsNullOrEmpty(rDirectoryName) && !Directory.Exists(rDirectoryName))
								Directory.CreateDirectory(rDirectoryName);
							
							File.WriteAllBytes(rPair.Value, rWebRequest.downloadHandler.data);

                            rDownloadState?.Invoke(rPair.Value, State.Completed, nIndex + 1, rDownloadList.Count);
                        }
					}
					catch (Exception rException)
					{
						Debug.LogException(rException);
						Debug.LogErrorFormat("HTTPDownload: Download {0} save {1} errror => {2}",
							rPair.Key, rPair.Value, rException.Message);
                        rDownloadState?.Invoke(rPair.Value, State.Error, nIndex + 1, rDownloadList.Count);
                    }
				}
			}

            rDownloadCompleted?.Invoke();
        }
	}
}