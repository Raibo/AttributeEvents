using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;
using System.Collections.Generic;
using System.Linq;

namespace Hudossay.AttributeEvents.Assets.Runtime.Static.LabeledMemberMatchers
{
    internal class LabeledMemberMatcherGarbage
    {
        public static LabeledMatch[] GetMatches(LabeledEvent[] labeledEvents, LabeledResponse[] labeledResponses)
        {
            // Here we get hashes of labeled events and responses. Hashes are based on label and args.
            // If event and response hashes don't match, then event and response won't connect anyway.
            // This is useful, because ahead is 2-level foreach and hashing can split this in smaller chunks
            var eventsMap = new Dictionary<int, List<LabeledEvent>>();
            var responsesMap = new Dictionary<int, List<LabeledResponse>>();

            foreach (var labeledEvent in labeledEvents)
            {
                var eventHash = labeledEvent.GetHashCode();

                if (!eventsMap.ContainsKey(eventHash))
                    eventsMap[eventHash] = new List<LabeledEvent>();

                eventsMap[eventHash].Add(labeledEvent);
            }

            foreach (var labeledResponse in labeledResponses)
            {
                var responseHash = labeledResponse.GetHashCode();

                if (!responsesMap.ContainsKey(responseHash))
                    responsesMap[responseHash] = new List<LabeledResponse>();

                responsesMap[responseHash].Add(labeledResponse);
            }

            var matches = new List<LabeledMatch>();

            foreach (var labelAndArgsHash in eventsMap.Keys.Intersect(responsesMap.Keys))
                foreach (var labeledEvent in eventsMap[labelAndArgsHash])
                    foreach (var labeledResponse in responsesMap[labelAndArgsHash])
                    {
                        if (labeledEvent.IsMatchingTo(labeledResponse))
                            matches.Add(new LabeledMatch(labeledEvent.EventLabel, labeledEvent, labeledResponse, labeledResponse.Args));
                    }

            return matches.ToArray();
        }
    }
}
