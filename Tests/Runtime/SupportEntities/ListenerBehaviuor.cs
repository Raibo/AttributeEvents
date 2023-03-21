using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class ListenerBehaviuor : MonoBehaviour
    {
        public bool ParameterlessCall;
        public int OneParamCall;
        public (int, int) TwoParamCall;
        public (int, int, int) ThreeParamCall;
        public (int, int, int, int) FourParamCall;

        [ResponseLocal(TestEventLabel.Label9)]
        public void UselessResponse()
        { }

        [ResponseLocal(TestEventLabel.Label1)]
        public void ParameterlessResponse() =>
            ParameterlessCall = true;

        [ResponseLocal(TestEventLabel.Label1)]
        public void OneParamResponse(int arg) =>
            OneParamCall = arg;

        [ResponseLocal(TestEventLabel.Label1)]
        public void ExcessArgResponse(int arg1, int arg2, int excessArg) =>
            TwoParamCall = (arg1, arg2);

        [ResponseLocal(TestEventLabel.Label2)]
        public void ThreeParamResponse(int arg1, int arg2, int arg3) =>
            ThreeParamCall = (arg1, arg2, arg3);

        [ResponseLocal(TestEventLabel.Label1)]
        public void FourParamResponse(int arg1, int arg2, int arg3, int arg4) =>
            FourParamCall = (arg1, arg2, arg3, arg4);
    }
}
