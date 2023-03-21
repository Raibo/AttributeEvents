using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.EventLinks
{
    /// <summary>
    /// Links one event from broadcaster GameObject to one method from listener GameObject.
    /// </summary>
    [Serializable]
    public class EventLinkBase : IEquatable<EventLinkBase>
    {
        public readonly GameObject BroadcasterObject;
        public readonly GameObject ListenerObject;

        public bool IsActive => BroadcasterObject.activeInHierarchy && ListenerObject.activeInHierarchy;


        public EventLinkBase(GameObject broadcaster, GameObject listener)
        {
            BroadcasterObject = broadcaster;
            ListenerObject = listener;
        }


#if UNITY_EDITOR
        public string EditorDescription;
        public string EventComponentTypeName;
        public string ResponseComponentTypeName;
        public string EventFieldName;


        public void FillEditorDescription(FieldInfo eventField, MethodInfo responseMethod)
        {
            var paramsTypeNames = string.Join(", ", responseMethod.GetParameters().Select(pi => pi.ParameterType.Name));

            EditorDescription = $"{eventField.ReflectedType.Name}.{eventField.Name} =>" +
                $" {responseMethod.ReflectedType.Name}.{responseMethod.Name}({paramsTypeNames})";

            EventComponentTypeName = eventField.ReflectedType.Name;
            ResponseComponentTypeName = responseMethod.ReflectedType.Name;
            EventFieldName = eventField.Name;
        }
#endif


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


        public virtual void RegisterToEvent() { }
        public virtual void UnregisterFromEvent() { }

        public virtual Delegate GetDelegate() => null;


        public override bool Equals(object obj) =>
            obj is not null && obj is EventLinkBase other && Equals(other);


        public bool Equals(EventLinkBase other) =>
            other is not null && other.IsBetween(BroadcasterObject, ListenerObject) && GetDelegate().Equals(other.GetDelegate());


        public override int GetHashCode() =>
            HashCode.Combine(ListenerObject, BroadcasterObject, GetDelegate().Method);
    }
}
