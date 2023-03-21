using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class ListenerAllMatch : MonoBehaviour
    {
        public int ParameterlessCall;
        public int OneParamCall;
        public (int, int) TwoParamCall;
        public (int, int, int) ThreeParamCall;
        public (int, int, int, int) FourParamCall;

        [ResponseLocal(TestEventLabel.Label5)]
        public void ParameterlessResponse() =>
            ParameterlessCall += 1;

        [ResponseLocal(TestEventLabel.Label5)]
        public void OneParamResponse(int arg) =>
            OneParamCall += arg;

        [ResponseLocal(TestEventLabel.Label5)]
        public void TwoParamResponse(int arg1, int arg2) =>
            TwoParamCall = (TwoParamCall.Item1 + arg1, TwoParamCall.Item2 + arg2);

        [ResponseLocal(TestEventLabel.Label5)]
        public void ThreeParamResponse(int arg1, int arg2, int arg3) =>
            ThreeParamCall = (ThreeParamCall.Item1 + arg1, ThreeParamCall.Item2 + arg2, ThreeParamCall.Item3 + arg3);

        [ResponseLocal(TestEventLabel.Label5)]
        public void FourParamResponse(int arg1, int arg2, int arg3, int arg4) =>
            FourParamCall = (FourParamCall.Item1 + arg1, FourParamCall.Item2 + arg2, FourParamCall.Item3 + arg3, FourParamCall.Item4 + arg4);
    }
}
