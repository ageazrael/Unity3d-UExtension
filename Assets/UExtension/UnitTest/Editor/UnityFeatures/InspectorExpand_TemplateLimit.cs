using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace UnitTest.UnityFeatures
{
    [System.Serializable]
    public class InspectorDataBase
    {

    }
    [System.Serializable]
    public class InspectorData<T> : InspectorDataBase
    {
        public T Value;
    }
    [System.Serializable]
    public class InspectorDataInt : InspectorData<int>
    {

    }
    [CustomPropertyDrawer(typeof(InspectorDataBase), true)]
    public class InspectorDataEditor : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            using(var scope = new EditorGUILayout.HorizontalScope())
            {
                var rBRect = EditorGUI.PrefixLabel(pos, label);

                GUI.Button(rBRect, "啊哈哈");
            }
        }
    }
    public class InspectorObject : ScriptableObject
    {
        [SerializeField]
        public InspectorData<int> xxx;
        public InspectorDataInt ba;
        public InspectorDataBase bb;
        public int value;
    }

    public class InspectorExpand_TemplateLimit
    {
        [Test]
        public void Mian()
        {
            // ::始终无法直接显示模板定义的数据
            var rAsset = InspectorObject.CreateInstance<InspectorObject>();

            Selection.activeObject = rAsset;
        }
    }

}