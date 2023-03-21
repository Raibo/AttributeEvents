#if UNITY_EDITOR
using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.Static
{
    internal static class GlobalLinksReportBuilder
    {
        public static string GetGlobalLinksReport(HashSet<EventLinker> globalEventLinkers)
        {
            List<EventLinkBase> links = new();

            foreach (var linker in globalEventLinkers)
                foreach (var linkSet in linker.MyGlobalBroadcastLinks.Values)
                    links.AddRange(linkSet);

            return FormatReport(links);
        }


        public static string GetAllLocalLinksReport()
        {
            var eventLinkers = Object.FindObjectsOfType<EventLinker>(true);

            List<EventLinkBase> links = new();

            foreach (var linker in eventLinkers)
                links.AddRange(linker.BroadcastedEventLinks);

            return FormatReport(links);
        }


        private static string FormatReport(IEnumerable<EventLinkBase> links)
        {
            var reportLines = links.Select(l => new
            {
                BroadcasterName = l.BroadcasterObject.name,
                ListenerName = l.ListenerObject.name,
                EventMonoTypeName = l.EventComponentTypeName,
                ResponseMonoTypeName = l.ResponseComponentTypeName,
                EventFieldName = l.EventFieldName,
                ResponseMethodName = l.GetDelegate().Method.Name,
                ArgsString = string.Join(",", l.GetDelegate().Method.GetParameters().Select(p => p.ParameterType.Name)),
                Description = l.EditorDescription,
            });

            var reportLinesList = reportLines
                .OrderBy(l => l.BroadcasterName)
                .ThenBy(l => l.ListenerName)
                .ThenBy(l => l.Description)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Broadcaster Object\tBroadcaster Component\tEvent\tListener Object\tListener Component\tCalled Method\tArgs");

            foreach (var line in reportLinesList)
                sb.AppendLine($"{line.BroadcasterName}\t{line.EventMonoTypeName}\t{line.EventFieldName}\t" +
                    $"{line.ListenerName}\t{line.ResponseMonoTypeName}\t{line.ResponseMethodName}\t({line.ArgsString})");

            return sb.ToString();
        }
    }
}
#endif
