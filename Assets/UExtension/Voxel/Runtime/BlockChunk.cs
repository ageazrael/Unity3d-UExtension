using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UExtension;

namespace UExtension.Voxel
{
    public class BlockChunkGeneratorMesh : BlockGeneratorMesh
    {
        public override Block GetBlock(Vector3Int rPositon)
        {
            if (null == this.mChunk)
            {
                return null;
            }
            var rIndex = this.mChunk.PositionToIndex(rPositon);
            if (!this.mChunk.IsValidIndex(rIndex))
            {
                return null;
            }
            return this.mChunk.Maps[rIndex.x][rIndex.y][rIndex.z];
        }

        public BlockChunkGeneratorMesh(BlockChunk rChunk)
        {
            this.mChunk = rChunk;
            this.PreallocBuffer(this.mChunk.Size);
        }

        protected BlockChunk mChunk;
    }

    [ExecuteAlways]
    public class BlockChunk : MonoBehaviour
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        //[Readonly]
        public VoxelSetting Data;
        public int          Size = 15; // 只能是基数

        public Block[][][]  Maps;

        [Readonly]
        public List<Block>  Store;

        public bool IsValidIndex(Vector3Int rIndex)
        {
            return 
                (rIndex.x >= 0 && rIndex.x < this.Size) &&
                (rIndex.y >= 0 && rIndex.y < this.Size) && 
                (rIndex.z >= 0 && rIndex.z < this.Size);
        }

        public Vector3Int IndexToPosition(Vector3Int rIndex)
        {
            var nHalfSize = Mathf.FloorToInt(this.Size / 2);

            return new Vector3Int(rIndex.x - nHalfSize, rIndex.y - nHalfSize, rIndex.z - nHalfSize);
        }
        public Vector3Int PositionToIndex(Vector3Int rPosition)
        {
            var nHalfSize = Mathf.FloorToInt(this.Size / 2);

            return new Vector3Int(rPosition.x + nHalfSize, rPosition.y + nHalfSize, rPosition.z + nHalfSize);
        }

        [ContextMenu("CreateBlockMap")]
        public void CreateBlockMap()
        {
            this.Maps = new Block[this.Size][][];
            this.Store = new List<Block>();
            for (var x = 0; x < this.Size; x ++)
            {
                this.Maps[x] = new Block[this.Size][];
                for (var y = 0; y < this.Size; y ++)
                {
                    this.Maps[x][y] = new Block[this.Size];
                    for (var z = 0; z < this.Size; z ++)
                    {
                        var rNewBlock = new Block();
                        rNewBlock.Position = this.IndexToPosition(new Vector3Int(x, y, z));
                        this.Store.Add(rNewBlock);
                        this.Maps[x][y][z] = rNewBlock;
                    }
                }
            }
        }
        [ContextMenu("RefreshBlockMesh")]
        public void RefreshBlockMesh()
        {
            if (!this.MeshData)
            {
                return;
            }

            var rGeneratorMesh = new BlockChunkGeneratorMesh(this);
            for (var nIndex = 0; nIndex < this.Store.Count; nIndex ++)
            {
                var rBlock = this.Store[nIndex];
                rGeneratorMesh.BeginBlock(rBlock, nIndex);
                rBlock.GeneratorMesh(rGeneratorMesh);
                rGeneratorMesh.EndBlock();
            }

            this.MeshData.Clear();
            this.MeshData.vertices = rGeneratorMesh.Vertices.ToArray();
            this.MeshData.uv = rGeneratorMesh.UVs.ToArray();
            this.MeshData.triangles = rGeneratorMesh.Triangles.ToArray();
            if (rGeneratorMesh.AllowPickData)
            {
                this.MeshData.uv2 = rGeneratorMesh.UV2s.ToArray();
            }
            this.MeshData.RecalculateBounds();
            this.MeshData.RecalculateNormals();
            this.StartCoroutine(this.RefreshCollider());

            if (Application.isPlaying)
            {
                Object.Destroy(this.Collider);
            }
            else
            {
                Object.DestroyImmediate(this.Collider);
            }

            this.Collider = this.gameObject.AddComponent<MeshCollider>();

            this.Renderer.sharedMaterial = this.Data.AtlasMaterial;
        }

        protected IEnumerator RefreshCollider()
        {
            this.Collider.enabled = false;
            yield return new WaitForSeconds(0.1f);
            this.Collider.enabled = true;
        }

        public void FilFull(BlockData rData)
        {
            foreach(var rBlock in this.Store)
            {
                rBlock.SetData(rData);
            }
        }
        public void FilPoint(BlockData rData)
        {
            this.FilFull(null);
            this.SetBlockDataFromPosition(new Vector3Int(0, 0, 0), rData);
        }
        public void FilPlane(BlockData rData)
        {
            this.FilFull(null);
            var y = this.Size / 2;
            for (var x = 0; x < this.Size; x ++)
            {
                for (var z = 0; z < this.Size; z ++)
                {
                    this.Maps[x][y][z].SetData(rData);
                }
            }
        }

        public void SetBlockDataFromPosition(Vector3Int rPosition, BlockData rData)
        {
            var rIndex = this.PositionToIndex(rPosition);
            if (this.IsValidIndex(rIndex))
            {
                this.Maps[rIndex.x][rIndex.y][rIndex.z].SetData(rData);
            }
        }

        [Readonly]
        public Mesh MeshData;
        public MeshFilter Filter;
        public MeshRenderer Renderer;
        public MeshCollider Collider;

        void Awake()
        {
            this.Filter = this.GetComponent<MeshFilter>();
            if (!this.Filter)
            {
                this.Filter = this.gameObject.AddComponent<MeshFilter>();
            }

            this.MeshData = this.Filter.sharedMesh;
            if (!this.MeshData)
            {
                this.MeshData = new Mesh();
                this.Filter.sharedMesh = this.MeshData;
            }

            this.Renderer = this.GetComponent<MeshRenderer>();
            if (!this.Renderer)
            {
                this.Renderer = this.gameObject.AddComponent<MeshRenderer>();
            }

            this.Collider = this.GetComponent<MeshCollider>();
            if (!this.Collider)
            {
                this.Collider = this.gameObject.AddComponent<MeshCollider>();
            }

            if (this.Data)
            {
                this.Renderer.sharedMaterial = this.Data.AtlasMaterial;
            }

            this.RestoreMaps();
        }

        protected void RestoreMaps()
        {
            if (this.Store != null && this.Store.Count > 0)
            {
                var nIndex = 0;
                this.Maps = new Block[this.Size][][];
                for (var x = 0; x < this.Size; x++)
                {
                    this.Maps[x] = new Block[this.Size][];
                    for (var y = 0; y < this.Size; y++)
                    {
                        this.Maps[x][y] = new Block[this.Size];
                        for (var z = 0; z < this.Size; z++)
                        {
                            var rBlock = this.Store[nIndex];
                            this.Maps[x][y][z] = rBlock;
                            rBlock.Restore();
                            nIndex++;
                        }
                    }
                }
            }
        }

        void Update()
        {

        }

#if UNITY_EDITOR
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.RestoreMaps();
        }
#endif
    }
}