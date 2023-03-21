using Hudossay.AttributeEvents.Assets.Runtime;
using Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime
{
    public class SelfCallTests
    {
        [UnityTest]
        public IEnumerator SameComponentDifferentObjectResponseCalled()
        {
            var (broadcaster, listener, _, _) = GetSubscriptedPair();

            var broadcasterComponent = broadcaster.GetComponent<BroadcasterAndListener>();
            var listenerComponent = listener.GetComponent<BroadcasterAndListener>();

            broadcasterComponent.OneParamEvent.Raise(3);

            Assert.AreEqual(3, listenerComponent.OneParamCall);

            yield break;
        }

        [UnityTest]
        public IEnumerator SameComponentSameObjectResponseNotCalled()
        {
            var (_, component) = GetWithComponents<BroadcasterAndListener>();

            component.OneParamEvent.Raise(3);

            Assert.AreEqual(0, component.OneParamCall);

            yield break;
        }

        [UnityTest]
        public IEnumerator DifferentComponentSameObjectResponseCalled()
        {
            var (_, broadcasterComponent, listenerComponent) = GetWithComponents<BroadcasterAllMatch, ListenerAllMatch>();

            broadcasterComponent.OneParamEvent.Raise(3);

            Assert.AreEqual(3, listenerComponent.OneParamCall);

            yield break;
        }


        private static (GameObject, GameObject, EventLinker, EventLinker) GetSubscriptedPair()
        {
            var broadcaster = new GameObject();
            broadcaster.SetActive(false);
            broadcaster.AddComponent<BroadcasterAndListener>();
            broadcaster.AddComponent<EventLinker>();
            broadcaster.SetActive(true);

            var listener = new GameObject();
            listener.SetActive(false);
            listener.AddComponent<BroadcasterAndListener>();
            listener.AddComponent<EventLinker>();
            listener.SetActive(true);

            var broadcasterLinker = broadcaster.GetComponent<EventLinker>();
            var listenerLinker = listener.GetComponent<EventLinker>();

            broadcasterLinker.StartBroadcastingTo(listener);

            return (broadcaster, listener, broadcasterLinker, listenerLinker);
        }

        private static (GameObject, T) GetWithComponents<T>() where T : Component
        {
            var obj = new GameObject();
            obj.SetActive(false);
            obj.AddComponent<T>();
            obj.AddComponent<EventLinker>();
            obj.SetActive(true);

            return (obj, obj.GetComponent<T>());
        }

        private static (GameObject, T1, T2) GetWithComponents<T1, T2>() where T1 : Component where T2 : Component
        {
            var obj = new GameObject();
            obj.SetActive(false);
            obj.AddComponent<T1>();
            obj.AddComponent<T2>();
            obj.AddComponent<EventLinker>();
            obj.SetActive(true);

            return (obj, obj.GetComponent<T1>(), obj.GetComponent<T2>());
        }
    }
}
