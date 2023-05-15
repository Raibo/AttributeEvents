using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;
using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using Hudossay.AttributeEvents.Assets.Runtime.Static.LabeledMemberMatchers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.Static
{
    internal static class GameObjectLinker
    {
        public static List<EventLinkBase> Connect(GameObject broadcaster, GameObject listener,
            Dictionary<Type, LabeledMono> LabeledMonos)
        {
            var broadcasterMonoInstances = broadcaster.GetComponents<MonoBehaviour>()
                .Where(i => i != null);

            var listenerMonoInstances = listener.GetComponents<MonoBehaviour>()
                .Where(i => i != null);

            var eventLinks = new List<EventLinkBase>();

            foreach (var broadcasterMonoInstance in broadcasterMonoInstances)
                foreach (var listenerMonoInstance in listenerMonoInstances)
                {
                    var labeledBroadcaster = LabeledMonos[broadcasterMonoInstance.GetType()];
                    var labeledListener = LabeledMonos[listenerMonoInstance.GetType()];

                    if (!labeledBroadcaster.LabeledEvents.Any() || !labeledListener.LabeledResponses.Any())
                        continue;

                    // Do not link events and responses from the same MonoBehaviour
                    if (broadcasterMonoInstance == listenerMonoInstance)
                        continue;

                    var matchesCount = ConnectMonoPair(labeledBroadcaster, labeledListener, broadcasterMonoInstance, listenerMonoInstance,
                        _connectLinksBuffer);

                    for (int matchIndex = 0; matchIndex < matchesCount; matchIndex++)
                        eventLinks.Add(_connectLinksBuffer[matchIndex]);
                }

            return eventLinks;
        }


        private static int ConnectMonoPair(LabeledMono labeledBroadcaster, LabeledMono labeledListener,
            MonoBehaviour broadcasterInstance, MonoBehaviour listenerInstance, EventLinkBase[] linksBuffer)
        {
            var matchesCount = LabeledMemberMatcher.GetMatches(labeledBroadcaster.LabeledEvents, labeledListener.LabeledResponses,
                _labeledMatchesBuffer);

            for (int matchIndex = 0; matchIndex < matchesCount; matchIndex++)
                linksBuffer[matchIndex] = _labeledMatchesBuffer[matchIndex].Connect(broadcasterInstance, listenerInstance);

            return matchesCount;
        }


        private const int MaxMatches = 256;
        private static readonly LabeledMatch[] _labeledMatchesBuffer = new LabeledMatch[MaxMatches];
        private static readonly EventLinkBase[] _connectLinksBuffer = new EventLinkBase[MaxMatches];
    }
}
