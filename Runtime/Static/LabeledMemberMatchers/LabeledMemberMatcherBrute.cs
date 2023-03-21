using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;

namespace Hudossay.AttributeEvents.Assets.Runtime.Static.LabeledMemberMatchers
{
    internal class LabeledMemberMatcherBrute
    {
        public static LabeledMatch[] GetMatches(LabeledEvent[] labeledEvents, LabeledResponse[] labeledResponses)
        {
            int matchesCount = 0;

            for (int eventIndex = 0; eventIndex < labeledEvents.Length; eventIndex++)
            {
                for (int responseIndex = 0; responseIndex < labeledResponses.Length; responseIndex++)
                {
                    ref var labeledEvent = ref labeledEvents[eventIndex];
                    ref var labeledResponse = ref labeledResponses[responseIndex];

                    if (labeledEvent.IsMatchingTo(labeledResponse))
                    {
                        _matchesBuffer[matchesCount] = ((byte)eventIndex, (byte)responseIndex);
                        matchesCount++;
                    }
                }
            }

            var matchArray = new LabeledMatch[matchesCount];

            for (int i = 0; i < matchesCount; i++)
            {
                var labeledEvent = labeledEvents[_matchesBuffer[i].EventIndex];
                var labeledResponse = labeledResponses[_matchesBuffer[i].ResponseIndex];

                matchArray[i] = new LabeledMatch(labeledEvent.EventLabel, labeledEvent, labeledResponse, labeledResponse.Args);
            }

            return matchArray;
        }


        private const int MaxMatches = 100;

        private static readonly (byte EventIndex, byte ResponseIndex)[] _matchesBuffer = new (byte, byte)[MaxMatches];
    }
}
