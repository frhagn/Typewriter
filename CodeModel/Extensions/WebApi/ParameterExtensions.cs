using System.Linq;
using Typewriter.CodeModel;

namespace Typewriter.Extensions.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class ParameterExtensions
    {
        /// <summary>
        /// comment
        /// </summary>
        public static string PostData(this Method method)
        {
            return PostData(method, UrlExtensions.DefaultRoute);
        }

        /// <summary>
        /// 
        /// </summary>
        public static string PostData(this Method method, string route)
        {
            var url = method.Url(route);
            var dataParameters = method.Parameters.Where(p => url.Contains($"${{{p.name}}}") == false).ToList();

            if (dataParameters.Any())
            {
                return $"{{ {string.Join(", ", dataParameters.Select(p => $"{p.name}: {p.name}"))} }}";
            }

            return "null";
        }
    }
}
