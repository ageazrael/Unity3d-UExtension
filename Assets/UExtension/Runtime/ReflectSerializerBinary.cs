using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UExtension
{
    /// <summary>
    /// ReflectSerializerBinary
    /// </summary>
    public class ReflectSerializerBinary
    {
        public static void Serialize(object rObject, BinaryWriter rWriter)
        {
            if (null == rObject)
                return; // TODO: invalid parameter

            var rType = rObject.GetType();
            Serialize(rObject, rType, rWriter);
        }
        public static object Deserialize(object rObject, Type rType, BinaryReader rReader)
        {
            if (Type.GetTypeCode(rType) == TypeCode.Object)
            {
                bool bIsEmpty = rReader.ReadBoolean();
                if (bIsEmpty)
                    return null;

                if (rType.GetInterface("System.Collections.IList") != null)
                    return DeserializeList(rObject, rType, rReader);
                else if (rType.GetInterface("System.Collections.IDictionary") != null)
                    return DeserializeDictionary(rObject, rType, rReader);
                else
                    return DeserializeClass(rObject, rType, rReader);
            }
            else
            {
                return ReadPrimitive(rType, rReader);
            }
        }
        static void Serialize(object rObject, Type rType, BinaryWriter rWriter)
        {
            if (Type.GetTypeCode(rType) == TypeCode.Object)
            {
                rWriter.Write(rObject == null);

                if (rObject != null)
                {
                    if (rType.GetInterface("System.Collections.IList") != null)
                        SerializeList(rObject, rWriter);
                    else if (rType.GetInterface("System.Collections.IDictionary") != null)
                        SerializeDictionary(rObject, rWriter);
                    else
                        SerializeClass(rObject, rWriter);
                }
            }
            else
            {
                WritePrimitive(rType, rObject, rWriter);
            }
        }

        static void SerializeClass(object rObject, BinaryWriter rWriter)
        {
            var rMemberInfos = SerializerBinaryUtility.SearchSerializeMember(rObject.GetType(), true);
            foreach (var rMemberInfo in rMemberInfos)
            {
                var rMemberType  = rMemberInfo.GetMemberDataType();
                var rMemberValue = rMemberInfo.GetMemberDataValue<object>(rObject);
                if (null == rMemberType)
                    continue;
                
                if (rMemberInfo.IsApplyAttr<SBDynamicAttribute>(false) && null != rMemberValue)
                {
                    rMemberType = rMemberValue.GetType();
                    rWriter.Write(rMemberType.AssemblyQualifiedName);
                }

                Serialize(rMemberValue, rMemberType, rWriter);
            }
        }
        static object DeserializeClass(object rObject, Type rType, BinaryReader rReader)
        {
            object rClassObject = null;
            if (null == rObject)
                rClassObject = ReflectExtension.Construct(rType);
            else
                rClassObject = rObject;
            var rMemberInfos = SerializerBinaryUtility.SearchSerializeMember(rType, true);
            foreach(var rMemberInfo in rMemberInfos)
            {
                var rMemberType = rMemberInfo.GetMemberDataType();
                if (null == rMemberType)
                    continue;
                if (rMemberInfo.IsApplyAttr<SBDynamicAttribute>(false))
                    rMemberType = Type.GetType(rReader.ReadString());
                if (null == rMemberType)
                    continue; // TODO: 找不到这个类型啊！！应该会影响程序接下来的运行的。

                rMemberInfo.SetMemberDataValue(rClassObject, Deserialize(null, rMemberType, rReader));
            }
            
            return rClassObject;
        }
        static void SerializeList(object rObject, BinaryWriter rWriter)
        {
            var rList = rObject as IList;
            if (rList == null)
                return; // TODO: 无法转换

            Type rType = rList.GetType();
            Type rElementType = null;
            if (rType.IsArray)
                rElementType = rType.GetElementType();
            else
                rElementType = rType.GetGenericArguments()[0];

            rWriter.Write(rList.Count);
            for (int nIndex = 0; nIndex < rList.Count; ++nIndex)
                Serialize(rList[nIndex], rElementType, rWriter);
        }
        static object DeserializeList(object rObject, Type rType, BinaryReader rReader)
        {
            var nCount = rReader.ReadInt32();

            IList rListObject = null;
            if (rType.IsArray)
            {
                var rElementType = rType.GetElementType();
                if (null == rObject)
                    rListObject = ReflectExtension.TConstruct<IList>(rType, nCount);
                else
                    rListObject = rObject as IList;
                for (int nIndex = 0; nIndex < nCount; ++nIndex)
                    rListObject[nIndex] = Deserialize(null, rElementType, rReader);

                return rListObject;
            }
            else
            {
                var rElementType = rType.GetGenericArguments()[0];
                if (null == rObject)
                    rListObject = ReflectExtension.TConstruct<IList>(rType);
                else
                    rListObject = rObject as IList;
                for (int nIndex = 0; nIndex < nCount; ++nIndex)
                    rListObject.Add(Deserialize(null, rElementType, rReader));

                return rListObject;
            }
        }
        static void SerializeDictionary(object rObject, BinaryWriter rWriter)
        {
            var rDictionary = rObject as IDictionary;
            if (rDictionary == null)
                return; // TODO: 无法转换

            var rGenericTypes = rObject.GetType().GetGenericArguments();
            if (rGenericTypes.Length != 2)
                return; // TODO: 无效模版个数啊

            var rKeyType = rGenericTypes[0];
            var rValueType = rGenericTypes[1];

            rWriter.Write(rDictionary.Count);
            var rDictionaryEnumerator = rDictionary.GetEnumerator();
            while (rDictionaryEnumerator.MoveNext())
            {
                Serialize(rDictionaryEnumerator.Key,   rKeyType,   rWriter);
                Serialize(rDictionaryEnumerator.Value, rValueType, rWriter);
            }
        }
        static object DeserializeDictionary(object rObject, Type rType, BinaryReader rReader)
        {
            var rGenericTypes = rType.GetGenericArguments();
            if (rGenericTypes.Length != 2)
                return null; // TODO: 这个类型不是标准的Dictionary
            IDictionary rDictionaryObject = null;
            if (null == rObject)
                rDictionaryObject = ReflectExtension.TConstruct<IDictionary>(rType);
            else
                rDictionaryObject = rObject as IDictionary;
            
            var rKeyType   = rGenericTypes[0];
            var rValueType = rGenericTypes[1];
            var nCount = rReader.ReadInt32();
            for (int nIndex = 0; nIndex < nCount; ++nIndex)
            {
                var rKey   = Deserialize(null, rKeyType, rReader);
                var rValue = Deserialize(null, rValueType, rReader);
                rDictionaryObject.Add(rKey, rValue);
            }
            return rDictionaryObject;
        }
        static void WritePrimitive(Type rType, object rObject, BinaryWriter rWriter)
        {
            var nTypeCode = Type.GetTypeCode(rType);
            switch (nTypeCode)
            {
            case TypeCode.Boolean:
                rWriter.Write((bool)rObject);
                break;

            case TypeCode.Char:
                rWriter.Write((char)rObject);
                break;

            case TypeCode.SByte:
                rWriter.Write((sbyte)rObject);
                break;

            case TypeCode.Byte:
                rWriter.Write((byte)rObject);
                break;

            case TypeCode.Int16:
                rWriter.Write((short)rObject);
                break;

            case TypeCode.UInt16:
                rWriter.Write((ushort)rObject);
                break;

            case TypeCode.Int32:
                rWriter.Write((int)rObject);
                break;

            case TypeCode.UInt32:
                rWriter.Write((uint)rObject);
                break;

            case TypeCode.Int64:
                rWriter.Write((long)rObject);
                break;

            case TypeCode.UInt64:
                rWriter.Write((ulong)rObject);
                break;

            case TypeCode.Single:
                rWriter.Write((float)rObject);
                break;

            case TypeCode.Double:
                rWriter.Write((double)rObject);
                break;

            case TypeCode.Decimal:
                rWriter.Write((decimal)rObject);
                break;

            case TypeCode.String:
                {
                    var bValid = !string.IsNullOrEmpty((string)rObject);
                    rWriter.Write(bValid);
                    if (bValid)
                        rWriter.Write((string)rObject);
                }
                break;

            default:
                break;
            }
        }
        static object ReadPrimitive(Type rType, BinaryReader rReader)
        {
            switch (Type.GetTypeCode(rType))
            {
            case TypeCode.Boolean:
                return rReader.ReadBoolean();

            case TypeCode.Char:
                return rReader.ReadChar();

            case TypeCode.SByte:
                return rReader.ReadSByte();

            case TypeCode.Byte:
                return rReader.ReadByte();

            case TypeCode.Int16:
                return rReader.ReadInt16();

            case TypeCode.UInt16:
                return rReader.ReadUInt16();

            case TypeCode.Int32:
                return rReader.ReadInt32();

            case TypeCode.UInt32:
                return rReader.ReadUInt32();

            case TypeCode.Int64:
                return rReader.ReadInt64();

            case TypeCode.UInt64:
                return rReader.ReadUInt64();

            case TypeCode.Single:
                return rReader.ReadSingle();

            case TypeCode.Double:
                return rReader.ReadDouble();

            case TypeCode.Decimal:
                return rReader.ReadDecimal();

            case TypeCode.String:
                {
                    var bValid = rReader.ReadBoolean();
                    if (bValid)
                        return rReader.ReadString();
                    else
                        return string.Empty;
                }

            default:
                return null;

            }
        }
    }


    public class ReflectSerializerBinaryArchive<T>
        where T : class
    {
        public static T StaticLoadArchive(string rFilePath)
        {
            if (string.IsNullOrEmpty(rFilePath))
                return null; // TODO: invalid path

            if (!Directory.Exists(Path.GetDirectoryName(rFilePath)))
                return null; // TODO: invalid path

            using(var rFileStream = File.OpenRead(rFilePath))
            {
                return ReflectSerializerBinary.Deserialize(null, typeof(T), new BinaryReader(rFileStream)) as T;
            }
        }
        
        public bool LoadArchive(string rFilePath)
        {
            if (string.IsNullOrEmpty(rFilePath))
                return false; // TODO: invalid path

            if (!Directory.Exists(Path.GetDirectoryName(rFilePath)))
                return false; // TODO: invalid path

            using(var rFileStream = File.OpenRead(rFilePath))
            {
                ReflectSerializerBinary.Deserialize(this, this.GetType(), new BinaryReader(rFileStream));
            }
            return true;
        }
        public Coroutine LoadArchiveByWWW(string rURL, Action rCompleted = null)
        {
            return CoroutineManager.Start(HandleLoadArchiveByWWW(rURL, rCompleted));
        }
        private IEnumerator HandleLoadArchiveByWWW(string rURL, Action rCompleted)
        {
            var www = new WWW(rURL);
            yield return www;

            if(!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                yield break;
            }

            using(var rMemoryStream = new MemoryStream(www.bytes))
            {
                ReflectSerializerBinary.Deserialize(this, this.GetType(), new BinaryReader(rMemoryStream));
            }

            if (null != rCompleted)
                rCompleted();
        }
        public bool SaveArchive(string rFilePath, int nCapacity = 2048)
        {
            if (string.IsNullOrEmpty(rFilePath))
                return false; // invalid path

            if (!Directory.Exists(Path.GetDirectoryName(rFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(rFilePath));

            using(var rFileStream = File.OpenWrite(rFilePath))
            {
                ReflectSerializerBinary.Serialize(this, new BinaryWriter(rFileStream));
            }
            return true;
        }
    }
}