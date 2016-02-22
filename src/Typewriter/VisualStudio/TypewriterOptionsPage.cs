using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace Typewriter.VisualStudio
{
    // References
    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/303fce01-dfc0-43b3-a578-8b3258c0b83f/get-a-dialogpage-outside-of-the-package-class?forum=vsx
    //https://msdn.microsoft.com/en-us/library/bb166553.aspx
    //https://msdn.microsoft.com/en-us/library/bb166195.aspx
    //https://msdn.microsoft.com/en-us/library/bb165657.aspx
    //https://msdn.microsoft.com/en-us/library/bb166030.aspx
    //https://msdn.microsoft.com/en-us/library/bb166195.aspx
    public class TypewriterOptionsPage : DialogPage
    {
        [Category("General options")]
        [DisplayName("Run on file save")]
        [Description("Enable/disable to run Typewriter on save files.")]
        public bool RunOnFileSave { get; set; }
    }
}
