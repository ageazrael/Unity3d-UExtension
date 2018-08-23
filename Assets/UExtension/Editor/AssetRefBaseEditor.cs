using UnityEngine;
using UnityEditor;
using System;

namespace UExtension
{
	using Object = UnityEngine.Object;
	
	[CustomPropertyDrawer(typeof(AssetRefBase), true)]
	public class AssetRefBaseEditor : PropertyDrawer
	{
		public override void OnGUI(Rect rTotalPosition, SerializedProperty rProperty, GUIContent rLabel)
		{
			EditorGUI.BeginProperty(rTotalPosition, rLabel, rProperty);


			var rAssetTypeFullName = rProperty.FindPropertyRelative("AssetTypeFullName");
			var rAssetPath         = rProperty.FindPropertyRelative("AssetPath");
			var rAssetGUID         = rProperty.FindPropertyRelative("AssetGUID");
			var rLoadedAsset       = rProperty.FindPropertyRelative("LoadedAssetInstanceID");

			var bNeedFix = false;
			var rResourcesAssetPath = rAssetPath.stringValue;
			if (!string.IsNullOrEmpty(rAssetGUID.stringValue))
			{
				rResourcesAssetPath = AssetPathToResourcePath(AssetDatabase.GUIDToAssetPath(rAssetGUID.stringValue));
				if (rResourcesAssetPath != rAssetPath.stringValue)
					bNeedFix = true;
			}

			var rAssetType = Type.GetType(rAssetTypeFullName.stringValue);
			if (null == rAssetType)
				rAssetType = typeof(Object);

			EditorGUI.BeginChangeCheck();

			Object rPreloadAsset = null;
			if (!string.IsNullOrEmpty(rResourcesAssetPath))
			{
				if (!Application.isPlaying)
					rPreloadAsset = Resources.Load(rResourcesAssetPath, rAssetType);
				else
					rPreloadAsset = EditorUtility.InstanceIDToObject(rLoadedAsset.intValue);
			}

			if (bNeedFix && !Application.isPlaying)
                rTotalPosition.xMax -= 24;

			var rNewObject = EditorGUI.ObjectField(rTotalPosition, rLabel.text, rPreloadAsset, rAssetType, false);
			if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
			{
				if (rNewObject)
				{
					var rPath = AssetDatabase.GetAssetPath(rNewObject);
					if (rPath.Contains("Resources"))
					{
						rAssetPath.stringValue = AssetPathToResourcePath(rPath);
						rAssetGUID.stringValue = AssetDatabase.AssetPathToGUID(rPath);
					}
				}
				else
				{
					rAssetPath.stringValue = string.Empty;
					rAssetGUID.stringValue = string.Empty;
				}
			}

			if (bNeedFix && !Application.isPlaying)
			{
				var rFixButtonRect = rTotalPosition;
				rFixButtonRect.xMin = rFixButtonRect.xMax;
				rFixButtonRect.xMax = rFixButtonRect.xMax + 24;
				if (GUI.Button(rFixButtonRect, "F"))
				{
					if (string.IsNullOrEmpty(rAssetGUID.stringValue))
						rAssetPath.stringValue = string.Empty;
					else
						rAssetPath.stringValue = AssetPathToResourcePath(AssetDatabase.GUIDToAssetPath(rAssetGUID.stringValue));

					if (string.IsNullOrEmpty(rAssetPath.stringValue))
						rAssetGUID.stringValue = string.Empty;
				}
			}

			EditorGUI.EndProperty();
		}

		public static string AssetPathToResourcePath(string rAssetPath)
		{
			var nLastIndex = rAssetPath.LastIndexOf("Resources");
			if(-1 != nLastIndex)
			{
				return PathExtension.GetPathWithoutExtension(
					rAssetPath.Substring(nLastIndex + "Resources".Length + 1));
			}
			return rAssetPath;
		}
		public static bool FixAssetPath(AssetRefBase rAssetRef)
		{
			if (null == rAssetRef)
				return false;
			
			var rNewAssetPath = AssetRefBaseEditor.AssetPathToResourcePath (
				AssetDatabase.GUIDToAssetPath (rAssetRef.AssetGUID));
			if (rNewAssetPath == rAssetRef.AssetPath)
				return false;

			rAssetRef.AssetPath = rNewAssetPath;
			return true;
		}
	}

}
