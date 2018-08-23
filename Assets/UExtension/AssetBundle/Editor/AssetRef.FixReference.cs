using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UExtension
{
	/// <summary>
	/// AssetRefFixReference.
	/// </summary>
	public static class AssetRefFixReference
	{
		public static System.Type AssetRefBaseType = typeof(AssetRefBase);

		public static bool ObjectFixAssetPath(System.Object rObject)
		{
			if (null == rObject)
				return false;

			if (!rObject.GetType().IsClass)
				return false;
			
			if (!typeof(UnityEngine.Object).IsAssignableFrom(rObject.GetType()) &&
			    !rObject.GetType().IsDefined(typeof(System.SerializableAttribute), false))
				return false;

            var bFix = false;
			foreach (var rFieldInfo in rObject.GetType().GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public))
			{
				if (AssetRefBaseType.IsAssignableFrom(rFieldInfo.FieldType))
				{
					var rValue = rFieldInfo.GetValue(rObject);
					if (null != rValue)
                        bFix = AssetRefBaseEditor.FixAssetPath(rValue as AssetRefBase) ? true : bFix;
				}
				else if (rFieldInfo.FieldType.IsArray || rFieldInfo.FieldType.GetInterface("System.Collections.IList") != null)
				{
					var rListObject = rFieldInfo.GetValue(rObject);
					if (null != rListObject)
					{
						foreach (var rNextObject in (IList)rListObject)
						{
							if (null == rNextObject)
								continue;

							if (AssetRefBaseType.IsAssignableFrom(rNextObject.GetType ()))
								bFix = AssetRefBaseEditor.FixAssetPath(rNextObject as AssetRefBase) ? true : bFix;
							else
								bFix = ObjectFixAssetPath(rNextObject) ? true : bFix;
						}
					}
				}
				else if (rFieldInfo.FieldType.IsClass)
				{
					bFix = ObjectFixAssetPath(rFieldInfo.GetValue (rObject)) ? true : bFix;
				}
			}
            return bFix;
		}

		[MenuItem("Tools/UExtension/FixReference")]
		public static void FixReference()
		{
			foreach (var rAssetGUID in AssetDatabase.FindAssets("t:GameObject"))
			{
				var rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
				if (string.IsNullOrEmpty(rAssetPath))
					continue;

				var rAssetGO = AssetDatabase.LoadAssetAtPath<GameObject>(rAssetPath);
				if (rAssetGO)
				{
                    foreach (var rComponent in rAssetGO.GetComponentsInChildren<MonoBehaviour>(true))
                    {
                        if (ObjectFixAssetPath(rComponent))
                            EditorUtility.SetDirty(rAssetGO);
                    }
				}
			}
			foreach (var rAssetGUID in AssetDatabase.FindAssets("t:ScriptableObject"))
			{
				var rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
				if (string.IsNullOrEmpty(rAssetPath))
					continue;

				var rScriptable = AssetDatabase.LoadAssetAtPath<ScriptableObject>(rAssetPath);
                if (rScriptable)
                {
                    if (ObjectFixAssetPath(rScriptable))
                        EditorUtility.SetDirty(rScriptable);
                    else
                        Resources.UnloadAsset(rScriptable);
                }
			}
		}
	}

}