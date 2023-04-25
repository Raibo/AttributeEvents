using Hudossay.AttributeEvents.Assets.Runtime.GameEvents;
using System;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.EventLinks
{
    /// <summary>
    /// Links one event from broadcaster GameObject to one method from listener GameObject's MoonoBehaviour.
    /// </summary>
    public class EventLink<T1, T2, T3> : EventLinkBase
    {
        [NonSerialized]
        public readonly GameEvent<T1, T2, T3> Event;
        [NonSerialized]
        public readonly Action<T1, T2, T3> Response;


        public EventLink(MonoBehaviour broadcaster, MonoBehaviour listener, object gameEvent, object responseAction) : base(broadcaster, listener)
        {
            var eventCasted = gameEvent as GameEvent<T1, T2, T3>;
            var actionCasted = responseAction as Action<T1, T2, T3>;

#if UNITY_EDITOR
            if (eventCasted == null)
                Debug.Log($"argument object was not of type {nameof(GameEvent<T1, T2, T3>)}");
            if (actionCasted == null)
                Debug.Log($"argument object was not of type {nameof(Action<T1, T2, T3>)}");
#endif

            Event = eventCasted;
            Response = actionCasted;
        }


        public void Raise(T1 argument1, T2 argument2, T3 argument3)
        {
            if (Response != null && IsActive)
                Response.Invoke(argument1, argument2, argument3);
        }


        public void RaiseForced(T1 argument1, T2 argument2, T3 argument3)
        {
            if (Response != null)
                Response.Invoke(argument1, argument2, argument3);
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
