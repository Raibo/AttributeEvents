using Hudossay.AttributeEvents.Assets.Runtime;
using Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime
{
    public class ReverseCallTests
    {
        [UnityTest]
        public IEnumerator WhenBroadcasterActivatedAfterSubscriptionResponseCalled()
        {
            var (broadcaster, listener, _, _) = GetBroadcasterLateActivatedSubscriptedObjects();

            var broadcasterComponent = broadcaster.GetComponent<BroadcasterAndListener>();
            var listenerComponent = listener.GetComponent<BroadcasterAndListener>();

            broadcasterComponent.OneParamEvent.Raise(3);

            Assert.AreEqual(3, listenerComponent.OneParamCall);

            yield break;
        }


        [UnityTest]
        public IEnumerator WhenListenerActivatedAfterSubscriptionResponseCalled()
        {
            var (broadcaster, listener, _, _) = GetListenerLateActivatedSubscriptedObjects();

            var broadcasterComponent = broadcaster.GetComponent<BroadcasterAndListener>();
            var listenerComponent = listener.GetComponent<BroadcasterAndListener>();

            broadcasterComponent.OneParamEvent.Raise(3);

            Assert.AreEqual(3, listenerComponent.OneParamCall);

            yield break;
        }


        private static (GameObject, GameObject, EventLinker, EventLinker) GetBroadcasterLateActivatedSubscriptedObjects()
        {
            var broadcaster = new GameObject();
            broadcaster.SetActive(false);
            broadcaster.AddComponent<BroadcasterAndListener>();
            broadcaster.AddComponent<EventLinker>();

            var listener = new GameObject();
            listener.SetActive(false);
            listener.AddComponent<BroadcasterAndListener>();
            listener.AddComponent<EventLinker>();
            listener.SetActive(true);

            var broadcasterLinker = broadcaster.GetComponent<EventLinker>();
            var listenerLinker = listener.GetComponent<EventLinker>();

            listenerLinker.StartListeningTo(broadcaster);
            broadcaster.SetActive(true);

            return (broadcaster, listener, broadcasterLinker, listenerLinker);
        }


        private static (GameObject, GameObject, EventLinker, EventLinker) GetListenerLateActivatedSubscriptedObjects()
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

            var broadcasterLinker = broadcaster.GetComponent<EventLinker>();
            var listenerLinker = listener.GetComponent<EventLinker>();

            broadcasterLinker.StartBroadcastingTo(listener);
            listener.SetActive(true);

            return (broadcaster, listener, broadcasterLinker, listenerLinker);
        }
    }
}
