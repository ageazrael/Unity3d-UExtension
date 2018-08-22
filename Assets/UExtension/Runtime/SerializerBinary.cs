using System.IO;
using System;
using System.Net;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace UExtension
{
    /// <summary>
    /// SerializerBinary
    ///    Public Field/Public Property(get;set)
    /// </summary>
    [TSIgnore]
    public class SerializerBinary
    {
        public virtual void Serialize(BinaryWriter rWriter) { }
        public virtual void Deserialize(BinaryReader rReader) { }
    }
    public class SerializerBinaryTypes : TypeSearchDefault<SerializerBinary> { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SBIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SBEnableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SBDynamicAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SBGroupAttribute : Attribute
    {
        public string GroupName;

        public SBGroupAttribute(string rGroupName)
        {
            this.GroupName = rGroupName;
        }
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SBGroupIneritedAttribute : SBGroupAttribute
    {
        public SBGroupIneritedAttribute(string rGroupName)
            : base(rGroupName)
        {}
    }

    /// <summary>
    /// ValueTypeSerialize
    /// </summary>
    public static class ValueTypeSerialize
    {
        public static void Serialize(this BinaryWriter rWriter, char value)     => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, byte value)     => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, sbyte value)    => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, bool value)     => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, short value)    => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, ushort value)   => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, int value)      => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, uint value)     => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, long value)     => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, ulong value)    => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, float value)    => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, double value)   => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, decimal value)  => rWriter.Write(value);
        public static void Serialize(this BinaryWriter rWriter, string value)
        {
            bool bValid = !string.IsNullOrEmpty(value);
            rWriter.Write(bValid);
            if (bValid)
                rWriter.Write(value);
        }
    }
    /// <summary>
    /// ValueTypeDeserialize
    /// </summary>
    public static class ValueTypeDeserialize
    {
        public static char      Deserialize(this BinaryReader rReader, char value)    => rReader.ReadChar();
        public static byte      Deserialize(this BinaryReader rReader, byte value)    => rReader.ReadByte();
        public static sbyte     Deserialize(this BinaryReader rReader, sbyte value)   => rReader.ReadSByte();
        public static bool      Deserialize(this BinaryReader rReader, bool value)    => rReader.ReadBoolean();
        public static short     Deserialize(this BinaryReader rReader, short value)   => rReader.ReadInt16();
        public static ushort    Deserialize(this BinaryReader rReader, ushort value)  => rReader.ReadUInt16();
        public static int       Deserialize(this BinaryReader rReader, int value)     => rReader.ReadInt32();
        public static uint      Deserialize(this BinaryReader rReader, uint value)    => rReader.ReadUInt32();
        public static long      Deserialize(this BinaryReader rReader, long value)    => rReader.ReadInt64();
        public static ulong     Deserialize(this BinaryReader rReader, ulong value)   => rReader.ReadUInt64();
        public static float     Deserialize(this BinaryReader rReader, float value)   => rReader.ReadSingle();
        public static double    Deserialize(this BinaryReader rReader, double value)  => rReader.ReadDouble();
        public static decimal   Deserialize(this BinaryReader rReader, decimal value) => rReader.ReadDecimal();
        public static string    Deserialize(this BinaryReader rReader, string value)
        {
            bool bValid = rReader.ReadBoolean();
            if (!bValid)
                return string.Empty;
            return rReader.ReadString();
        }
    }
    /// <summary>
    /// SerializerBinarySerialize
    /// </summary>
    public static class SerializerBinarySerialize
    {
        public static void Serialize<T>(this BinaryWriter rWriter, T rValue)
            where T : SerializerBinary
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
                rValue.Serialize(rWriter);
        }
        public static void SerializeDynamic<T>(this BinaryWriter rWriter, T rValue)
            where T : SerializerBinary
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
            {
                rWriter.Serialize(rValue.GetType().FullName);
                rValue.Serialize(rWriter);
            }
        }
    }
    /// <summary>
    /// SerializerBinaryDeserialize
    /// </summary>
    public static class SerializerBinaryDeserialize
    {
        public static T Deserialize<T>(this BinaryReader rReader, T value)
            where T : SerializerBinary
        {
            bool bValid = rReader.Deserialize(false);
            if (!bValid)
                return null;

            var rInstance = ReflectExtension.Construct<T>();
            rInstance.Deserialize(rReader);
            return rInstance;
        }
        public static T DeserializeDynamic<T>(this BinaryReader rReader, T rValue)
            where T : SerializerBinary
        {
            bool bValid = rReader.Deserialize(false);
            if (!bValid)
                return null;

            var rFullName = rReader.Deserialize(string.Empty);
            var rInstance = ReflectExtension.TConstruct<T>(Type.GetType(rFullName));
            rInstance.Deserialize(rReader);
            return rInstance;
        }
    }

    [TSIgnore]
    public partial class SerializerBinaryArchive<T> : SerializerBinary
        where T : SerializerBinary
    {
        public static T LoadArchive(string rFilePath) => ReflectExtension.Construct<T>().LoadArchive(rFilePath);
        public static T LoadArchiveByURL(string rURL) => ReflectExtension.Construct<T>().LoadArchiveByURL(rURL);
    }


    public static class SerializerBinaryExpand
    {
        public static T LoadArchive<T>(this T rSerializerBinary, string rFilePath)
            where T : SerializerBinary
        {
            if (string.IsNullOrEmpty(rFilePath))
                return rSerializerBinary; // TODO: invalid path

            if (!Directory.Exists(Path.GetDirectoryName(rFilePath)))
                return rSerializerBinary; // TODO: invalid path

            using(var rFileStream = File.OpenRead(rFilePath))
            {
                if (null != rSerializerBinary)
                    rSerializerBinary.Deserialize(new BinaryReader(rFileStream));
            }
            return rSerializerBinary;
        }
        public static T LoadArchiveByURL<T>(this T rSerializerBinary, string rURL)
            where T : SerializerBinary
        {
            if (string.IsNullOrEmpty(rURL))
                return rSerializerBinary; // TODO: invalid path

            using(var rWebClient = new WebClient())
            {
                using(var rMemoryStream = new MemoryStream(rWebClient.DownloadData(rURL)))
                {
                    if (null != rSerializerBinary)
                        rSerializerBinary.Deserialize(new BinaryReader(rMemoryStream));
                }
            }
            return rSerializerBinary;
        }
        public static Coroutine LoadArchiveByWWW<T>(this T rSerializerBinary, string rURL, Action rCompleted = null)
            where T : SerializerBinary
            => CoroutineManager.Start(HandleLoadArchiveByWWW(rSerializerBinary, rURL, rCompleted));
        private static IEnumerator HandleLoadArchiveByWWW<T>(this T rSerializerBinary, string rURL, Action rCompleted)
            where T : SerializerBinary
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
                rSerializerBinary.Deserialize(new BinaryReader(rMemoryStream));
            }

            if (null != rCompleted)
                rCompleted();
        }
        public static void SaveArchive<T>(this T rSerializerBinary, string rFilePath, int nCapacity = 2048)
            where T : SerializerBinary
        {
            if (null == rSerializerBinary)
                return; // invalid object

            if (string.IsNullOrEmpty(rFilePath))
                return; // invalid path

            if (!Directory.Exists(Path.GetDirectoryName(rFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(rFilePath));

            using(var rMemberStream = new MemoryStream(nCapacity))
            {
                var rWriter       = new BinaryWriter(rMemberStream);
                rSerializerBinary.Serialize(rWriter);

                File.WriteAllBytes(rFilePath, rMemberStream.ToArray());
            }
        }
    }

    public class SerializerBinaryUtility
    {
        public static List<MemberInfo> SearchSerializeMember(Type rType, bool bIncludeBaseClass = true)
        {
            var rMemberInfos = new List<MemberInfo>();
            foreach(var rMemberInfo in rType.GetMembers())
            {
                if((rMemberInfo.MemberType != MemberTypes.Field &&
                    rMemberInfo.MemberType != MemberTypes.Property) ||
                    (rMemberInfo.DeclaringType == rType && !bIncludeBaseClass))
                    continue;

                if(rMemberInfo.IsDefined(typeof(SBIgnoreAttribute), false))
                    continue;

                if(rMemberInfo.MemberType == MemberTypes.Property &&
                    (!(rMemberInfo as PropertyInfo).CanRead || !(rMemberInfo as PropertyInfo).CanWrite))
                {
                    continue;
                }

                rMemberInfos.Add(rMemberInfo);
            }
            foreach(var rMemberInfo in rType.GetMembers(BindingFlags.NonPublic|BindingFlags.Instance))
            {
                if((rMemberInfo.MemberType != MemberTypes.Field &&
                    rMemberInfo.MemberType != MemberTypes.Property) ||
                    rMemberInfo.DeclaringType != rType)
                    continue;

                if(!rMemberInfo.IsDefined(typeof(SBEnableAttribute), false))
                    continue;

                if(rMemberInfo.MemberType == MemberTypes.Property &&
                    (!(rMemberInfo as PropertyInfo).CanRead || !(rMemberInfo as PropertyInfo).CanWrite))
                {
                    continue;
                }

                rMemberInfos.Add(rMemberInfo);
            }
            return rMemberInfos;
        }
    }
}

