using System;

namespace UExtension
{

    public static class TypeExtension
    {
        public static Type SearchBaseTo(this Type rType, Type rBaseType)
        {
            var rSearchType = rType;
            while (rSearchType.BaseType != rBaseType && null != rSearchType.BaseType)
                rSearchType = rSearchType.BaseType;

            return rSearchType.BaseType == rBaseType ? rSearchType : null;
        }
        public static Type SearchBaseTo<T>(this Type rType)
        {
            return SearchBaseTo(rType, typeof(T));
        }
    }

}