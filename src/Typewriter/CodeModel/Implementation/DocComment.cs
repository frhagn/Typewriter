using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.Implementation
{
    public class DocCommentImpl : DocComment
    {
        private readonly XElement _root;

        private DocCommentImpl(XElement root, Item parent)
        {
            _root = root;
            Parent = parent;
        }

        public override Item Parent { get; }

        private string _summary;
        public override string Summary => _summary ?? (_summary = FormatValue(_root.Element("summary") == null ? string.Empty : string.Concat(_root.Element("summary").Nodes())));

        private string _returns;
        public override string Returns => _returns ?? (_returns = FormatValue(_root.Element("returns") == null ? string.Empty : string.Concat(_root.Element("returns").Nodes())));

        private ParameterCommentCollection _parameters;
        public override ParameterCommentCollection Parameters => _parameters ?? (_parameters = ParameterCommentImpl.FromXElements(_root.Elements("param"), this));

        public override string ToString()
        {
            return Summary;
        }

        public static string FormatValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var lines = value.Split('\r', '\n');
            return string.Join(" ", lines.Select(l => l.Trim())).Trim();
        }

        public static DocComment FromXml(string xml, Item parent)
        {
            try
            {
                if (string.IsNullOrEmpty(xml)) return null;

                var root = XDocument.Parse(xml).Root;
                return root != null ? new DocCommentImpl(root, parent) : null;
            }
            catch
            {
                return null;
            }
        }
    }
    
    public class ParameterCommentImpl : ParameterComment
    {
        private ParameterCommentImpl(XElement element, Item parent)
        {
            Parent = parent;
            Name = element.Attribute("name")?.Value.Trim();
            Description = DocCommentImpl.FormatValue(element.Value);
        }

        public override Item Parent { get; }

        public override string Name { get; }
        public override string Description { get; }

        public override string ToString()
        {
            return Name;
        }

        public static ParameterCommentCollection FromXElements(IEnumerable<XElement> elements, Item parent)
        {
           return new ParameterCommentCollectionImpl(elements.Select(e => new ParameterCommentImpl(e, parent)));
        }
    }
}