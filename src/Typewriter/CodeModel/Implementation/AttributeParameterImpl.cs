using Typewriter.CodeModel.Implementation;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Typescript.Implementation
{
    public class AttributeArgumentImpl : AttributeArgument
    {
        private IAttributeArgumentMetadata _metadata;
        private readonly Item parent;

        public AttributeArgumentImpl(IAttributeArgumentMetadata metadata, Item parent)
        {
            _metadata = metadata;
            this.parent = parent;
        }
        public override Type Type => TypeImpl.FromMetadata(_metadata.Type, parent);

        public override Type TypeValue => TypeImpl.FromMetadata(_metadata.TypeValue, parent);

        public override object Value => _metadata.Value;
    }
}
