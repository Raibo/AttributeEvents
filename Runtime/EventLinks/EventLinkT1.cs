using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using System;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.EventLinks
{
    /// <summary>
    /// Links one event from broadcaster GameObject to one method from listener GameObject's MoonoBehaviour.
    /// </summary>
    public class EventLink<T1> : EventLinkBase
    {
        [NonSerialized]
        public readonly GameEvent<T1> Event;
        [NonSerialized]
        public readonly Action<T1> Response;


        public EventLink(MonoBehaviour broadcaster, MonoBehaviour listener, object gameEvent, object responseAction) : base(broadcaster, listener)
        {
            var eventCasted = gameEvent as GameEvent<T1>;
            var actionCasted = responseAction as Action<T1>;

#if UNITY_EDITOR
            if (eventCasted == null)
                Debug.Log($"argument object was not of type {nameof(GameEvent<T1>)}");
            if (actionCasted == null)
                Debug.Log($"argument object was not of type {nameof(Action<T1>)}");
#endif

            Event = eventCasted;
            Response = actionCasted;
        }


        public void Raise(T1 argument1)
        {
            if (Response != null && IsActive)
                Response.Invoke(argument1);
        }


        public void RaiseForced(T1 argument1)
        {
            if (Response != null)
                Response.Invoke(argument1);
        }


        public override void RegisterToEvent()
        {
            if (Event != null)
                Event.RegisterListener(this);
        }


        public override void UnregisterFromEvent()
        {
            if (Event != null)
                Event.UnregisterListener(this);
        }


        public override Delegate GetDelegate() => Response;
    }
}
