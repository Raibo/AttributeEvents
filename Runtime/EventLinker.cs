using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;
using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using Hudossay.AttributeEvents.Assets.Runtime.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime
{
    public class EventLinker : MonoBehaviour
    {
        public List<EventLinkBase> BroadcastedEventLinks;
#if UNITY_EDITOR
        public List<EventLinkBase> ListenedEventLinks;
#endif
        public List<EventSubscription> Subscriptions;

        public Dictionary<LabeledEvent, HashSet<EventLinkBase>> MyGlobalBroadcastLinks;

        public static Dictionary<Type, LabeledMono> LabeledMonos;
        public static Dictionary<LabeledEvent, HashSet<MonoBehaviour>> GlobalBroadcastingMonos;
        public static Dictionary<LabeledResponse, HashSet<MonoBehaviour>> GlobalListeningMonos;

        public static HashSet<EventLinker> GlobalBroadcastingEventLinkers;

        // These fields should have [NonSerialized] attribute so that their values are lost during hot recompilation.
        // Othervise, it would be impossible to recover after the recompilation.
        [NonSerialized] private bool _areLabelsMapped;
        [NonSerialized] private bool _areGameObjectEventsInitialized;
        [NonSerialized] private bool _areGlobalConnectionsInitialized;
#if UNITY_EDITOR
        [NonSerialized] private bool _withoutScriptRecompilation;
#endif


        private void Awake()
        {
            BroadcastedEventLinks ??= new();
            Subscriptions ??= new();

            MyGlobalBroadcastLinks ??= new();
            GlobalBroadcastingMonos ??= new();
            GlobalListeningMonos ??= new();
            GlobalBroadcastingEventLinkers ??= new();
#if UNITY_EDITOR
            ListenedEventLinks ??= new();
            _withoutScriptRecompilation = true;
#endif
        }


        void OnEnable()
        {
            LabelGameObjectMonos();
            InitializeGameObjectEvents();
#if UNITY_EDITOR
            RestoreBrokenLinks();
#endif
            StartListeningTo(gameObject);
            ConnectGlobal();
        }


        private void OnDisable()
        {
            _areGameObjectEventsInitialized = false;
        }


        private void OnDestroy()
        {
            DisconnectMyLinks();
            DisconnectGlobal();
        }


        public void StartListeningTo(GameObject obj)
        {
            if (obj == null)
                return;

            Connect(obj, gameObject);
        }


        public void StartBroadcastingTo(GameObject obj)
        {
            if (obj == null)
                return;

            Connect(gameObject, obj);
        }


        public void StopListeningTo(GameObject obj) =>
            Disconnect(obj, gameObject);


        public void StopBroadcastingTo(GameObject obj) =>
            Disconnect(gameObject, obj);


        public void InitializeGameObjectEvents()
        {
            if (_areGameObjectEventsInitialized)
                return;

            var monoInstances = gameObject.GetComponents<MonoBehaviour>()
                .Where(i => i != null);

            foreach (var monoInstance in monoInstances)
            {
                var monoType = monoInstance.GetType();
                if (!LabeledMonos.TryGetValue(monoType, out var labeledMonoType))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"Could not find {nameof(labeledMonoType)} for {monoType.Name} to initialize events");
#endif
                    continue;
                }

                labeledMonoType.InitializeEvents(monoInstance);
            }

            _areGameObjectEventsInitialized = true;
        }


        public void LabelGameObjectMonos()
        {
            if (_areLabelsMapped)
                return;

            LabeledMonos ??= new Dictionary<Type, LabeledMono>();

            var monoInstances = gameObject.GetComponents<MonoBehaviour>();

            var monoTypes = monoInstances
                .Where(i => i != null)
                .Select(instance => instance.GetType());

            var newMonoTypes = monoTypes
                .Where(t => !LabeledMonos.ContainsKey(t));

            foreach (var monoType in newMonoTypes)
                LabeledMonos[monoType] = new LabeledMono(monoType);

            _areLabelsMapped = true;
        }


        public void DisconnectGlobalFromGameObject(GameObject gameObject)
        {
            if (MyGlobalBroadcastLinks is null)
                return;

            foreach (var linkSet in MyGlobalBroadcastLinks.Values)
            {
                foreach (var link in linkSet.Where(l => l.ListenerObject == gameObject))
                    link.UnregisterFromEvent();

                linkSet.RemoveWhere(l => l.ListenerObject == gameObject);
            }
        }


        public void AddGlobalLinkForEvent(EventLinkBase link, LabeledEvent evt)
        {
            if (!MyGlobalBroadcastLinks.ContainsKey(evt))
                MyGlobalBroadcastLinks[evt] = new HashSet<EventLinkBase>();

            MyGlobalBroadcastLinks[evt].Add(link);
        }


        private void Connect(GameObject broadcaster, GameObject listener)
        {
            if (Subscriptions.Any(s => s.IsBetween(broadcaster, listener)))
                return;

            var broadcasterLinker = GetEventLinker(broadcaster);
            var listenerLinker = GetEventLinker(listener);

            if (broadcasterLinker == null || listenerLinker == null)
                return;

            broadcasterLinker.LabelGameObjectMonos();
            listenerLinker.LabelGameObjectMonos();

            if (!broadcasterLinker._areGameObjectEventsInitialized)
                broadcasterLinker.InitializeGameObjectEvents();

            var links = GameObjectLinker.Connect(broadcaster, listener, LabeledMonos);

            broadcasterLinker.BroadcastedEventLinks ??= new();
            broadcasterLinker.BroadcastedEventLinks.AddRange(links);

            broadcasterLinker.Subscriptions ??= new();
            broadcasterLinker.Subscriptions.Add(new EventSubscription(broadcaster, listener));

#if UNITY_EDITOR
            listenerLinker.ListenedEventLinks ??= new();
            listenerLinker.ListenedEventLinks.AddRange(links);
#endif

            if (broadcaster == listener)
                return;

            listenerLinker.Subscriptions ??= new();
            listenerLinker.Subscriptions.Add(new EventSubscription(broadcaster, listener));
        }


        private void Disconnect(GameObject broadcaster, GameObject listener)
        {
            var broadcasterLinker = GetEventLinker(broadcaster);
            var listenerLinker = GetEventLinker(listener);

#if UNITY_EDITOR
            // For hot recompilation
            broadcasterLinker?.BroadcastedEventLinks.RemoveAll(l => l.BroadcasterObject is null || l.ListenerObject is null);
            listenerLinker?.ListenedEventLinks.RemoveAll(l => l.BroadcasterObject is null || l.ListenerObject is null);
#endif

            var linksToRemove = broadcasterLinker?.BroadcastedEventLinks.Where(l => l.IsBetween(broadcaster, listener)) ??
                Enumerable.Empty<EventLinkBase>();

            foreach (var link in linksToRemove)
                link.UnregisterFromEvent();

            broadcasterLinker?.BroadcastedEventLinks.RemoveAll(l => l.IsBetween(broadcaster, listener));
            broadcasterLinker?.Subscriptions.RemoveAll(s => s.IsBetween(broadcaster, listener));

#if UNITY_EDITOR
            listenerLinker?.ListenedEventLinks.RemoveAll(l => l.IsBetween(broadcaster, listener));
#endif
            listenerLinker?.Subscriptions.RemoveAll(s => s.IsBetween(broadcaster, listener));
        }


        private EventLinker GetEventLinker(GameObject obj)
        {
            if (obj == null)
                return null;

            if (obj == gameObject)
                return this;

            return obj.GetComponent<EventLinker>();
        }


        private void DisconnectMyLinks()
        {
            for (int i = Subscriptions.Count - 1; i >= 0; i--)
                Disconnect(Subscriptions[i].BroadcasterObject, Subscriptions[i].ListenerObject);
        }


        private void ConnectGlobal()
        {
            if (_areGlobalConnectionsInitialized)
                return;

            var monoInstances = GetComponents<MonoBehaviour>();

            foreach (var monoInstance in monoInstances)
            {
                var labeledMono = LabeledMonos[monoInstance.GetType()];

                if (labeledMono.IsGlobalBroadcaster)
                {
                    GlobalBroadcastingEventLinkers.Add(this);
                    AddGlobalBroadcasterMono(ref labeledMono, monoInstance);
                }

                if (labeledMono.IsGlobalListener)
                    AddGlobalListenerMono(ref labeledMono, monoInstance);
            }

            _areGlobalConnectionsInitialized = true;
        }


        private void AddGlobalBroadcasterMono(ref LabeledMono labeledMono, MonoBehaviour monoInstance)
        {
            foreach (var globalEvent in labeledMono.LabeledGlobalEvents)
                AddGlobalEvent(globalEvent, monoInstance);
        }


        private void AddGlobalListenerMono(ref LabeledMono labeledMono, MonoBehaviour monoInstance)
        {
            foreach (var globalEvent in labeledMono.LabeledGlobalResponses)
                AddGlobalResponse(globalEvent, monoInstance);
        }

        private void AddGlobalEvent(LabeledEvent globalEvent, MonoBehaviour monoInstance)
        {
            if (GlobalBroadcastingMonos.ContainsKey(globalEvent) && GlobalBroadcastingMonos[globalEvent].Contains(monoInstance))
                return;

            if (!GlobalBroadcastingMonos.ContainsKey(globalEvent))
                GlobalBroadcastingMonos[globalEvent] = new HashSet<MonoBehaviour>();

            GlobalBroadcastingMonos[globalEvent].Add(monoInstance);

            var hash = globalEvent.GetHashCode();
            var matchingResponses = GlobalListeningMonos.Keys.Where(k => k.GetHashCode() == hash && globalEvent.IsMatchingTo(k));

            foreach (var matchingResponse in matchingResponses)
            {
                var match = new LabeledMatch(globalEvent.EventLabel, globalEvent, matchingResponse, globalEvent.Args);

                foreach (var responseMonoInstance in GlobalListeningMonos[matchingResponse])
                {
                    // No same GameObjects connections
                    if (monoInstance == responseMonoInstance)
                        continue;

                    // No duplication connections
                    if (MyGlobalBroadcastLinks.ContainsKey(globalEvent) &&
                        MyGlobalBroadcastLinks[globalEvent]?.Any(l => l.ListenerObject == responseMonoInstance) == true)
                        continue;

                    var link = match.Connect(monoInstance, responseMonoInstance);
                    AddGlobalLinkForEvent(link, globalEvent);
                }
            }
        }


        private void AddGlobalResponse(LabeledResponse globalResponse, MonoBehaviour monoInstance)
        {
            if (GlobalListeningMonos.ContainsKey(globalResponse) && GlobalListeningMonos[globalResponse].Contains(monoInstance))
                return;

            if (!GlobalListeningMonos.ContainsKey(globalResponse))
                GlobalListeningMonos[globalResponse] = new HashSet<MonoBehaviour>();

            GlobalListeningMonos[globalResponse].Add(monoInstance);

            var hash = globalResponse.GetHashCode();
            var matchingEvents = GlobalBroadcastingMonos.Keys.Where(k => k.GetHashCode() == hash && k.IsMatchingTo(globalResponse));

            foreach (var matchingEvent in matchingEvents)
            {
                var match = new LabeledMatch(globalResponse.EventLabel, matchingEvent, globalResponse, globalResponse.Args);

                foreach (var eventMonoInstance in GlobalBroadcastingMonos[matchingEvent])
                {
                    // No same GameObjects connections
                    if (eventMonoInstance == monoInstance)
                        continue;

                    // No duplication connections
                    var broadcastingLinkerGlobalLinks = GetEventLinker(eventMonoInstance.gameObject).MyGlobalBroadcastLinks;

                    if (broadcastingLinkerGlobalLinks.ContainsKey(matchingEvent) &&
                        broadcastingLinkerGlobalLinks[matchingEvent]?.Any(l => l.ListenerObject == gameObject) == true)
                        continue;

                    var link = match.Connect(eventMonoInstance, monoInstance);
                    GetEventLinker(eventMonoInstance.gameObject).AddGlobalLinkForEvent(link, matchingEvent);
                }
            }
        }


        private void DisconnectGlobal()
        {
            var monoInstances = GetComponents<MonoBehaviour>();

            foreach (var monoInstance in monoInstances)
            {
                var labeledMono = LabeledMonos[monoInstance.GetType()];

                if (labeledMono.IsGlobalBroadcaster)
                    RemoveGlobalBroadcasterMono(ref labeledMono, monoInstance);

                if (labeledMono.IsGlobalListener)
                    RemoveGlobalListenerMono(ref labeledMono, monoInstance);
            }

            // Disconnect all links
            if (MyGlobalBroadcastLinks is not null)
            {
                foreach (var linkSet in MyGlobalBroadcastLinks.Values)
                    foreach (var link in linkSet)
                        link.UnregisterFromEvent();

                MyGlobalBroadcastLinks.Clear();
            }

            foreach (var globalBroadcastingLinker in GlobalBroadcastingEventLinkers)
                globalBroadcastingLinker.DisconnectGlobalFromGameObject(gameObject);

            GlobalBroadcastingEventLinkers.Remove(this);
        }


        private void RemoveGlobalBroadcasterMono(ref LabeledMono labeledMono, MonoBehaviour monoInstance)
        {
            foreach (var globalEvent in labeledMono.LabeledGlobalEvents)
                RemoveGlobalEvent(globalEvent, monoInstance);
        }


        private void RemoveGlobalListenerMono(ref LabeledMono labeledMono, MonoBehaviour monoInstance)
        {
            foreach (var globalEvent in labeledMono.LabeledGlobalResponses)
                RemoveGlobalResponse(globalEvent, monoInstance);
        }


        private void RemoveGlobalEvent(LabeledEvent globalEvent, MonoBehaviour monoInstance)
        {
            if (GlobalBroadcastingMonos.TryGetValue(globalEvent, out var setForEvent))
                setForEvent.Remove(monoInstance);
        }


        private void RemoveGlobalResponse(LabeledResponse globalResponse, MonoBehaviour monoInstance)
        {
            if (GlobalListeningMonos.TryGetValue(globalResponse, out var setForResponse))
                setForResponse.Remove(monoInstance);
        }


#if UNITY_EDITOR
        private void RestoreBrokenLinks()
        {
            if (_withoutScriptRecompilation)
                return;

            MyGlobalBroadcastLinks ??= new();
            GlobalBroadcastingMonos ??= new();
            GlobalListeningMonos ??= new();
            GlobalBroadcastingEventLinkers ??= new();

            ConnectGlobal();

            var subs = Subscriptions.ToList();

            foreach (var sub in subs)
            {
                Disconnect(sub.BroadcasterObject, sub.ListenerObject);
                Connect(sub.BroadcasterObject, sub.ListenerObject);
            }

            _withoutScriptRecompilation = true;
        }


        [ContextMenu("CopyGlobalLinksReport")]
        public void CopyGlobalLinksReport() =>
            GUIUtility.systemCopyBuffer = GlobalLinksReportBuilder.GetGlobalLinksReport(GlobalBroadcastingEventLinkers);


        [ContextMenu("CopyLocalLinksReport")]
        public void CopyLocalLinksReport() =>
            GUIUtility.systemCopyBuffer = GlobalLinksReportBuilder.GetAllLocalLinksReport();
#endif
    }
}
