using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("AttributeArguments", "AttributeArguments")]
    public abstract class AttributeArgument : Item
    {
        public abstract Type Type { get; }
        public abstract Type TypeValue { get; }
        public abstract object Value { get; }
    }
}