using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;
using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
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

                    var pair = LabeledMonoPair.FromLabeledMono(labeledBroadcaster, labeledListener);
                    eventLinks.AddRange(pair.Connect(broadcasterMonoInstance, listenerMonoInstance));
                }

            return eventLinks;
        }
    }
}
