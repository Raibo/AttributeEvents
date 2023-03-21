using Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure;
using System;
using UnityEditor;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Editor
{
    [CustomPropertyDrawer(typeof(EventSubscription), true)]
    public class EventSubscriptionPropertyDrawer : PropertyDrawer
    {
        private const int MinWidth = 50;
        private const int TypicalFieldHeight = 17;

        private const string ListenerLabelText = "L";
        private const string BroadcasterLabelText = "B";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelStyle = GUI.skin.label;
            var listenerLabel = new GUIContent(ListenerLabelText);
            var broadcasterLabel = new GUIContent(BroadcasterLabelText);

            var broadcasterLabelSize = labelStyle.CalcSize(broadcasterLabel);
            var listenerLabelSize = labelStyle.CalcSize(listenerLabel);

            if (string.IsNullOrWhiteSpace(BroadcasterLabelText))
                broadcasterLabelSize.x = 0;

            if (string.IsNullOrWhiteSpace(ListenerLabelText))
                listenerLabelSize.x = 0;

            var objectFieldWidth = Math.Max(MinWidth, (position.width - listenerLabelSize.x - broadcasterLabelSize.x) / 2);

            var listenerObj = (property.OriginalValue() as EventSubscription)?.ListenerObject;
            var broadcesterObj = (property.OriginalValue() as EventSubscription)?.BroadcasterObject;

            var label1Rect = new Rect(position.x, position.y, broadcasterLabelSize.x, TypicalFieldHeight);
            var field1Rect = new Rect(position.x + broadcasterLabelSize.x, position.y, objectFieldWidth, TypicalFieldHeight);
            var label2Rect = new Rect(position.x + broadcasterLabelSize.x + objectFieldWidth, position.y, listenerLabelSize.x, TypicalFieldHeight);
            var field2Rect = new Rect(position.x + broadcasterLabelSize.x + objectFieldWidth + listenerLabelSize.x, position.y, objectFieldWidth, TypicalFieldHeight);

            EditorGUI.LabelField(label1Rect, broadcasterLabel);
            EditorGUI.ObjectField(field1Rect, broadcesterObj, typeof(GameObject), true);

            EditorGUI.LabelField(label2Rect, listenerLabel);
            EditorGUI.ObjectField(field2Rect, listenerObj, typeof(GameObject), true);

            EditorGUI.EndProperty();
        }
    }
}
