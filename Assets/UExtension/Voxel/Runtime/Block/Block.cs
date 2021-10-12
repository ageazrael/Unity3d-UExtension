using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension.Voxel
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class BlockRuntimeImplAttribute : Attribute
    {
        public System.Type ImplType;
        public BlockRuntimeImplAttribute(Type rRuntimeImplType)
        {
            this.ImplType = rRuntimeImplType;
        }
    }

    [Serializable]
    [BlockRuntimeImpl(typeof(Block))]
    public class BlockData
    {
        [Readonly]
        public int Id;
    }
    public class BlockDataTypes : TypeSearchDefault<BlockData> { }


    public enum EBlockFace
    {
        Top,
        Bottom,
        Front,
        Back,
        Left,
        Right,
    }
    public class BlockGeneratorMesh
    {
        public float CubeHalfSize = 0.5f;

        public bool AllowPickData = true;

        public virtual Block Current => this.mCurrent;
        public List<Vector3> Vertices => this.mVertices;
        public List<Vector2> UVs => this.mUVs;
        public List<Vector2> UV2s => this.mUV2s;
        public List<int> Triangles => this.mTriangles;

        public virtual Block GetBlock(Vector3Int rPositon)
        {
            return null;
        }
        public virtual void AddFace(EBlockFace face, Sprite rSprite)
        {
            this.UpdateCubeVertex();

            var nIndex = this.mVertices.Count;
            switch (face)
            {
                case EBlockFace.Top:
                    this.AddVertices(0, 1, 4, 5);
                    break;
                case EBlockFace.Bottom:
                    this.AddVertices(6, 7, 2, 3);
                    break;
                case EBlockFace.Front:
                    this.AddVertices(1, 0, 3, 2);
                    break;
                case EBlockFace.Back:
                    this.AddVertices(4, 5, 6, 7);
                    break;
                case EBlockFace.Left:
                    this.AddVertices(0, 4, 2, 6);
                    break;
                case EBlockFace.Right:
                    this.AddVertices(5, 1, 7, 3);
                    break;
            }
            var xMin = rSprite.rect.xMin / rSprite.texture.width;
            var yMin = rSprite.rect.yMin / rSprite.texture.height;
            var xMax = rSprite.rect.xMax / rSprite.texture.width;
            var yMax = rSprite.rect.yMax / rSprite.texture.height;
            this.UVs.Add(new Vector2(xMin, yMax));
            this.UVs.Add(new Vector2(xMax, yMax));
            this.UVs.Add(new Vector2(xMin, yMin));
            this.UVs.Add(new Vector2(xMax, yMin));

            if (this.AllowPickData)
            {
                this.UV2s.Add(new Vector2(this.mPickId, (int)face));
                this.UV2s.Add(new Vector2(this.mPickId, (int)face));
                this.UV2s.Add(new Vector2(this.mPickId, (int)face));
                this.UV2s.Add(new Vector2(this.mPickId, (int)face));
            }

            this.mTriangles.Add(nIndex + 0);
            this.mTriangles.Add(nIndex + 1);
            this.mTriangles.Add(nIndex + 2);

            this.mTriangles.Add(nIndex + 1);
            this.mTriangles.Add(nIndex + 3);
            this.mTriangles.Add(nIndex + 2);
        }
        protected void AddVertices(int _1, int _2, int _3, int _4)
        {
            this.mVertices.Add(this.mCubeVertex[_1]);
            this.mVertices.Add(this.mCubeVertex[_2]);
            this.mVertices.Add(this.mCubeVertex[_3]);
            this.mVertices.Add(this.mCubeVertex[_4]);
        }

        public void BeginBlock(Block rBlock, int nPickId)
        {
            this.mCurrent = rBlock;
            this.mPickId = nPickId;
            this.mCubeVertexUpdate = false;
        }
        public void EndBlock() { }

        public Block GetOffsetBlock(int x, int y, int z)
        {
            var rPosition = this.Current.Position;
            return this.GetBlock(new Vector3Int(rPosition.x + x, rPosition.y + y, rPosition.z + z));
        }
        public int GetOffsetBlockId(int x, int y, int z)
        {
            var rBlock = this.GetOffsetBlock(x, y, z);
            return rBlock == null ? Block.EmptyId : rBlock.BlockId;
        }

        public bool IsTopEmpty      => GetOffsetBlockId(0, 1, 0) == Block.EmptyId;
        public bool IsBottomEmpty   => GetOffsetBlockId(0, -1, 0) == Block.EmptyId;
        public bool IsFrontEmpty    => GetOffsetBlockId(0, 0, 1) == Block.EmptyId;
        public bool IsBackEmpty     => GetOffsetBlockId(0, 0, -1) == Block.EmptyId;
        public bool IsLeftEmpty     => GetOffsetBlockId(-1, 0, 0) == Block.EmptyId;
        public bool IsRightEmpty    => GetOffsetBlockId(1, 0, 0) == Block.EmptyId;

        public int TopBlockId      => GetOffsetBlockId(0, 1, 0);
        public int BottomBlockId   => GetOffsetBlockId(0, -1, 0);
        public int FrontBlockId    => GetOffsetBlockId(0, 0, 1);
        public int BackBlockId     => GetOffsetBlockId(0, 0, -1);
        public int LeftBlockId     => GetOffsetBlockId(-1, 0, 0);
        public int RightBlockId    => GetOffsetBlockId(1, 0, 0);

        protected void PreallocBuffer(int nSize)
        {
            var nPreallocSize = nSize * nSize * nSize * 6 * 4;
            this.mVertices = new List<Vector3>(nPreallocSize);
            this.mUVs = new List<Vector2>(nPreallocSize);
            this.mUV2s = new List<Vector2>(nPreallocSize);
            this.mTriangles = new List<int>(nPreallocSize * 2);
            this.mCubeVertex = new Vector3[8];
            this.mFaceVertex = new Vector3[4];
            this.mCubeVertexUpdate = false;
        }

        protected void UpdateCubeVertex()
        {
            if (this.mCubeVertexUpdate)
            {
                return;
            }
            var rPosition = this.Current.Position;
            var fHS = this.CubeHalfSize;

            mCubeVertex[0] = new Vector3(rPosition.x - fHS, rPosition.y + fHS, rPosition.z + fHS);
            mCubeVertex[1] = new Vector3(rPosition.x + fHS, rPosition.y + fHS, rPosition.z + fHS);
            mCubeVertex[2] = new Vector3(rPosition.x - fHS, rPosition.y - fHS, rPosition.z + fHS);
            mCubeVertex[3] = new Vector3(rPosition.x + fHS, rPosition.y - fHS, rPosition.z + fHS);
            mCubeVertex[4] = new Vector3(rPosition.x - fHS, rPosition.y + fHS, rPosition.z - fHS);
            mCubeVertex[5] = new Vector3(rPosition.x + fHS, rPosition.y + fHS, rPosition.z - fHS);
            mCubeVertex[6] = new Vector3(rPosition.x - fHS, rPosition.y - fHS, rPosition.z - fHS);
            mCubeVertex[7] = new Vector3(rPosition.x + fHS, rPosition.y - fHS, rPosition.z - fHS);
            this.mCubeVertexUpdate = true;
        }

        protected Block mCurrent;

        protected List<Vector3> mVertices;
        protected List<Vector2> mUVs;
        protected List<Vector2> mUV2s;
        protected List<int> mTriangles;
        protected Vector3[] mCubeVertex;
        protected bool mCubeVertexUpdate;
        protected Vector3[] mFaceVertex;
        protected int mPickId;
    }

    [Serializable]
    public class Block
    {
        public static readonly int EmptyId = 0;

        public Vector3Int   Position;
        [SerializeReference, Readonly]
        public BlockRT      RT;
        [SerializeReference, Readonly]
        public BlockData    Data;

        public void SetData(BlockData rData)
        {
            this.Data = rData;
            if (this.Data == null)
            {
                this.RT?.OnDestroy();
                this.RT = null;
                return;
            }

            var rTypeAttr = this.Data.GetType().GetCustomAttribute<BlockRuntimeImplAttribute>(true);
            if (rTypeAttr == null)
            {
                this.RT?.OnDestroy();
                this.RT = null;
                return;
            }

            this.RT = ReflectExtension.TConstruct<BlockRT>(rTypeAttr.ImplType);
            this.RT.OnCreate(rData);
        }
        public void Restore()
        {
            if (this.RT != null && this.Data != null)
            {
                this.RT.OnCreate(this.Data);
            }
        }

        public bool IsEmpty => this.RT != null;
        public int  BlockId => this.Data != null ? this.Data.Id : 0;

        public void GeneratorMesh(BlockGeneratorMesh rGeneratorMesh)
        {
            this.RT?.GeneratorMesh(rGeneratorMesh, this.Data);
        }
    }

    [Serializable]
    public class BlockRT
    {
        public virtual void GeneratorMesh(BlockGeneratorMesh rGeneratorMesh, BlockData rData)
        {

        }
        public virtual void OnCreate(BlockData rData) {}
        public virtual void OnDestroy() {}
        public virtual void OnUpdate(BlockData rData) { }
    }
}
