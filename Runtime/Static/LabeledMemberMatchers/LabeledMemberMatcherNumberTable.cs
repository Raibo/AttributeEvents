using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;
using System;

namespace Hudossay.AttributeEvents.Assets.Runtime.Static.LabeledMemberMatchers
{
    internal class LabeledMemberMatcherNumberTable
    {
        public static LabeledMatch[] GetMatches(LabeledEvent[] labeledEvents, LabeledResponse[] labeledResponses)
        {
            var maxLength = Math.Max(Math.Max(labeledEvents.Length, labeledResponses.Length), MinMapSize);

            var maxEventHashCollisions = CountEventCollisions(labeledEvents, maxLength);
            var maxResponseHashCollisions = CountResponseCollisions(labeledResponses, maxLength);

            Span<short> eventByHashMap = stackalloc short[maxLength * maxEventHashCollisions];
            PopulateEventHashMap(labeledEvents, maxLength, maxEventHashCollisions, ref eventByHashMap);

            Span<short> responseByHashMap = stackalloc short[maxLength * maxResponseHashCollisions];
            PopulateResponseHashMap(labeledResponses, maxLength, maxResponseHashCollisions, ref responseByHashMap);

            short matchesCount = 0;
            Span<(short EventIndex, short ResponseIndex)> matches =
                stackalloc (short, short)[Math.Max(labeledEvents.Length, labeledResponses.Length)];

            for (int eventMapIndex = 0; eventMapIndex < eventByHashMap.Length; eventMapIndex++)
            {
                if (eventByHashMap[eventMapIndex] < 0)
                    continue;

                var hashIndex = eventMapIndex % maxLength;

                for (int collision = 0; collision < maxResponseHashCollisions; collision++)
                {
                    var responseMapIndex = GetMapIndex(hashIndex, collision, maxLength);

                    if (responseByHashMap[responseMapIndex] < 0)
                        break;

                    var labeledEventIndex = eventByHashMap[eventMapIndex];
                    var labeledEvent = labeledEvents[labeledEventIndex];

                    var labeledResponseIndex = responseByHashMap[responseMapIndex];
                    var labeledResponse = labeledResponses[labeledResponseIndex];

                    if (labeledEvent.IsMatchingTo(labeledResponse))
                    {
                        matches[matchesCount] = (labeledEventIndex, labeledResponseIndex);
                        matchesCount++;
                    }
                }
            }

            var matchArray = new LabeledMatch[matchesCount];

            for (int i = 0; i < matchesCount; i++)
            {
                var labeledEvent = labeledEvents[matches[i].EventIndex];
                var labeledResponse = labeledResponses[matches[i].ResponseIndex];

                matchArray[i] = new LabeledMatch(labeledEvent.EventLabel, labeledEvent, labeledResponse, labeledResponse.Args);
            }

            return matchArray;
        }

        private static int CountEventCollisions(LabeledEvent[] labeledEvents, int eventsLength)
        {
            Span<ushort> broadcasterLabelCounts = stackalloc ushort[eventsLength];
            broadcasterLabelCounts.Clear();
            var eventLabelCollisions = 0;

            for (int i = 0; i < labeledEvents.Length; i++)
            {
                var currentEvent = labeledEvents[i];
                var index = CalculateHashIndex(currentEvent.GetHashCode(), broadcasterLabelCounts.Length);
                broadcasterLabelCounts[index] += 1;
                eventLabelCollisions = Math.Max(eventLabelCollisions, broadcasterLabelCounts[index]);
            }

            return eventLabelCollisions;
        }

        private static int CountResponseCollisions(LabeledResponse[] labeledResponses, int responsesLength)
        {
            Span<ushort> listenerLabelCounts = stackalloc ushort[responsesLength];
            listenerLabelCounts.Clear();
            var responseLabelCollisions = 0;

            for (int i = 0; i < labeledResponses.Length; i++)
            {
                var currentEvent = labeledResponses[i];
                var index = CalculateHashIndex(currentEvent.GetHashCode(), listenerLabelCounts.Length);
                listenerLabelCounts[index] += 1;
                responseLabelCollisions = Math.Max(responseLabelCollisions, listenerLabelCounts[index]);
            }

            return responseLabelCollisions;
        }


        private static void PopulateEventHashMap(LabeledEvent[] labeledEvents, int eventsLength, int maxEventHashCollisions, ref Span<short> eventByLabelMap)
        {
            eventByLabelMap.Fill(-1);

            for (int eventIndex = 0; eventIndex < labeledEvents.Length; eventIndex++)
            {
                var currentEvent = labeledEvents[eventIndex];
                var hashIndex = CalculateHashIndex(currentEvent.GetHashCode(), eventsLength);

                for (int collisionCount = 0; collisionCount < maxEventHashCollisions; collisionCount++)
                {
                    var mapIndex = GetMapIndex(hashIndex, collisionCount, eventsLength);

                    if (eventByLabelMap[mapIndex] < 0)
                    {
                        eventByLabelMap[mapIndex] = (short)eventIndex;
                        break;
                    }
                }
            }
        }


        private static void PopulateResponseHashMap(LabeledResponse[] labeledResponses, int responsesLength, int maxResponseHashCollisions, ref Span<short> responseByLabelMap)
        {
            responseByLabelMap.Fill(-1);

            for (int eventIndex = 0; eventIndex < labeledResponses.Length; eventIndex++)
            {
                var currentResponse = labeledResponses[eventIndex];
                var hashIndex = CalculateHashIndex(currentResponse.GetHashCode(), responsesLength);

                for (int collisionCount = 0; collisionCount < maxResponseHashCollisions; collisionCount++)
                {
                    var mapIndex = GetMapIndex(hashIndex, collisionCount, responsesLength);

                    if (responseByLabelMap[mapIndex] < 0)
                    {
                        responseByLabelMap[mapIndex] = (short)eventIndex;
                        break;
                    }
                }
            }
        }


        private static int GetMapIndex(int hashIndex, int collisionCount, int length) =>
            hashIndex + length * collisionCount;


        private static short CalculateHashIndex(int hash, int length) =>
            (short)(Math.Abs(hash) % length);


        private const int MinMapSize = 13;
    }
}
