using System;
using System.Collections;
using System.Collections.Generic;

namespace Typewriter.Tests.CodeModel.Support
{
    public class EventInfo
    {
        [AttributeInfo]
        public event Delegate DelegateEvent;
        public event GenericDelegate<string> GenericDelegateEvent;
    }
}
