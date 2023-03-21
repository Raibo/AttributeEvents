using System;

namespace Hudossay.AttributeEvents.Assets.Runtime.Attributes
{
    /// <summary>
    /// For GameEvents.
    /// Specifies a label globally on the method so that it can be invoked by a local event with such label.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ResponseGlobalAttribute : ResponseAttribute
    {
        public ResponseGlobalAttribute(object label) : base(label)
        { }
    }
}
