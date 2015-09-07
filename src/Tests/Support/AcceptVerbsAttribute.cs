using System;

namespace Typewriter.Tests.Support
{
    public class AcceptVerbsAttribute : Attribute
    {
        public AcceptVerbsAttribute(params string[] verbs)
        {
        }
    }
}
