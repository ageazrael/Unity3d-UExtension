using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UExtension.Voxel
{
    [System.Serializable]
    public class VoxelBlock
    {
        public int Id;
        [TypeSearch(typeof(BlockDataTypes))]
        public string    TypeName;
        [SerializeReference]
        public BlockData Data;
    }
    [CreateAssetMenu(menuName = "UExtension/Voxel/BlockData")]
    public class VoxelSetting : ScriptableObject
    {
        public VoxelBlock[] DataTable;
        public Material AtlasMaterial;

        public void Awake()
        {
            this.RefreshBlock();
        }
        public void OnValidate()
        {
            this.RefreshBlock();
        }

        protected void RefreshBlock()
        {
            foreach(var rBlock in this.DataTable)
            {
                var rType = System.Type.GetType(rBlock.TypeName);
                if (rType == null)
                {
                    continue;
                }
                if (rBlock.Data != null && rBlock.Data.GetType() == rType)
                {
                    rBlock.Data.Id = rBlock.Id;
                    continue;
                }

                rBlock.Data = ReflectExtension.TConstruct<BlockData>(rType);
                if (rBlock.Data != null)
                {
                    rBlock.Data.Id = rBlock.Id;
                }
            }
        }
    }
}
