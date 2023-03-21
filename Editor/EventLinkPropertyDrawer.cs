using Hudossay.AttributeEvents.Assets.Runtime.EventLinks;
using UnityEditor;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Editor
{
    [CustomPropertyDrawer(typeof(EventLinkBase), true)]
    public class EventLinkPropertyDrawer : PropertyDrawer
    {
        private const string BroadcasterLabelText = "";
        private const string ListenerLabelText = "=>";
        private const int AdditionalVerticalSpace = 10;
        private const int TypicalFieldHeight = 17;

        private GUIStyle _textStyle;
        private Rect _lastPosition;


        public EventLinkPropertyDrawer()
        {
            _textStyle = GUI.skin.textField;
            _textStyle.wordWrap = true;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var desc = property.FindPropertyRelative(nameof(EventLinkBase.EditorDescription));
            var descText = desc?.stringValue;
            var descTextHeight = _textStyle.CalcHeight(new GUIContent(descText), _lastPosition.width);

            return TypicalFieldHeight + descTextHeight + 1 + AdditionalVerticalSpace;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _lastPosition = position;

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

            var objectFieldWidth = (position.width - listenerLabelSize.x - broadcasterLabelSize.x) / 2;

            var listenerObj = (property.OriginalValue() as EventLinkBase)?.ListenerObject;
            var broadcesterObj = (property.OriginalValue() as EventLinkBase)?.BroadcasterObject;

            var desc = property.FindPropertyRelative(nameof(EventLinkBase.EditorDescription));
            var descText = desc.stringValue;
            var descTextHeight = _textStyle.CalcHeight(new GUIContent(descText), position.width);

            var label1Rect = new Rect(position.x, position.y, broadcasterLabelSize.x, TypicalFieldHeight);
            var field1Rect = new Rect(position.x + broadcasterLabelSize.x, position.y, objectFieldWidth, TypicalFieldHeight);
            var label2Rect = new Rect(position.x + broadcasterLabelSize.x + objectFieldWidth, position.y, listenerLabelSize.x, TypicalFieldHeight);
            var field2Rect = new Rect(position.x + broadcasterLabelSize.x + objectFieldWidth + listenerLabelSize.x, position.y, objectFieldWidth, TypicalFieldHeight);

            var descRect = new Rect(position.x, position.y + TypicalFieldHeight + 1, position.width, descTextHeight);

            EditorGUI.LabelField(label1Rect, broadcasterLabel);
            EditorGUI.ObjectField(field1Rect, broadcesterObj, typeof(GameObject), true);

            EditorGUI.LabelField(label2Rect, listenerLabel);
            EditorGUI.ObjectField(field2Rect, listenerObj, typeof(GameObject), true);

            EditorGUI.TextField(descRect, descText, _textStyle);

            EditorGUI.EndProperty();
        }
    }
}
