using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using System;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure
{
    public readonly struct LabeledMatch
    {
        public readonly object EventLabel;
        public readonly LabeledEvent LabeledEvent;
        public readonly LabeledResponse LabeledResponse;
        public readonly Type[] Args;


        public LabeledMatch(object eventLabel, LabeledEvent labeledEvent, LabeledResponse labeledResponse, Type[] args)
        {
            EventLabel = eventLabel;
            LabeledEvent = labeledEvent;
            LabeledResponse = labeledResponse;
            Args = args;
        }


        public EventLinkBase Connect(MonoBehaviour broadcaster, MonoBehaviour listener)
        {
            if (Args.Length == 0)
                return ConnectParameterless(broadcaster, listener);

            return ConnectParametered(broadcaster, listener);
        }


        private EventLinkBase ConnectParameterless(MonoBehaviour broadcaster, MonoBehaviour listener)
        {
            var eventValueInMono = LabeledEvent.GetEventFieldValue(broadcaster);
            var responseDelegate = LabeledResponse.GetDelegate(listener);

            if (eventValueInMono == null)
            {
                Debug.LogError($"GameObject's '{broadcaster?.gameObject?.name}' GameEvent {LabeledEvent.FieldInfo.ReflectedType.Name}" +
                    $".{LabeledEvent.FieldInfo.Name} was null during EventLink generation");
                return null;
            }

            var newLink = new EventLink(broadcaster, listener, eventValueInMono, responseDelegate);
            newLink.RegisterToEvent();

#if UNITY_EDITOR
            newLink.FillEditorDescription(LabeledEvent.FieldInfo, LabeledResponse.ResponseMethod);
#endif
            return newLink;
        }


        private EventLinkBase ConnectParametered(MonoBehaviour broadcasterMono, MonoBehaviour listenerMono)
        {
            var eventLinkTypeOpen = Args.Length switch
            {
                1 => typeof(EventLink<>),
                2 => typeof(EventLink<,>),
                3 => typeof(EventLink<,,>),
                4 => typeof(EventLink<,,,>),
                _ => throw new NotSupportedException($"Generic event link with {Args.Length} type arguments is not supperted"),
            };

            var eventLinkType = eventLinkTypeOpen.MakeGenericType(Args);

            var eventValueInMono = LabeledEvent.GetEventFieldValue(broadcasterMono);
            var responseDelegate = LabeledResponse.GetDelegate(listenerMono);

            if (eventValueInMono == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"GameObject's '{broadcasterMono?.gameObject?.name}' GameEvent<> {LabeledEvent.FieldInfo.ReflectedType.Name}" +
                    $".{LabeledEvent.FieldInfo.Name} was null during EventLink generation");
#endif
                return null;
            }

            _linkCtorParams[0] = broadcasterMono;
            _linkCtorParams[1] = listenerMono;
            _linkCtorParams[2] = eventValueInMono;
            _linkCtorParams[3] = responseDelegate;

            var newLink = eventLinkType.GetConstructor(_linkCtorTypes).Invoke(_linkCtorParams) as EventLinkBase;
            newLink.RegisterToEvent();

#if UNITY_EDITOR
            newLink.FillEditorDescription(LabeledEvent.FieldInfo, LabeledResponse.ResponseMethod);
#endif
            return newLink;
        }


        private static readonly Type[] _linkCtorTypes =
        {
            typeof(MonoBehaviour),
            typeof(MonoBehaviour),
            typeof(object),
            typeof(object),
        };

        private static readonly object[] _linkCtorParams = new object[4];
    }
}
