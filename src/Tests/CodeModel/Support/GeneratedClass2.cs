using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.Tests.CodeModel.Support
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
