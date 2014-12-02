using System;
using System.Collections.Generic;

namespace Typewriter.TemplateEditor.Lexing.Scopes
{
    public class ScopeStack : Stack<Scope>
    {
        private bool changed;

        public ScopeStack()
        {
            base.Push(Scope.None);
        }

        public new void Push(Scope scope)
        {
            changed = true;
            base.Push(scope);
        }

        public bool Changed
        {
            get
            {
                var c = changed;
                changed = false;
                return c;
            }
        }

        public Scope Current
        {
            get { return Peek(); }
        }
    }
}