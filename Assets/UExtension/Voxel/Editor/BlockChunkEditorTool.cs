using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace UExtension.Voxel
{
    [EditorTool("Block Chunk Tool", typeof(BlockChunk))]
    public class BlockChunkEditorTool : EditorTool, IDrawSelectedHandles
    {
        public override void OnToolGUI(EditorWindow window) { }

        public void OnDrawHandles()
        {
            BlockChunk rChunk = null;
            Block rBlock = null;
            int nIndex = 0;
            EBlockFace nFace = EBlockFace.Top;

            var rCamera = SceneView.lastActiveSceneView.camera;
            var rPoint = rCamera.ScreenToViewportPoint(new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y, 0));
            rPoint.y = 1.0f - rPoint.y;
            if (Physics.Raycast(rCamera.ViewportPointToRay(rPoint), out var rHitInfo, 1000))
            {
                rChunk = rHitInfo.collider.GetComponent<BlockChunk>();
                if (!rChunk)
                {
                    return;
                }

                nIndex = (int)rHitInfo.textureCoord2.x;
                nFace = (EBlockFace)rHitInfo.textureCoord2.y;

                rBlock = rChunk.Store[nIndex];

                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.DrawWireCube(rBlock.Position, new Vector3(1.1f, 1.1f, 1.1f));
                Handles.DrawWireCube(rHitInfo.point, new Vector3(0.2f, 0.2f, 0.2f));
                SceneView.RepaintAll();
            }

            if (rChunk != null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (BlockChunkEditorState.Mode == EBlockChunkEditorMode.Add && BlockChunkEditorState.SelectBlockData != null)
                {
                    Vector3Int rOffset = new Vector3Int();
                    switch (nFace)
                    {
                        case EBlockFace.Top:    rOffset.y += 1; break;
                        case EBlockFace.Bottom: rOffset.y -= 1; break;
                        case EBlockFace.Front:  rOffset.z += 1; break;
                        case EBlockFace.Back:   rOffset.z -= 1; break;
                        case EBlockFace.Left:   rOffset.x -= 1; break;
                        case EBlockFace.Right:  rOffset.x += 1; break;
                    }

                    rChunk.SetBlockDataFromPosition(rBlock.Position + rOffset, BlockChunkEditorState.SelectBlockData.Data);
                    rChunk.RefreshBlockMesh();
                }
                else if (BlockChunkEditorState.Mode == EBlockChunkEditorMode.Sub)
                {
                    rChunk.SetBlockDataFromPosition(rBlock.Position, null);
                    rChunk.RefreshBlockMesh();
                }
                else if (BlockChunkEditorState.Mode == EBlockChunkEditorMode.Change && BlockChunkEditorState.SelectBlockData != null)
                {
                    rChunk.SetBlockDataFromPosition(rBlock.Position, BlockChunkEditorState.SelectBlockData.Data);
                    rChunk.RefreshBlockMesh();
                }
            }
        }
    }
}