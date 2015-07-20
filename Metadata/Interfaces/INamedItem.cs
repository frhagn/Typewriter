using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.Metadata.Interfaces
{
    public interface INamedItem
    {
        string Name { get; }
        string FullName { get; }
    }
}
