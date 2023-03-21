using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class BroadcasterBehaviour : MonoBehaviour
    {
        [EventLocal(TestEventLabel.Label1)]
        public GameEvent ParameterlessEvent;

        [EventLocal(TestEventLabel.Label3)]
        public GameEvent<int> OneParamEvent;

        [EventLocal(TestEventLabel.Label1)]
        public GameEvent<int, int> TwoParamEvent;

        [EventLocal(TestEventLabel.Label2)]
        public GameEvent<int, int, int> ThreeParamEvent;

        [EventLocal(TestEventLabel.Label2)]
        public GameEvent<int, int, int, int> FourParamEvent;
    }
}
