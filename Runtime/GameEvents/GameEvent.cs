using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using System.Collections.Generic;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.GameEvents
{
    public class GameEvent : GameEventBase
    {
        public List<EventLink> Links = new List<EventLink>();


        public void Raise()
        {
            for (int i = Links.Count - 1; i >= 0; i--)
                Links[i].Raise();
        }


        public void AddLink(EventLink listener)
        {
            if (!Links.Contains(listener))
                Links.Add(listener);
        }


        public void RemoveLink(EventLink listener)
        {
            if (Links.Contains(listener))
                Links.Remove(listener);
        }


        public override void RemoveLinksToGameObject(GameObject gameObject) =>
            Links.RemoveAll(l => l.ListenerObject == gameObject);
    }
}
