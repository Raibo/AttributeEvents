using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using System.Collections.Generic;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.GameEvents
{
    public class GameEvent<T1> : GameEventBase
    {
        public List<EventLink<T1>> Links = new List<EventLink<T1>>();


        public void Raise(T1 argument1)
        {
            for (int i = Links.Count - 1; i >= 0; i--)
                Links[i].Raise(argument1);
        }


        public void RaiseForced(T1 argument1)
        {
            for (int i = Links.Count - 1; i >= 0; i--)
                Links[i].RaiseForced(argument1);
        }


        public void RegisterListener(EventLink<T1> listener)
        {
            if (!Links.Contains(listener))
                Links.Add(listener);
        }


        public void UnregisterListener(EventLink<T1> listener)
        {
            if (Links.Contains(listener))
                Links.Remove(listener);
        }


        public override void RemoveLinksToGameObject(GameObject gameObject) =>
            Links.RemoveAll(l => l.ListenerObject == gameObject);
    }
}
