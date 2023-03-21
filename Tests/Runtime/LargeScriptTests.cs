using Hudossay.AttributeEvents.Assets.Runtime;
using Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime
{
    public class LargeScriptTests
    {
        [UnityTest]
        public IEnumerator LargeScriptsAllResponsesCalled()
        {
            var (broadcaster, listener) = GetSubscriptedPair();

            var largeBroadcaster = broadcaster.GetComponent<LargeBroadcaster>();
            var largeListener = listener.GetComponent<LargeListener>();

            largeBroadcaster.Event0.Raise(1);
            largeBroadcaster.Event1.Raise(1);
            largeBroadcaster.Event2.Raise(1);
            largeBroadcaster.Event3.Raise(1);
            largeBroadcaster.Event4.Raise(1);
            largeBroadcaster.Event5.Raise(1);
            largeBroadcaster.Event6.Raise(1);
            largeBroadcaster.Event7.Raise(1);
            largeBroadcaster.Event8.Raise(1);
            largeBroadcaster.Event9.Raise(1);
            largeBroadcaster.Event10.Raise(1);
            largeBroadcaster.Event11.Raise(1);
            largeBroadcaster.Event12.Raise(1);
            largeBroadcaster.Event13.Raise(1);
            largeBroadcaster.Event14.Raise(1);

            Assert.AreEqual(4, largeListener.Field0);
            Assert.AreEqual(4, largeListener.Field1);
            Assert.AreEqual(4, largeListener.Field2);
            Assert.AreEqual(4, largeListener.Field3);
            Assert.AreEqual(4, largeListener.Field4);
            Assert.AreEqual(4, largeListener.Field5);
            Assert.AreEqual(4, largeListener.Field6);
            Assert.AreEqual(4, largeListener.Field7);
            Assert.AreEqual(4, largeListener.Field8);
            Assert.AreEqual(4, largeListener.Field9);
            Assert.AreEqual(4, largeListener.Field10);
            Assert.AreEqual(4, largeListener.Field11);
            Assert.AreEqual(2, largeListener.Field12);
            Assert.AreEqual(2, largeListener.Field13);
            Assert.AreEqual(1, largeListener.Field14);

            yield break;
        }


        [UnityTest]
        public IEnumerator LargeScriptsAllResponsesCalled2() //same test for measurements
        {
            var (broadcaster, listener) = GetSubscriptedPair();

            var largeBroadcaster = broadcaster.GetComponent<LargeBroadcaster>();
            var largeListener = listener.GetComponent<LargeListener>();

            largeBroadcaster.Event0.Raise(1);
            largeBroadcaster.Event1.Raise(1);
            largeBroadcaster.Event2.Raise(1);
            largeBroadcaster.Event3.Raise(1);
            largeBroadcaster.Event4.Raise(1);
            largeBroadcaster.Event5.Raise(1);
            largeBroadcaster.Event6.Raise(1);
            largeBroadcaster.Event7.Raise(1);
            largeBroadcaster.Event8.Raise(1);
            largeBroadcaster.Event9.Raise(1);
            largeBroadcaster.Event10.Raise(1);
            largeBroadcaster.Event11.Raise(1);
            largeBroadcaster.Event12.Raise(1);
            largeBroadcaster.Event13.Raise(1);
            largeBroadcaster.Event14.Raise(1);

            Assert.AreEqual(4, largeListener.Field0);
            Assert.AreEqual(4, largeListener.Field1);
            Assert.AreEqual(4, largeListener.Field2);
            Assert.AreEqual(4, largeListener.Field3);
            Assert.AreEqual(4, largeListener.Field4);
            Assert.AreEqual(4, largeListener.Field5);
            Assert.AreEqual(4, largeListener.Field6);
            Assert.AreEqual(4, largeListener.Field7);
            Assert.AreEqual(4, largeListener.Field8);
            Assert.AreEqual(4, largeListener.Field9);
            Assert.AreEqual(4, largeListener.Field10);
            Assert.AreEqual(4, largeListener.Field11);
            Assert.AreEqual(2, largeListener.Field12);
            Assert.AreEqual(2, largeListener.Field13);
            Assert.AreEqual(1, largeListener.Field14);

            yield break;
        }


        private static (GameObject, GameObject) GetSubscriptedPair()
        {
            var broadcaster = new GameObject();
            broadcaster.SetActive(false);
            broadcaster.AddComponent<LargeBroadcaster>();
            broadcaster.AddComponent<EventLinker>();
            broadcaster.SetActive(true);

            var listener = new GameObject();
            listener.SetActive(false);
            listener.AddComponent<LargeListener>();
            listener.AddComponent<EventLinker>();
            listener.SetActive(true);

            var broadcasterLinker = broadcaster.GetComponent<EventLinker>();
            var listenerLinker = listener.GetComponent<EventLinker>();

            broadcasterLinker.StartBroadcastingTo(listener);

            return (broadcaster, listener);
        }
    }
}
