using System;

namespace Hudossay.AttributeEvents.Assets.Runtime.Attributes
{
    /// <summary>
    /// For GameEvents.
    /// Specifies a label on the method so that it can be invoked by a local event with such label.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ResponseLocalAttribute : ResponseAttribute
    {
        public ResponseLocalAttribute(object label) : base(label)
        { }
    }
}
