using Hudossay.AttributeEvents.Assets.Runtime;
using Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime
{
    public class EventRaiseTests
    {
        [UnityTest]
        public IEnumerator AllEventLinkTypesCallsWithParameters()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            var broadcasterBeh = broadcaster.GetComponent<BroadcasterAllMatch>();
            var listenerBeh = listener.GetComponent<ListenerAllMatch>();

            broadcasterBeh.ParameterlessEvent.Raise();
            broadcasterBeh.OneParamEvent.Raise(1);
            broadcasterBeh.TwoParamEvent.Raise(2, 3);
            broadcasterBeh.ThreeParamEvent.Raise(4, 5, 6);
            broadcasterBeh.FourParamEvent.Raise(7, 8, 9, 10);

            Assert.AreEqual(1, listenerBeh.ParameterlessCall);
            Assert.AreEqual(1, listenerBeh.OneParamCall);
            Assert.AreEqual((2, 3), listenerBeh.TwoParamCall);
            Assert.AreEqual((4, 5, 6), listenerBeh.ThreeParamCall);
            Assert.AreEqual((7, 8, 9, 10), listenerBeh.FourParamCall);

            yield break;
        }

        [UnityTest]
        public IEnumerator AllEventLinkTypesCallsAfterReactivate()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            var broadcasterBeh = broadcaster.GetComponent<BroadcasterAllMatch>();
            var listenerBeh = listener.GetComponent<ListenerAllMatch>();

            listener.SetActive(false);
            listener.SetActive(true);

            broadcasterBeh.ParameterlessEvent.Raise();
            broadcasterBeh.OneParamEvent.Raise(1);
            broadcasterBeh.TwoParamEvent.Raise(2, 3);
            broadcasterBeh.ThreeParamEvent.Raise(4, 5, 6);
            broadcasterBeh.FourParamEvent.Raise(7, 8, 9, 10);

            Assert.AreEqual(1, listenerBeh.ParameterlessCall);
            Assert.AreEqual(1, listenerBeh.OneParamCall);
            Assert.AreEqual((2, 3), listenerBeh.TwoParamCall);
            Assert.AreEqual((4, 5, 6), listenerBeh.ThreeParamCall);
            Assert.AreEqual((7, 8, 9, 10), listenerBeh.FourParamCall);

            yield break;
        }

        [UnityTest]
        public IEnumerator NoInvokeWhenInactiveListener()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            var broadcasterBeh = broadcaster.GetComponent<BroadcasterAllMatch>();
            var listenerBeh = listener.GetComponent<ListenerAllMatch>();

            listener.SetActive(false);

            broadcasterBeh.ParameterlessEvent.Raise();
            broadcasterBeh.OneParamEvent.Raise(1);
            broadcasterBeh.TwoParamEvent.Raise(2, 3);
            broadcasterBeh.ThreeParamEvent.Raise(4, 5, 6);
            broadcasterBeh.FourParamEvent.Raise(7, 8, 9, 10);

            Assert.AreEqual(0, listenerBeh.ParameterlessCall);
            Assert.AreEqual(0, listenerBeh.OneParamCall);
            Assert.AreEqual((0, 0), listenerBeh.TwoParamCall);
            Assert.AreEqual((0, 0, 0), listenerBeh.ThreeParamCall);
            Assert.AreEqual((0, 0, 0, 0), listenerBeh.FourParamCall);

            yield break;
        }

        [UnityTest]
        public IEnumerator NoInvokeWhenInactiveBroadcaster()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            var broadcasterBeh = broadcaster.GetComponent<BroadcasterAllMatch>();
            var listenerBeh = listener.GetComponent<ListenerAllMatch>();

            broadcaster.SetActive(false);

            broadcasterBeh.ParameterlessEvent.Raise();
            broadcasterBeh.OneParamEvent.Raise(1);
            broadcasterBeh.TwoParamEvent.Raise(2, 3);
            broadcasterBeh.ThreeParamEvent.Raise(4, 5, 6);
            broadcasterBeh.FourParamEvent.Raise(7, 8, 9, 10);

            Assert.AreEqual(0, listenerBeh.ParameterlessCall);
            Assert.AreEqual(0, listenerBeh.OneParamCall);
            Assert.AreEqual((0, 0), listenerBeh.TwoParamCall);
            Assert.AreEqual((0, 0, 0), listenerBeh.ThreeParamCall);
            Assert.AreEqual((0, 0, 0, 0), listenerBeh.FourParamCall);

            yield break;
        }


        private static (GameObject, GameObject, EventLinker, EventLinker) GetSubscriptedPair()
        {
            var broadcaster = new GameObject();
            broadcaster.SetActive(false);
            broadcaster.AddComponent<BroadcasterAllMatch>();
            broadcaster.AddComponent<EventLinker>();
            broadcaster.SetActive(true);

            var listener = new GameObject();
            listener.SetActive(false);
            listener.AddComponent<ListenerAllMatch>();
            listener.AddComponent<EventLinker>();
            listener.SetActive(true);

            var broadcasterLinker = broadcaster.GetComponent<EventLinker>();
            var listenerLinker = listener.GetComponent<EventLinker>();

            broadcasterLinker.StartBroadcastingTo(listener);

            return (broadcaster, listener, broadcasterLinker, listenerLinker);
        }
    }
}
