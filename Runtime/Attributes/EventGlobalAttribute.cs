using System;

namespace Hudossay.AttributeEvents.Assets.Runtime.Attributes
{
    /// <summary>
    /// For GameEvents.
    /// Specifies a label on the event so that it can be listened globally, so that corresponding methods are invoked when raised.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EventGlobalAttribute : EventAttribute
    {
        public EventGlobalAttribute(object label) : base(label)
        { }
    }
}
