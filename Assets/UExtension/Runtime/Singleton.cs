using System.Collections;

namespace UExtension
{
    public class TSingleton<T> : System.IDisposable
        where T : System.IDisposable
    {
        public static T Instance
        {
            get {
                if (null == GInstance)
                    return CreateInstance();
                return GInstance;
            }
        }

        public static T CreateInstance(params object[] args)
        {
            if (null == GInstance)
            {
                lock (GInstanceLock)
                {
                    if (null == GInstance)
                        GInstance = ReflectExtension.Construct<T>(args);
                }
            }
            return GInstance;
        }
        public static void DestroyInstance()
        {
            if (null != GInstance)
            {
                lock (GInstanceLock)
                {
                    GInstance?.Dispose();
                    GInstance = default(T);
                }
            }
        }
        public virtual void Dispose()   {}


#region protected Field
        protected static T      GInstance;
        protected static object GInstanceLock = new object();
#endregion
    }
}