using System.ComponentModel.DataAnnotations;

namespace Typewriter.Tests.Metadata.Support
{
    [MetadataType(typeof(GeneratedClassMetadata))]
    public partial class GeneratedClass
    {
        internal sealed class GeneratedClassMetadata
        {
            [Key]
            public int Id { get; set; }
            [Display(Name = "NewPropertyName")]
            public string Title { get; set; }
        }
    }
}