using Hudossay.AttributeEvents.Assets.Runtime;
using Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime
{
    public class GlobalSubscriptionTests
    {
        // In this tests game objects are not destroyed after test has finished, so we also check that
        // additional global objects do not interfere with each other.

        [UnityTest]
        public IEnumerator GlobalMatchingResponseCalled()
        {
            var (broadcaster, listener) = GetGlobalEventObjectsPair();

            var broadcasterMono = broadcaster.GetComponent<GlobalBroadcaster>();
            var listenerMono = listener.GetComponent<GlobalListener>();

            broadcasterMono.GlobalEvent.Raise();
            broadcasterMono.GlobalEventInt.Raise(2);

            Assert.AreEqual(1, listenerMono.NoParamCalls);
            Assert.AreEqual(2, listenerMono.IntParamCalls);

            yield break;
        }


        [UnityTest]
        public IEnumerator GlobalMatchingResponseCalledInMultipleListeners()
        {
            var (broadcaster, listener) = GetGlobalEventObjectsPair();

            var listener2 = new GameObject();
            listener2.name = $"Global Listener Additional";
            listener2.SetActive(false);
            listener2.AddComponent<GlobalListener>();
            listener2.AddComponent<EventLinker>();
            listener2.SetActive(true);

            var broadcasterMono = broadcaster.GetComponent<GlobalBroadcaster>();
            var listenerMono = listener.GetComponent<GlobalListener>();
            var listener2Mono = listener2.GetComponent<GlobalListener>();


            broadcasterMono.GlobalEvent.Raise();
            broadcasterMono.GlobalEventInt.Raise(2);

            Assert.AreEqual(1, listenerMono.NoParamCalls);
            Assert.AreEqual(2, listenerMono.IntParamCalls);

            Assert.AreEqual(1, listener2Mono.NoParamCalls);
            Assert.AreEqual(2, listener2Mono.IntParamCalls);

            yield break;
        }


        [UnityTest]
        public IEnumerator GlobalSameObjectResponseNotCalled()
        {
            var (broadcaster, listener) = GetGlobalEventObjectsPair();

            var broadcasterMono = broadcaster.GetComponent<GlobalBroadcaster>();
            var listenerMono = listener.GetComponent<GlobalListener>();

            broadcasterMono.GlobalEventForSameObject.Raise();

            Assert.AreEqual(0, broadcasterMono.SameMonoCalls);

            yield break;
        }


        [UnityTest]
        public IEnumerator GlobalMatchingResponseCalledWhenListenerCreatedFirst()
        {
            var (broadcaster, listener) = GetGlobalEventObjectsPairReversed();

            var broadcasterMono = broadcaster.GetComponent<GlobalBroadcaster>();
            var listenerMono = listener.GetComponent<GlobalListener>();

            broadcasterMono.GlobalEvent.Raise();
            broadcasterMono.GlobalEventInt.Raise(2);

            Assert.AreEqual(1, listenerMono.NoParamCalls);
            Assert.AreEqual(2, listenerMono.IntParamCalls);

            yield break;
        }


        [UnityTest]
        public IEnumerator GlobalNotMatchingResponseNotCalled()
        {
            var (broadcaster, listener) = GetGlobalEventObjectsPair();

            var broadcasterMono = broadcaster.GetComponent<GlobalBroadcaster>();
            var listenerMono = listener.GetComponent<GlobalListener>();

            broadcasterMono.GlobalEventNotConnected.Raise(2, 2);

            Assert.AreEqual(0, listenerMono.NotConnectedCalls);

            yield break;
        }


        [UnityTest]
        public IEnumerator AfterDestroyListenerNoLinksInEvents()
        {
            var (broadcaster, listener) = GetGlobalEventObjectsPair();

            var broadcasterMono = broadcaster.GetComponent<GlobalBroadcaster>();
            var listenerMono = listener.GetComponent<GlobalListener>();

            var listenerInstanceId = listener.GetHashCode();
            Object.Destroy(listener);

            yield return null;  // We have to skip a frame after Destroy

            Assert.AreEqual(0, broadcasterMono.GlobalEvent.Links.Count(l => l.ListenerObject.GetHashCode() == listenerInstanceId));
            Assert.AreEqual(0, broadcasterMono.GlobalEventForSameObject.Links.Count(l => l.ListenerObject.GetHashCode() == listenerInstanceId));
            Assert.AreEqual(0, broadcasterMono.GlobalEventInt.Links.Count(l => l.ListenerObject.GetHashCode() == listenerInstanceId));
            Assert.AreEqual(0, broadcasterMono.GlobalEventNotConnected.Links.Count(l => l.ListenerObject.GetHashCode() == listenerInstanceId));

            yield break;
        }


        [UnityTest]
        public IEnumerator AfterDestroyListenerNoLinksInEventLinker()
        {
            var (broadcaster, listener) = GetGlobalEventObjectsPair();

            var broadcasterMono = broadcaster.GetComponent<GlobalBroadcaster>();
            var broadcasterLinker = broadcaster.GetComponent<EventLinker>();
            var listenerMono = listener.GetComponent<GlobalListener>();

            var listenerInstanceId = listener.GetHashCode();
            Object.Destroy(listener);

            yield return null;  // We have to skip a frame after Destroy

            foreach (var linkSet in broadcasterLinker.MyGlobalBroadcastLinks)
                Assert.AreEqual(0, broadcasterMono.GlobalEvent.Links.Count(l => l.ListenerObject.GetHashCode() == listenerInstanceId));

            yield break;
        }


        private static (GameObject broadcaster, GameObject listener) GetGlobalEventObjectsPair()
        {
            var broadcaster = new GameObject();
            broadcaster.name = $"Global Broadcaster {_objectsCreateCount}";
            broadcaster.SetActive(false);
            broadcaster.AddComponent<GlobalBroadcaster>();
            broadcaster.AddComponent<EventLinker>();
            broadcaster.SetActive(true);

            var listener = new GameObject();
            listener.name = $"Global Listener {_objectsCreateCount}";
            listener.SetActive(false);
            listener.AddComponent<GlobalListener>();
            listener.AddComponent<EventLinker>();
            listener.SetActive(true);

            _objectsCreateCount++;

            return (broadcaster, listener);
        }


        private static (GameObject broadcaster, GameObject listener) GetGlobalEventObjectsPairReversed()
        {
            var listener = new GameObject();
            listener.name = $"Global Listener {_objectsCreateCount}";
            listener.SetActive(false);
            listener.AddComponent<GlobalListener>();
            listener.AddComponent<EventLinker>();
            listener.SetActive(true);

            var broadcaster = new GameObject();
            broadcaster.name = $"Global Broadcaster {_objectsCreateCount}";
            broadcaster.SetActive(false);
            broadcaster.AddComponent<GlobalBroadcaster>();
            broadcaster.AddComponent<EventLinker>();
            broadcaster.SetActive(true);

            _objectsCreateCount++;

            return (broadcaster, listener);
        }

        private static int _objectsCreateCount;
    }
}
