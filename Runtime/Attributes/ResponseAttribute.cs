using System;

namespace Hudossay.AttributeEvents.Assets.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class ResponseAttribute : Attribute
    {
        public object Label;

        public ResponseAttribute(object label)
        {
            Label = label;
        }
    }
}
