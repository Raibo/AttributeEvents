using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;
using System;
using System.Buffers;

namespace Hudossay.AttributeEvents.Assets.Runtime.Static.LabeledMemberMatchers
{
    internal class LabeledMemberMatcher
    {
        public unsafe static int GetMatches(LabeledEvent[] labeledEvents, LabeledResponse[] labeledResponses,
            LabeledMatch[] resultBuffer)
        {
            var maxLength = Math.Max(Math.Max(labeledEvents.Length, labeledResponses.Length), MinMapSize);
            var totalNodesRequired = labeledEvents.Length + labeledResponses.Length + maxLength + maxLength;

            var demandedNodesMemory = sizeof(Node) * totalNodesRequired;
            var canFitNodesInStack = demandedNodesMemory <= MaxStackNodesMemory;

            Node[] sharedBuffer = canFitNodesInStack ? null : ArrayPool<Node>.Shared.Rent(totalNodesRequired);

            fixed (Node* sharedBufferPtr = canFitNodesInStack ? null : sharedBuffer)
            {
                Span<Node> allNodeSpan =
                    canFitNodesInStack ? stackalloc Node[totalNodesRequired] : new Span<Node>(sharedBuffer, 0, totalNodesRequired);

                var defaultNode = new Node { Index = -1 };

                // Span.Clear is slow for some reason
                for (int i = 0; i < allNodeSpan.Length; i++)
                    allNodeSpan[i] = defaultNode;

                Span<Node> spareEventNodes = allNodeSpan.Slice(0, labeledEvents.Length);
                Span<Node> spareResponseNodes = allNodeSpan.Slice(labeledEvents.Length, labeledResponses.Length);

                Span<Node> eventsHashLinkedMap = allNodeSpan.Slice(labeledEvents.Length + labeledResponses.Length, maxLength);
                Span<Node> responsesHashLinkedMap = allNodeSpan.Slice(labeledEvents.Length + labeledResponses.Length + maxLength, maxLength);

                int currentSpareEventNode = 0;
                int currentSpareResponseNode = 0;

                PopulateEventsMap(ref eventsHashLinkedMap, labeledEvents, maxLength, ref spareEventNodes, ref currentSpareEventNode);
                PopulateResponseMap(ref responsesHashLinkedMap, labeledResponses, maxLength, ref spareResponseNodes, ref currentSpareResponseNode);

                var matchesCount = FindMatches(labeledEvents, labeledResponses, maxLength, eventsHashLinkedMap, responsesHashLinkedMap,
                    ref _matchesBuffer);

                for (int i = 0; i < matchesCount; i++)
                {
                    var labeledEvent = labeledEvents[_matchesBuffer[i].EventIndex];
                    var labeledResponse = labeledResponses[_matchesBuffer[i].ResponseIndex];

                    resultBuffer[i] = new LabeledMatch(labeledEvent.EventLabel, labeledEvent, labeledResponse, labeledResponse.Args);
                }

                if (!canFitNodesInStack)
                    ArrayPool<Node>.Shared.Return(sharedBuffer);

                return matchesCount;
            }
        }


        private static void PopulateEventsMap(ref Span<Node> eventsHashLinkedMap, LabeledEvent[] labeledEvents, int maxLength,
            ref Span<Node> spareEventNodes, ref int currentSpareEventNode)
        {
            for (short elementIndex = 0; elementIndex < labeledEvents.Length; elementIndex++)
            {
                var element = labeledEvents[elementIndex];
                var hashIndex = CalculateHashIndex(element.GetHashCode(), maxLength);

                ref Node node = ref eventsHashLinkedMap[hashIndex];
                node.Append(elementIndex, ref spareEventNodes, ref currentSpareEventNode);
            }
        }


        private static void PopulateResponseMap(ref Span<Node> responseHashLinkedMap, LabeledResponse[] labeledResponses, int maxLength,
            ref Span<Node> spareEventNodes, ref int currentSpareResponseNode)
        {
            for (short elementIndex = 0; elementIndex < labeledResponses.Length; elementIndex++)
            {
                var element = labeledResponses[elementIndex];
                var hashIndex = CalculateHashIndex(element.GetHashCode(), maxLength);

                ref Node node = ref responseHashLinkedMap[hashIndex];
                node.Append(elementIndex, ref spareEventNodes, ref currentSpareResponseNode);
            }
        }


        private static short CalculateHashIndex(int hash, int length) =>
            (short)(Math.Abs(hash) % length);


        private static unsafe short FindMatches(LabeledEvent[] labeledEvents, LabeledResponse[] labeledResponses, int maxLength,
            Span<Node> eventsHashLinkedMap, Span<Node> responsesHashLinkedMap, ref (byte, byte)[] matchesBuffer)
        {
            short currentMatchIndex = 0;
            Node* eventNodePtr;
            Node* responseNodePtr;

            for (int hashMapIndex = 0; hashMapIndex < maxLength; hashMapIndex++)
            {
                fixed (Node* fixedEventNodePtr = &eventsHashLinkedMap[hashMapIndex])
                    eventNodePtr = fixedEventNodePtr;

                while (eventNodePtr is not null && eventNodePtr->Index >= 0)
                {
                    fixed (Node* fixedResponseNodePtr = &responsesHashLinkedMap[hashMapIndex])
                        responseNodePtr = fixedResponseNodePtr;

                    while (responseNodePtr is not null && responseNodePtr->Index >= 0)
                    {
                        ref var labeledEvent = ref labeledEvents[eventNodePtr->Index];
                        ref var labeledResponse = ref labeledResponses[responseNodePtr->Index];

                        if (labeledEvent.IsMatchingTo(labeledResponse))
                        {
                            matchesBuffer[currentMatchIndex] = ((byte)eventNodePtr->Index, (byte)responseNodePtr->Index);
                            currentMatchIndex++;
                        }

                        responseNodePtr = responseNodePtr->Next;
                    }

                    eventNodePtr = eventNodePtr->Next;
                }
            }

            return currentMatchIndex;
        }


        private unsafe struct Node
        {
            public short Index;
            public Node* Next;


            public void Append(short value, ref Span<Node> spareEventNodes, ref int currentSpareEventNode)
            {
                if (Index < 0)
                {
                    Index = value;
                    return;
                }

                ref Node nextNode = ref spareEventNodes[currentSpareEventNode];

                if (Next is not null)
                    nextNode = ref *Next;
                else
                    currentSpareEventNode++;

                fixed (Node* nextNodePtr = &nextNode)
                    Next = nextNodePtr;

                nextNode.Append(value, ref spareEventNodes, ref currentSpareEventNode);
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                if (Index < 0)
                    return string.Empty;

                var str = Index.ToString();

                if (Next is not null)
                    str += $" -> {Next->ToString()}";

                return str;
            }
#endif
        }


        private const int MinMapSize = 17;
        private const int MaxStackNodesMemory = 1024;
        private const int MaxMatches = 256;

        private static (byte EventIndex, byte ResponseIndex)[] _matchesBuffer = new (byte, byte)[MaxMatches];
    }
}
