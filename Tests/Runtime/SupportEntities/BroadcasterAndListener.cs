using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class BroadcasterAndListener : MonoBehaviour
    {
        public int OneParamCall;

        [EventLocal(TestEventLabel.Label6)]
        public GameEvent<int> OneParamEvent;

        [ResponseLocal(TestEventLabel.Label6)]
        public void OneParamResponse(int arg) =>
            OneParamCall += arg;
    }
}
