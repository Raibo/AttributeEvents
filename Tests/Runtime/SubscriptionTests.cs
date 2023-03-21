using Hudossay.AttributeEvents.Assets.Runtime;
using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime
{
    public class SubscriptionTests
    {
        [UnityTest]
        public IEnumerator SubscriptionsWhenSubscribedExist()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            Assert.IsTrue(broadcasterLinker.Subscriptions.Any(s => s.ListenerObject == listener));

            yield break;
        }

        [UnityTest]
        public IEnumerator SubscriptionsWhenUnsubscribedNotExist()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            listenerLinker.StopListeningTo(broadcaster);
            Assert.IsFalse(broadcasterLinker.Subscriptions.Any(s => s.ListenerObject == listener));

            yield break;
        }

        [UnityTest]
        public IEnumerator MatchingLinksWhenSubscribedExist()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            Assert.AreEqual(broadcasterLinker.BroadcastedEventLinks.Count(l => l is EventLink), 1);
            Assert.AreEqual(broadcasterLinker.BroadcastedEventLinks.Count(l => l is EventLink<int, int, int>), 1);

            yield break;
        }

        [UnityTest]
        public IEnumerator MismatchingLinksWhenSubscribedNotExist()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            Assert.AreEqual(broadcasterLinker.BroadcastedEventLinks.Count(l => l is EventLink<int>), 0);
            Assert.AreEqual(broadcasterLinker.BroadcastedEventLinks.Count(l => l is EventLink<int, int>), 0);

            yield break;
        }

        [UnityTest]
        public IEnumerator OnlyMatchingLinksCalled()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            var broadcasterBeh = broadcaster.GetComponent<BroadcasterBehaviour>();
            var listenerBeh = listener.GetComponent<ListenerBehaviuor>();

            broadcasterBeh.ParameterlessEvent.Raise();
            broadcasterBeh.OneParamEvent.Raise(1);
            broadcasterBeh.TwoParamEvent.Raise(1, 2);
            broadcasterBeh.ThreeParamEvent.Raise(1, 2, 3);
            broadcasterBeh.FourParamEvent.Raise(1, 2, 3, 4);

            Assert.AreEqual(true, listenerBeh.ParameterlessCall);
            Assert.AreEqual(0, listenerBeh.OneParamCall);
            Assert.AreEqual((0, 0), listenerBeh.TwoParamCall);
            Assert.AreEqual((1, 2, 3), listenerBeh.ThreeParamCall);
            Assert.AreEqual((0, 0, 0, 0), listenerBeh.FourParamCall);

            yield break;
        }

        [UnityTest]
        public IEnumerator UnsibscribedLinksNotCalled()
        {
            var (broadcaster, listener, broadcasterLinker, listenerLinker) = GetSubscriptedPair();

            var broadcasterBeh = broadcaster.GetComponent<BroadcasterBehaviour>();
            var listenerBeh = listener.GetComponent<ListenerBehaviuor>();

            listenerLinker.StopListeningTo(broadcaster);

            broadcasterBeh.ParameterlessEvent.Raise();
            broadcasterBeh.OneParamEvent.Raise(1);
            broadcasterBeh.TwoParamEvent.Raise(1, 2);
            broadcasterBeh.ThreeParamEvent.Raise(1, 2, 3);
            broadcasterBeh.FourParamEvent.Raise(1, 2, 3, 4);

            Assert.AreEqual(false, listenerBeh.ParameterlessCall);
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
            broadcaster.AddComponent<BroadcasterBehaviour>();
            broadcaster.AddComponent<EventLinker>();
            broadcaster.SetActive(true);

            var listener = new GameObject();
            listener.SetActive(false);
            listener.AddComponent<ListenerBehaviuor>();
            listener.AddComponent<EventLinker>();
            listener.SetActive(true);

            var broadcasterLinker = broadcaster.GetComponent<EventLinker>();
            var listenerLinker = listener.GetComponent<EventLinker>();

            broadcasterLinker.StartBroadcastingTo(listener);

            return (broadcaster, listener, broadcasterLinker, listenerLinker);
        }
    }
}
