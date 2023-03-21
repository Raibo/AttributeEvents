using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure
{
    public readonly struct LabeledMono
    {
        public readonly Type MonoType;
        public readonly LabeledEvent[] LabeledEvents;
        public readonly LabeledResponse[] LabeledResponses;

        public readonly bool IsGlobalBroadcaster;
        public readonly bool IsGlobalListener;

        public readonly LabeledEvent[] LabeledGlobalEvents;
        public readonly LabeledResponse[] LabeledGlobalResponses;


        public LabeledMono(Type monoType)
        {
            MonoType = monoType;

            var events = monoType.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Select(fi => new LabeledEvent(fi))
                .Where(le => le.IsValid)
                .ToArray();

            var responses = monoType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(mi => new LabeledResponse(mi))
                .Where(lr => lr.IsValid)
                .ToArray();

            LabeledEvents = events.Where(e => !e.IsGlobal).ToArray();
            LabeledResponses = responses.Where(r => !r.IsGlobal).ToArray();

            if (events.Any(e => e.IsGlobal))
            {
                IsGlobalBroadcaster = true;
                LabeledGlobalEvents = events.Where(e => e.IsGlobal).ToArray();
            }
            else
            {
                IsGlobalBroadcaster = false;
                LabeledGlobalEvents = null;
            }

            if (responses.Any(r => r.IsGlobal))
            {
                IsGlobalListener = true;
                LabeledGlobalResponses = responses.Where(r => r.IsGlobal).ToArray();
            }
            else
            {
                IsGlobalListener = false;
                LabeledGlobalResponses = null;
            }
        }


        public void InitializeEvents(MonoBehaviour monoInstance)
        {
            foreach (var labeledEvent in LabeledEvents)
                labeledEvent.InitializeEvent(monoInstance);

            if (LabeledGlobalEvents is not null)
                foreach (var labeledEvent in LabeledGlobalEvents)
                    labeledEvent.InitializeEvent(monoInstance);
        }
    }
}
