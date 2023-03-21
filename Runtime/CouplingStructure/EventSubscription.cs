using System;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure
{
    [Serializable]
    public class EventSubscription
    {
        // These fields should be readonly, but if they were, then their values couldn't be saved during hot recompilation.
        public GameObject BroadcasterObject;
        public GameObject ListenerObject;


        public EventSubscription(GameObject broadcasterObject, GameObject listenerObject)
        {
            BroadcasterObject = broadcasterObject;
            ListenerObject = listenerObject;
        }


        public bool IsBetween(GameObject broadcaster, GameObject listener)
        {
            // We assume that all of 4 gameobjects here are enabled, not destroyed, not null
            var myBroadcasterHash = BroadcasterObject.GetHashCode();
            var otherBroadcasterHash = broadcaster.GetHashCode();

            var myListenerHash = ListenerObject.GetHashCode();
            var otherListenerHash = listener.GetHashCode();

            // We can be sure that hashes equality is enough since UnityEngine.Object.GetHashCode returns m_InstanceID
            return (myBroadcasterHash ^ otherBroadcasterHash) == (myListenerHash ^ otherListenerHash)
                && myBroadcasterHash == otherBroadcasterHash
                && myListenerHash == otherListenerHash;
        }


        public bool Equals(EventSubscription other) =>
            BroadcasterObject.Equals(other.BroadcasterObject) && ListenerObject.Equals(other.ListenerObject);


        public override bool Equals(object obj)
        {
            var otherSubscription = obj as EventSubscription;

            if (otherSubscription == null)
                return false;

            return Equals(otherSubscription);
        }


        public override int GetHashCode() =>
            BroadcasterObject.GetHashCode() ^ ListenerObject.GetHashCode();


        public static bool operator ==(EventSubscription left, EventSubscription right) =>
            Equals(left, right);


        public static bool operator !=(EventSubscription left, EventSubscription right) =>
            !Equals(left, right);


        public override string ToString() =>
            $"{BroadcasterObject.name} => {ListenerObject.name}";
    }
}
