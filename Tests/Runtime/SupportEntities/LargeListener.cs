using Hudossay.AttributeEvents.Assets.Runtime.Attributes;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Tests.Runtime.SupportEntities
{
    internal class LargeListener : MonoBehaviour
    {
        public int Field0;
        public int Field1;
        public int Field2;
        public int Field3;
        public int Field4;
        public int Field5;
        public int Field6;
        public int Field7;
        public int Field8;
        public int Field9;
        public int Field10;
        public int Field11;
        public int Field12;
        public int Field13;
        public int Field14;

        [ResponseLocal(TestEventLabel.Label1)]
        public void Response0(int arg) =>
            Field0 += arg;

        [ResponseLocal(TestEventLabel.Label1)]
        public void Response1(int arg) =>
            Field1 += arg;

        [ResponseLocal(TestEventLabel.Label1)]
        public void Response2(int arg) =>
            Field2 += arg;

        [ResponseLocal(TestEventLabel.Label1)]
        public void Response3(int arg) =>
            Field3 += arg;

        [ResponseLocal(TestEventLabel.Label2)]
        public void Response4(int arg) =>
            Field4 += arg;

        [ResponseLocal(TestEventLabel.Label2)]
        public void Response5(int arg) =>
            Field5 += arg;

        [ResponseLocal(TestEventLabel.Label2)]
        public void Response6(int arg) =>
            Field6 += arg;

        [ResponseLocal(TestEventLabel.Label2)]
        public void Response7(int arg) =>
            Field7 += arg;

        [ResponseLocal(TestEventLabel.Label3)]
        public void Response8(int arg) =>
            Field8 += arg;

        [ResponseLocal(TestEventLabel.Label3)]
        public void Response9(int arg) =>
            Field9 += arg;

        [ResponseLocal(TestEventLabel.Label3)]
        public void Response10(int arg) =>
            Field10 += arg;

        [ResponseLocal(TestEventLabel.Label3)]
        public void Response11(int arg) =>
            Field11 += arg;

        [ResponseLocal(TestEventLabel.Label4)]
        public void Response12(int arg) =>
            Field12 += arg;

        [ResponseLocal(TestEventLabel.Label4)]
        public void Response13(int arg) =>
            Field13 += arg;

        [ResponseLocal(TestEventLabel.Label5)]
        public void Response14(int arg) =>
            Field14 += arg;

        [ResponseLocal(TestEventLabel.Label9)]
        public void ResponseIrrelevant(int arg) { }
    }
}
