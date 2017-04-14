using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UExtension
{
    public abstract class ReferenceObject
    {
        public ReferenceObject(string rName, ReferenceManager rManager)
        {
            this.mName      = rName;
            this.mManager   = rManager;
        }
        public string Name
        {
            get { return mName; }
        }
        public int ReferenceCount
        {
            get { return mReferenceCount; }
        }
        public ReferenceManager Manager
        {
            get { return mManager; }
        }

        public int Increment()
        {
            mReferenceCount ++;
            if (1 == mReferenceCount)
            {
                mManager.HandleReferenceValid(this);
                this.OnReferenceValid();
            }

            return mReferenceCount;
        }
        public int Decrement()
        {
            if (0 >= mReferenceCount)
                return mReferenceCount;

            mReferenceCount--;
            if (0 == mReferenceCount)
            {
                mManager.HandleReferenceEmpty(this);
                this.OnReferenceEmpty();
            }

            return mReferenceCount;
        }

        public abstract void Discard();

        protected virtual void OnReferenceValid() { }
        protected virtual void OnReferenceEmpty() { }
        
        protected string            mName;
        protected int               mReferenceCount;
        protected ReferenceManager  mManager;
    }

    public class ReferenceManager
    {
        public virtual void HandleReferenceValid(ReferenceObject rObject) {}
        public virtual void HandleReferenceEmpty(ReferenceObject rObject) {}
    }

    public class TReferenceManager<T> : ReferenceManager
        where T : ReferenceObject
    {
        public TReferenceManager()
            : this(false)
        {}
        public TReferenceManager(bool bImmediate)
        {
            this.mImmediate = bImmediate;
        }
        public bool Contains(string rName)
        {
            return mReferenceDictionary.ContainsKey(rName);
        }
        public T Get(string rName)
        {
            T rResult = default(T);
            mReferenceDictionary.TryGetValue(rName, out rResult);
            return rResult;
        }
        public T GetOrCreateObject(string rName)
        {
            T rResult = default(T);
            if (mReferenceDictionary.TryGetValue(rName, out rResult))
                return rResult;

            rResult = ReflectExtension.Construct<T>(rName, this);
            if (null == rResult)
                return default(T);
            mReferenceDictionary.Add(rName, rResult);

            return rResult;
        }
        public bool GetOrCreateObject(string rName, out T rResult)
        {
            rResult = default(T);
            if (mReferenceDictionary.TryGetValue(rName, out rResult))
                return false;

            rResult = ReflectExtension.Construct<T>(rName, this);
            if (null == rResult)
                return false;

            mReferenceDictionary.Add(rName, rResult);
            return true;
        }
        public void DiscardReferenceEmpty()
        {
            List<string> rRemoveList = new List<string>();
            foreach(var rPair in mReferenceDictionary)
            {
                if (0 == rPair.Value.ReferenceCount)
                {
                    rPair.Value.Discard();
                    rRemoveList.Add(rPair.Key);
                }
            }

            for (int nIndex = 0; nIndex < rRemoveList.Count; ++ nIndex)
                mReferenceDictionary.Remove(rRemoveList[nIndex]);
        }
        public void DiscardAllAndClear()
        {
            foreach(var rPair in mReferenceDictionary)
                rPair.Value.Discard();
            mReferenceDictionary.Clear();
        }
        public override void HandleReferenceEmpty(ReferenceObject rObject)
        {
            if (null == rObject)
                return;

            T rReferenceObject = default(T);
            if (!mReferenceDictionary.TryGetValue(rObject.Name, out rReferenceObject))
                return; // Error

            if (rReferenceObject != rObject)
                return; // Error

            if (mImmediate)
            {
                mReferenceDictionary.Remove(rReferenceObject.Name);
                rReferenceObject.Discard();
            }
        }

        protected bool                  mImmediate           = false;
        protected Dictionary<string, T> mReferenceDictionary = new Dictionary<string, T>();
    }

}