using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class GlobalBroadcaster : MonoBehaviour
    {
        public int SameMonoCalls;

        [EventGlobal(TestEventLabel.Label1)]
        public GameEvent GlobalEvent;

        [EventGlobal(TestEventLabel.Label2)]
        public GameEvent<int> GlobalEventInt;

        [EventGlobal(TestEventLabel.Label3)]
        public GameEvent<int, int> GlobalEventNotConnected;

        [EventGlobal(TestEventLabel.Label5)]
        public GameEvent GlobalEventForSameObject;


        [ResponseGlobal(TestEventLabel.Label5)]
        public void GlobalResponseInSameMono() => SameMonoCalls++;


        [ContextMenu("CallGlobalEvent")]
        public void CallGlobalEvent()
        {
            GlobalEvent.Raise();
        }


        [ContextMenu("CallGlobalEventInt1")]
        public void CallGlobalEventInt1()
        {
            GlobalEventInt.Raise(1);
        }
    }
}
