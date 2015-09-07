using System.Collections.Generic;
using System.Linq;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Identifiers
    {
        private readonly Dictionary<Context, Identifier[]> identifiers = new Dictionary<Context, Identifier[]>();
        
        public IEnumerable<Identifier> GetTempIdentifiers(Context context)
        {
            return identifiers.ContainsKey(context) ? identifiers[context] : new Identifier[0];
        }
        
        public void Add(IEnumerable<TemporaryIdentifier> temporaryIdentifiers)
        {
            foreach (var identifier in temporaryIdentifiers.GroupBy(t => t.Context))
            {
                identifiers.Add(identifier.Key, identifier.Select(grouping => grouping.Identifier).ToArray());
            }
        }
    }
}