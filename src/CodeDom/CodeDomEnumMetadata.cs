using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomEnumMetadata : IEnumMetadata
    {
        private readonly CodeEnum _codeEnum;
        private readonly CodeDomFileMetadata _file;

        private CodeDomEnumMetadata(CodeEnum codeEnum, CodeDomFileMetadata file)
        {
            _codeEnum = codeEnum;
            _file = file;
        }

        public string DocComment => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return _codeEnum.DocComment;
        }).Join();

        public string Name => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return _codeEnum.Name;
        }).Join();

        public string FullName => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return _codeEnum.FullName;
        }).Join();

        public string Namespace => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return GetNamespace();
        }).Join();

        public ITypeMetadata Type => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return new LazyCodeDomTypeMetadata(_codeEnum.FullName, false, false, _file);
        }).Join();

        public IEnumerable<IAttributeMetadata> Attributes => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return CodeDomAttributeMetadata.FromCodeElements(_codeEnum.Attributes);
        }).Join();

        public IEnumerable<IEnumValueMetadata> Values => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return CodeDomEnumValueMetadata.FromCodeElements(_codeEnum.Members, _file);
        }).Join();

        public IClassMetadata ContainingClass => ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // You're now on the UI thread.
            return CodeDomClassMetadata.FromCodeClass(_codeEnum.Parent as CodeClass2, _file);
        }).Join();

        private string GetNamespace()
        {
            return ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                // You're now on the UI thread.
                return _codeEnum.Parent is CodeClass2 parent
                    ? parent.FullName
                    : (_codeEnum.Namespace?.FullName ?? string.Empty);
            }).Join();
        }

        internal static IEnumerable<IEnumMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                // You're now on the UI thread.

                var elems = codeElements.OfType<CodeEnum>().Where(e => e.Access == vsCMAccess.vsCMAccessPublic)
                    .Select(e => new CodeDomEnumMetadata(e, file)).ToList();
                return elems;
            }).Join();
        }
    }
}