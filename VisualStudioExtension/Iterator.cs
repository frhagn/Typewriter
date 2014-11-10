using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Typewriter
{
    public sealed class Iterator<T> : IEnumerable<T>
    {
        public static IEnumerable<TOut> Select<TOut>(Func<CodeElements> items, Func<T, TOut> selector)
        {
            CodeElements elements = null;
            try
            {
                elements = items();
            }
            catch (NotImplementedException) { }

            var iterator = new Iterator<T>(elements);
            return iterator.Select(selector);
        }

        public static IEnumerable<TOut> Select<TOut>(Func<CodeElements> items, Func<T, int, TOut> selector)
        {
            CodeElements elements = null;
            try
            {
                elements = items();
            }
            catch (NotImplementedException) { }

            var iterator = new Iterator<T>(elements);
            return iterator.Select(selector);
        }

        public static IEnumerable<TOut> Select<TOut>(Func<CodeElements> items, Func<T, bool> predicate, Func<T, TOut> selector)
        {
            CodeElements elements = null;
            try
            {
                elements = items();
            }
            catch (NotImplementedException) { }

            var iterator = new Iterator<T>(elements).Where(predicate);
            return iterator.Select(selector);
        }

        private readonly CodeElements codeElements;

        public Iterator(CodeElements codeElements)
        {
            //if (codeElements == null)
            //{
            //    throw new ArgumentNullException("codeElements");
            //}

            this.codeElements = codeElements;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate(codeElements).OfType<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<CodeElement> Enumerate(CodeElements elements)
        {
            if (elements == null)
            {
                yield break;
            }

            foreach (CodeElement element in elements)
            {
                yield return element;

                //CodeElements children;

                //try
                //{
                //    children = element.Children;
                //}
                //catch (NotImplementedException)
                //{
                //    children = null;
                //}

                //if (children == null) continue;

                //foreach (var subElement in Enumerate(children))
                //{
                //    yield return subElement;
                //}
            }
        }
    }
}