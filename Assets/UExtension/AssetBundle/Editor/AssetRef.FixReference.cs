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

		public static void ObjectFixAssetPath(System.Object rObject)
		{
			if (null == rObject)
				return;

			if (!rObject.GetType().IsClass)
				return;
			
			if (!typeof(UnityEngine.Object).IsAssignableFrom(rObject.GetType()) &&
			    !rObject.GetType().IsDefined(typeof(System.SerializableAttribute), false))
				return;
			
			foreach (var rFieldInfo in rObject.GetType().GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public))
			{
				if (AssetRefBaseType.IsAssignableFrom(rFieldInfo.FieldType))
				{
					var rValue = rFieldInfo.GetValue(rObject);
					if (null != rValue)
						AssetRefBaseEditor.FixAssetPath(rValue as AssetRefBase);
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
								AssetRefBaseEditor.FixAssetPath(rNextObject as AssetRefBase);
							else
								ObjectFixAssetPath(rNextObject);
						}
					}
				}
				else if (rFieldInfo.FieldType.IsClass)
				{
					ObjectFixAssetPath(rFieldInfo.GetValue (rObject));
				}
			}
		}

		[MenuItem("Tools/UExtension/FixReference")]
		public static void FixReference()
		{
			var rAssetGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(GameObject).Name));
			foreach (var rAssetGUID in rAssetGUIDs)
			{
				var rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
				if (string.IsNullOrEmpty(rAssetPath))
					continue;

				var rAssetGO = AssetDatabase.LoadAssetAtPath<GameObject>(rAssetPath);
				if (rAssetGO)
				{
					foreach(var rComponent in rAssetGO.GetComponentsInChildren<MonoBehaviour>(true))
						ObjectFixAssetPath(rComponent);
				}
			}
			rAssetGUIDs = AssetDatabase.FindAssets(string.Format ("t:{0}", typeof(ScriptableObject).Name));
			foreach (var rAssetGUID in rAssetGUIDs)
			{
				var rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
				if (string.IsNullOrEmpty(rAssetPath))
					continue;

				var rScriptable = AssetDatabase.LoadAssetAtPath<ScriptableObject>(rAssetPath);
				if (rScriptable)
					ObjectFixAssetPath(rScriptable);

				Resources.UnloadAsset(rScriptable);
			}
		}
	}

}