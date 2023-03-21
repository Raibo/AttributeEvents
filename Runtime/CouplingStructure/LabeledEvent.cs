using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure
{
    public readonly struct LabeledEvent
    {
        public readonly object EventLabel;
        public readonly FieldInfo FieldInfo;
        public readonly Type[] Args;
        public readonly bool IsGlobal;
        public readonly bool IsValid;

        private readonly int _labeledMemberHash;


        public LabeledEvent(FieldInfo fieldInfo)
        {
            var labelAttribute = fieldInfo.GetCustomAttribute<EventAttribute>();

            if (labelAttribute is null)
            {
                IsValid = false;

                EventLabel = null;
                FieldInfo = fieldInfo;
                Args = null;
                _labeledMemberHash = 0;
                IsGlobal = false;

                return;
            }

            var args = fieldInfo.FieldType.IsGenericType
                ? fieldInfo.FieldType.GetGenericArguments()
                : Array.Empty<Type>();

            IsGlobal = labelAttribute is EventGlobalAttribute;

            EventLabel = labelAttribute.Label;
            FieldInfo = fieldInfo;
            Args = args;
            IsValid = true;
            _labeledMemberHash = 0;
            _labeledMemberHash = GetLabelArgsHash();
        }


        public bool IsMatchingTo(LabeledResponse labeledResponse) =>
            Equals(EventLabel, labeledResponse.EventLabel) && Args.SequenceEqual(labeledResponse.Args);


        public object GetEventFieldValue(MonoBehaviour monoBehaviour) =>
            FieldInfo.GetValue(monoBehaviour);


        public void InitializeEvent(MonoBehaviour monoInstance)
        {
            var eventValue = GetEventFieldValue(monoInstance);

            if (eventValue is not null)
                return;

            var constructor = FieldInfo.FieldType.GetConstructor(Array.Empty<Type>());

            if (constructor is null)
            {
#if UNITY_EDITOR
                Debug.LogError($"Parameterless constructor for GameEvent type {FieldInfo.ReflectedType.Name} could not be found");
#endif
                return;
            }

            FieldInfo.SetValue(monoInstance, constructor.Invoke(Array.Empty<object>()));
        }


        public override int GetHashCode() =>
            _labeledMemberHash;


        private int GetLabelArgsHash() =>
            (EventLabel, EventLabel.GetType(), ((IStructuralEquatable)Args).GetHashCode(EqualityComparer<Type>.Default)).GetHashCode();


        public override bool Equals(object obj) =>
            obj is not null && obj is LabeledEvent other && Equals(other);


        public bool Equals(LabeledEvent other) =>
            Equals(EventLabel, other.EventLabel) && IsGlobal == other.IsGlobal && FieldInfo == other.FieldInfo;


        public override string ToString() =>
            Args.Length == 0
                ? $"[{EventLabel}] {FieldInfo.FieldType.Name} {FieldInfo.ReflectedType.Name}.{FieldInfo.Name}"
                : $"[{EventLabel}] {FieldInfo.FieldType.Name}<{string.Join(", ", Args.Select(t => t.Name))}> {FieldInfo.ReflectedType.Name}.{FieldInfo.Name}";
    }
}
