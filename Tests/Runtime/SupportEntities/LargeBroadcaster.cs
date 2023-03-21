using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class LargeBroadcaster : MonoBehaviour
    {
        [EventLocal(TestEventLabel.Label1)]
        public GameEvent<int> Event0;

        [EventLocal(TestEventLabel.Label1)]
        public GameEvent<int> Event1;

        [EventLocal(TestEventLabel.Label1)]
        public GameEvent<int> Event2;

        [EventLocal(TestEventLabel.Label1)]
        public GameEvent<int> Event3;

        [EventLocal(TestEventLabel.Label2)]
        public GameEvent<int> Event4;

        [EventLocal(TestEventLabel.Label2)]
        public GameEvent<int> Event5;

        [EventLocal(TestEventLabel.Label2)]
        public GameEvent<int> Event6;

        [EventLocal(TestEventLabel.Label2)]
        public GameEvent<int> Event7;

        [EventLocal(TestEventLabel.Label3)]
        public GameEvent<int> Event8;

        [EventLocal(TestEventLabel.Label3)]
        public GameEvent<int> Event9;

        [EventLocal(TestEventLabel.Label3)]
        public GameEvent<int> Event10;

        [EventLocal(TestEventLabel.Label3)]
        public GameEvent<int> Event11;

        [EventLocal(TestEventLabel.Label4)]
        public GameEvent<int> Event12;

        [EventLocal(TestEventLabel.Label4)]
        public GameEvent<int> Event13;

        [EventLocal(TestEventLabel.Label5)]
        public GameEvent<int> Event14;
    }
}
