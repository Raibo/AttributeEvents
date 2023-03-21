using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class GlobalListener : MonoBehaviour
    {
        public int NoParamCalls;
        public int IntParamCalls;
        public int NotConnectedCalls;

        [ResponseGlobal(TestEventLabel.Label1)]
        public void GlobalResponseNoParam() => NoParamCalls++;

        [ResponseGlobal(TestEventLabel.Label2)]
        public void GlobalResponseIntParam(int a) => IntParamCalls += a;

        [ResponseGlobal(TestEventLabel.Label4)]
        public void GlobalResponseNotConnected(int a, int b) => NotConnectedCalls += a;


    }
}
