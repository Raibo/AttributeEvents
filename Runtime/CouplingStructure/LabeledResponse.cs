using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.CouplingStructure
{
    public readonly struct LabeledResponse
    {
        public readonly object EventLabel;
        public readonly MethodInfo ResponseMethod;
        public readonly Type[] Args;
        public readonly bool IsGlobal;
        public readonly bool IsValid;

        private readonly int _labeledMemberHash;


        public LabeledResponse(MethodInfo methodInfo)
        {
            var labelAttribute = methodInfo.GetCustomAttribute<ResponseAttribute>();

            if (labelAttribute is null)
            {
                IsValid = false;

                EventLabel = null;
                ResponseMethod = methodInfo;
                Args = null;
                _labeledMemberHash = 0;
                IsGlobal = false;

                return;
            }

            IsGlobal = labelAttribute is ResponseGlobalAttribute;

            EventLabel = labelAttribute.Label;
            ResponseMethod = methodInfo;
            Args = methodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
            IsValid = true;
            _labeledMemberHash = 0;
            _labeledMemberHash = GetLabelArgsHash();
        }


        public Delegate GetDelegate(MonoBehaviour monoBehaviour)
        {
            if (Args.Length == 0)
                return ResponseMethod.CreateDelegate(typeof(Action), monoBehaviour);

            var unityActionOpenedType = Args.Length switch
            {
                1 => typeof(Action<>),
                2 => typeof(Action<,>),
                3 => typeof(Action<,,>),
                4 => typeof(Action<,,,>),
                _ => throw new NotSupportedException($"Responses with more than 4 type arguments are not supperted. (Received {Args.Length})"),
            };

            var unityActionType = unityActionOpenedType.MakeGenericType(Args);
            return ResponseMethod.CreateDelegate(unityActionType, monoBehaviour);
        }


        public override int GetHashCode() =>
            _labeledMemberHash;


        private int GetLabelArgsHash() =>
            (EventLabel, EventLabel.GetType(), ((IStructuralEquatable)Args).GetHashCode(EqualityComparer<Type>.Default)).GetHashCode();


        public override bool Equals(object obj) =>
            obj is not null && obj is LabeledResponse other && Equals(other);


        public bool Equals(LabeledResponse other) =>
            Equals(EventLabel, other.EventLabel) && IsGlobal == other.IsGlobal && ResponseMethod == other.ResponseMethod;


        public override string ToString() =>
            Args.Length == 0
                ? $"[{EventLabel}] {ResponseMethod.DeclaringType.Name}.{ResponseMethod.Name}()"
                : $"[{EventLabel}] {ResponseMethod.DeclaringType.Name}.{ResponseMethod.Name}({string.Join(", ", Args.Select(t => t.Name))})";
    }
}
