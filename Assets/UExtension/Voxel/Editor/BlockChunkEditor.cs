using UnityEngine;
using UnityEditor;
using System.IO;

namespace UExtension.Voxel
{

    public enum EBlockChunkEditorMode
    {
        Add,
        Sub,
        Change,
    }

    public class BlockChunkEditorState
    {
        public static int       SelectBlockId = 0;
        public static VoxelBlock SelectBlockData = null;

        public static EBlockChunkEditorMode Mode = EBlockChunkEditorMode.Add;
        
    }

    [CustomEditor(typeof(BlockChunk), true)]
    public class BlockChunkEditor : Editor
    {
        BlockChunk      mChunk = null;
        VoxelSetting    mVoxelSetting = null;

        public void OnEnable()
        {
            this.mChunk         = this.target as BlockChunk;
            this.mVoxelSetting  = this.mChunk.Data;

        }

        public void OnDisable()
        {
        }

        protected Vector2 mBlockScrollView;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Change"))
            {
                BlockChunkEditorState.Mode = EBlockChunkEditorMode.Change;
            }
            if (GUILayout.Button("+"))
            {
                BlockChunkEditorState.Mode = EBlockChunkEditorMode.Add;
            }
            if (GUILayout.Button("-"))
            {
                BlockChunkEditorState.Mode = EBlockChunkEditorMode.Sub;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            var bOldEnable = GUI.enabled;
            GUI.enabled = BlockChunkEditorState.SelectBlockData != null;
            if (GUILayout.Button("Full"))
            {
                this.mChunk.FilFull(BlockChunkEditorState.SelectBlockData.Data);
                this.mChunk.RefreshBlockMesh();
            }
            if (GUILayout.Button("Point"))
            {
                this.mChunk.FilPoint(BlockChunkEditorState.SelectBlockData.Data);
                this.mChunk.RefreshBlockMesh();
            }
            if (GUILayout.Button("Plane"))
            {
                this.mChunk.FilPlane(BlockChunkEditorState.SelectBlockData.Data);
                this.mChunk.RefreshBlockMesh();
            }
            GUI.enabled = bOldEnable;
            GUILayout.EndHorizontal();

            if (this.mVoxelSetting)
            {
                this.mBlockScrollView = GUILayout.BeginScrollView(this.mBlockScrollView);
                for (var nIndex = 0; nIndex < this.mVoxelSetting.DataTable.Length; nIndex ++)
                {
                    if (GUILayout.Button(this.mVoxelSetting.DataTable[nIndex].Id.ToString(), GUILayout.MaxWidth(32)))
                    {
                        BlockChunkEditorState.SelectBlockId = this.mVoxelSetting.DataTable[nIndex].Id;
                        BlockChunkEditorState.SelectBlockData = this.mVoxelSetting.DataTable[nIndex];
                    }
                }
                GUILayout.EndScrollView();
            }
        }
    }

}