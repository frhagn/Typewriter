using System;
using System.Linq;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.Generation.Parsing
{
    public class Extensions
    {
        [Property("(extension) string name", "The name of the $context (camelCased)")]
        public static string name(IItemInfo itemInfo)
        {
            if (itemInfo.Name.Length > 1)
                return itemInfo.Name.Substring(0, 1).ToLowerInvariant() + itemInfo.Name.Substring(1);
            return itemInfo.Name.ToLowerInvariant();
        }

        [Property("(extension) string Route", "Returns the value of the RouteAttribute of the $context and the RoutePrefixAttribute of the current class")]
        public static string Route(IMethodInfo methodInfo)
        {
            var parent = methodInfo.Parent as IItemInfo;
            var route = methodInfo.Attributes.FirstOrDefault(a => a.Name == "Route");
            var routePrefix = parent != null ? parent.Attributes.FirstOrDefault(a => a.Name == "RoutePrefix") : null;

            if (route == null && routePrefix == null) return null;
            
            return string.Concat(routePrefix != null ? routePrefix.Value + "/" : null, route != null ? route.Value : null);
        }
    }
}