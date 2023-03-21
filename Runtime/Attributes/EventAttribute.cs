using System;

namespace Hudossay.AttributeEvents.Assets.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public abstract class EventAttribute : Attribute
    {
        public object Label;

        public EventAttribute(object label)
        {
            Label = label;
        }
    }
}
