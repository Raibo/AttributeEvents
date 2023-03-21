using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using Hudossay.AttributeEvents.Assets.Runtime.Static.LabeledMemberMatchers;
using System.Collections.Generic;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure
{
    public struct LabeledMonoPair
    {
        public LabeledMono Broadcaster;
        public LabeledMono Listener;

        public LabeledMatch[] LabeledMatches;


        public static LabeledMonoPair FromLabeledMono(LabeledMono broadcaster, LabeledMono listener)
        {
            var pair = new LabeledMonoPair
            {
                Broadcaster = broadcaster,
                Listener = listener,
            };

            pair.LabeledMatches = LabeledMemberMatcherLinkedList.GetMatches(broadcaster.LabeledEvents, listener.LabeledResponses);
            return pair;
        }


        public List<EventLinkBase> Connect(MonoBehaviour broadcasterInstance, MonoBehaviour listenerInstance)
        {
            var links = new List<EventLinkBase>();

            foreach (LabeledMatch match in LabeledMatches)
                links.Add(match.Connect(broadcasterInstance, listenerInstance));

            return links;
        }
    }
}
