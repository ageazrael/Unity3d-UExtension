using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension.Voxel
{
    [Serializable]
    [BlockRuntimeImpl(typeof(BlockRTCustom))]
    public class BlockCustomData : BlockData
    {
        public Sprite Top;
        public Sprite Bottom;
        public Sprite Front;
        public Sprite Back;
        public Sprite Left;
        public Sprite Right;
    }
    [Serializable]
    public class BlockRTCustom : BlockRT
    {
        public override void GeneratorMesh(BlockGeneratorMesh rGeneratorMesh, BlockData rBaseData)
        {
            var rData = rBaseData as BlockCustomData;

            if (rGeneratorMesh.IsTopEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Top, rData.Top);
            }
            if (rGeneratorMesh.IsBottomEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Bottom, rData.Bottom);
            }
            if (rGeneratorMesh.IsFrontEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Front, rData.Front);
            }
            if (rGeneratorMesh.IsBackEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Back, rData.Back);
            }
            if (rGeneratorMesh.IsLeftEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Left, rData.Left);
            }
            if (rGeneratorMesh.IsRightEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Right, rData.Right);
            }
        }
    }
}
