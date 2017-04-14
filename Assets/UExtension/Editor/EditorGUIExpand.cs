using UnityEngine;
using UnityEditor;

namespace UExtension
{
    public class HorizontalSpace : System.IDisposable
    {
        public HorizontalSpace()
        {
            GUILayout.BeginHorizontal();
        }

        void System.IDisposable.Dispose()
        {
            GUILayout.EndHorizontal();
        }
    }
    public class VerticalSpace : System.IDisposable
    {
        public VerticalSpace()
        {
            GUILayout.BeginVertical();
        }

        void System.IDisposable.Dispose()
        {
            GUILayout.EndVertical();
        }
    }
	public class LabelWidthSpace : System.IDisposable
	{
		float oldLabelWidth = 0;
		public LabelWidthSpace(float labelWidth)
		{
			oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = labelWidth;
		}
		void System.IDisposable.Dispose()
		{
			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
	}
}