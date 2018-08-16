using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UExtension;

namespace UnitTest
{
	public class UTHTTPDownload : MonoBehaviour
	{
		[System.Serializable]
		public class DownloadItem
		{
			public string Url;
			public string SavePath;
		}
		public DownloadItem[] Downloader = new DownloadItem[0];

		HTTPDownload download;
		bool 		 downloading;
		void Start ()
		{
		}

		void OnGUI()
		{
			if (GUILayout.Button(this.downloading ? "Downloading..." : "Start Download") && !this.downloading)
			{
				this.downloading = true;
				var rDownloadList = new List<KeyValuePair<string, string>>();
				foreach (var rItem in Downloader)
					rDownloadList.Add(new KeyValuePair<string, string>(rItem.Url, rItem.SavePath));

				this.StartCoroutine(this.HandleDownload(rDownloadList));
			}
		}

		IEnumerator HandleDownload(List<KeyValuePair<string, string>> rDownloadList)
		{
			yield return HTTPDownload.Start(rDownloadList, this.OnDownloadCompleted, this.OnDownloadStateChange);

			Debug.Log("Download Wait Completed");
		}

		void OnDownloadStateChange(string rFilename, HTTPDownload.State rState, int nIndex, int nTotal)
		{
			Debug.LogFormat("Download File:{0} State:{1}", rFilename, rState);
		}
		void OnDownloadCompleted()		
		{
			Debug.Log("Download Completed!!");
			this.downloading = false;
		}
	}
}