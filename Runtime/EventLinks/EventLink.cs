using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using System;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.EventLinks
{
    /// <summary>
    /// Links one event from broadcaster GameObject to one method from listener GameObject's MoonoBehaviour.
    /// </summary>
    public class EventLink : EventLinkBase
    {
        [NonSerialized]
        public readonly GameEvent Event;
        [NonSerialized]
        public readonly Action Response;


        public EventLink(MonoBehaviour broadcaster, MonoBehaviour listener, object gameEvent, object responseAction) : base(broadcaster, listener)
        {
            var eventCasted = gameEvent as GameEvent;
            var actionCasted = responseAction as Action;

#if UNITY_EDITOR
            if (eventCasted == null)
                Debug.Log($"argument object was not of type {nameof(GameEvent)}");
            if (actionCasted == null)
                Debug.Log($"argument object was not of type {nameof(Action)}");
#endif

            Event = eventCasted;
            Response = actionCasted;
        }


        public void Raise()
        {
            if (Response != null && IsActive)
                Response.Invoke();
        }


        public void RaiseForced()
        {
            if (Response != null)
                Response.Invoke();
        }


        public override void RegisterToEvent()
        {
            if (Event != null)
                Event.AddLink(this);
        }


        public override void UnregisterFromEvent()
        {
            if (Event != null)
                Event.RemoveLink(this);
        }


        public override Delegate GetDelegate() => Response;
    }
}
