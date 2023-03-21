using Hudossay.AttributeEvents.Assets.Runtime;
using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class BroadcasterAllMatch : MonoBehaviour
    {
        public GameObject LestenerReference;
        public EventLinker EventLinker;

        [EventLocal(TestEventLabel.Label5)]
        public GameEvent ParameterlessEvent;

        [EventLocal(TestEventLabel.Label5)]
        public GameEvent<int> OneParamEvent;

        [EventLocal(TestEventLabel.Label5)]
        public GameEvent<int, int> TwoParamEvent;

        [EventLocal(TestEventLabel.Label5)]
        public GameEvent<int, int, int> ThreeParamEvent;

        [EventLocal(TestEventLabel.Label5)]
        public GameEvent<int, int, int, int> FourParamEvent;


        [ContextMenu("CallParameterlessEvent")]
        public void CallGlobalEvent()
        {
            ParameterlessEvent.Raise();
        }


        [ContextMenu("CallOneParamEvent1")]
        public void CallGlobalEventInt1()
        {
            OneParamEvent.Raise(1);
        }


        [ContextMenu("StartBroadcasting")]
        public void StartBroadcasting()
        {
            EventLinker.StartBroadcastingTo(LestenerReference);
        }
    }
}
