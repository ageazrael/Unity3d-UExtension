﻿using System.Reflection;
using System;

namespace UExtension
{
    public static class ReflectExtension
    {
        public static object Construct(Type rType, params object[] args)
        {
            Debugger.AssertE(null != rType, "rType invalid");

            var rParamType = new Type[args.Length];
            for (int nIndex = 0; nIndex < args.Length; ++nIndex)
                rParamType[nIndex] = args[nIndex].GetType();

            var rConstructor = rType.GetConstructor(rParamType);
            Debugger.AssertE(null != rConstructor, "Invalid Constructor");

            return rConstructor.Invoke(args);
        }

        public static T Construct<T>(params object[] args) => (T)Construct(typeof(T), args);
        public static T TConstruct<T>(Type rType, params object[] args) => (T)Construct(rType, args);

        public static Type GetMemberDataType(this MemberInfo rMemberInfo)
        {
            if (rMemberInfo.MemberType == MemberTypes.Field)
                return (rMemberInfo as FieldInfo).FieldType;
            else if (rMemberInfo.MemberType == MemberTypes.Property)
                return (rMemberInfo as PropertyInfo).PropertyType;
            else
                return null;
        }
        public static T GetMemberDataValue<T>(this MemberInfo rMemberInfo, object rObject)
        {
            if (rMemberInfo.MemberType == MemberTypes.Field)
                return (T)(rMemberInfo as FieldInfo).GetValue(rObject);
            else if (rMemberInfo.MemberType == MemberTypes.Property)
                return (T)(rMemberInfo as PropertyInfo).GetValue(rObject, null);
            else
                return default(T);
        }
        public static void SetMemberDataValue<T>(this MemberInfo rMemberInfo, object rObject, T rValue)
        {
            if (rMemberInfo.MemberType == MemberTypes.Field)
                (rMemberInfo as FieldInfo).SetValue(rObject, rValue);
            else if (rMemberInfo.MemberType == MemberTypes.Property)
                (rMemberInfo as PropertyInfo).SetValue(rObject, rValue, null);
        }
    }

    public static class ICustomAttributeProviderExpand
    {
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider rProvider, bool bInherit)
            where T : System.Attribute
        {
            var rAttributes = rProvider.GetCustomAttributes(typeof(T), bInherit);
            if (rAttributes.Length == 0)
                return default;
            return (T)(rAttributes[0]);
        }
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider rProvider, bool bInherit)
            where T : System.Attribute
        {
            var rAttributes = rProvider.GetCustomAttributes(typeof(T), bInherit);
            var rResultAttrs = new T[rAttributes.Length];
            for (int nIndex = 0; nIndex < rAttributes.Length; ++ nIndex)
                rResultAttrs[nIndex] = rAttributes[nIndex] as T;
            return rResultAttrs;
        }

        /// <summary>
        /// IsApplyAttr<T>/IsApplyAttr
        ///     IsDefined函数功能是判定一个Attribute的使用被定义在该类中或者父类中。但如果使用的Attribute标记的是Inherit=false
        ///     该函数返回true，但无法通过GetCustomAttributes获取。IsApplyAttr为了和GetCustomAttributes的结果对应，
        ///     IsApplyAttr返回true，GetCustomAttributes一定能获得。
        /// </summary>
        public static bool IsApplyAttr<T>(this ICustomAttributeProvider rProvider, bool bInherit) 
            => rProvider.GetCustomAttributes(typeof(T), bInherit).Length > 0;

        public static bool IsApplyAttr(this ICustomAttributeProvider rProvider, Type rType, bool bInherit)
            => rProvider.GetCustomAttributes(rType, bInherit).Length > 0;
    }
}