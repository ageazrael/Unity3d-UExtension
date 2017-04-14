using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using UExtension;

namespace UnitTest.Common
{

    public class UTReferenceObject : ReferenceObject
    {
        public UTReferenceObject(string rName, ReferenceManager rManager)
            : base(rName, rManager)
        {}
        
        public bool     IsDiscard;
        
        public override void Discard()
        {
            this.IsDiscard = true;
        }

        protected override void OnReferenceValid()
        {
            this.IsDiscard = false;
        }
    }

    public class ReferenceManagerUnitTest
    {
	    [Test]
	    public void Domain()
        {
            {
                var rImmediateManager = new TReferenceManager<UTReferenceObject>(true);;
                var r01 = rImmediateManager.GetOrCreateObject("01");

                Assert.AreNotEqual(rImmediateManager.Get("01"), null);

                Assert.AreEqual(r01.Increment(),    1);
                Assert.AreEqual(r01.ReferenceCount, 1);

                Assert.AreEqual(r01.Increment(),    2);
                Assert.AreEqual(r01.ReferenceCount, 2);

                Assert.AreEqual(r01.Decrement(),    1);
                Assert.AreEqual(r01.ReferenceCount, 1);

                Assert.AreEqual(r01.Decrement(),    0);
                Assert.AreEqual(r01.ReferenceCount, 0);
                Assert.AreEqual(r01.IsDiscard,      true);
            }
		    
            {
                var rLazyManager = new TReferenceManager<UTReferenceObject>(false);
                var r01 = rLazyManager.GetOrCreateObject("01");
                var r02 = rLazyManager.GetOrCreateObject("02");

                Assert.AreNotEqual(rLazyManager.Get("01"), null);
                Assert.AreNotEqual(rLazyManager.Get("02"), null);

                Assert.AreEqual(r01.Increment(),    1);
                Assert.AreEqual(r01.ReferenceCount, 1);
                Assert.AreEqual(r02.Increment(),    1);
                Assert.AreEqual(r02.ReferenceCount, 1);

                Assert.AreEqual(r01.Decrement(),    0);
                Assert.AreEqual(r01.ReferenceCount, 0);
                Assert.AreEqual(r01.IsDiscard,      false);
                Assert.AreEqual(r02.Decrement(),    0);
                Assert.AreEqual(r02.ReferenceCount, 0);
                Assert.AreEqual(r02.IsDiscard,      false);

                rLazyManager.DiscardReferenceEmpty();

                Assert.AreEqual(r01.IsDiscard,      true);
                Assert.AreEqual(r02.IsDiscard,      true);

                rLazyManager.DiscardAllAndClear();
                Assert.AreEqual(rLazyManager.Get("01"), null);
                Assert.AreEqual(rLazyManager.Get("02"), null);
            }
	    }
    }
}