using System;
using System.Linq;
using System.Text.RegularExpressions;
using Typewriter.CodeModel;

namespace Typewriter.Extensions.WebApi
{
    /// <summary>
    /// Extension methods for extracting Web API url.
    /// </summary>
    public static class UrlExtensions
    {
        internal const string DefaultRoute = "api/{controller}/{id?}";

        /// <summary>
        /// Returns the url for the Web API action based on route attributes (or "api/{controller}/{id?}" if no attributes are present).
        /// Route parameters are converted to TypeScript string interpolation syntax by prefixing all parameters with $ e.g. ${id}.
        /// Optional parameters are added as QueryString parameters for GET and HEAD requests.
        /// </summary>
        public static string Url(this Method method)
        {
            return Url(method, DefaultRoute);
        }

        /// <summary>
        /// Returns the url for the Web API action based on route attributes (or the supplied convension route if no attributes are present).
        /// Route parameters are converted to TypeScript string interpolation syntax by prefixing all parameters with $ e.g. ${id}.
        /// Optional parameters are added as QueryString parameters for GET and HEAD requests.
        /// </summary>
        public static string Url(this Method method, string route)
        {
            // 1. Select the proper route based on route attributes and the supplied convension route.
            // 2. Remove optional route parameters that have no matching parameter on the method.
            // 3. Get the controler and action parameters from the class name and method name.
            // 4. Remove all parameter constraints and convert the placeholder to TypeScript string interpolation syntax by prefixing all parameters with $.
            // 5. Find parameters on the method that are not specified in the route and add them as a query string.

            route = Route(method, route);
            route = RemoveUnmatchedOptionalParameters(method, route);
            route = ReplaceSpecialParameters(method, route);
            route = ConvertRouteParameters(method, route);
            route = AppendQueryString(method, route);

            return route;
        }

        /// <summary>
        /// Returns the route for the Web API action based on route attributes (or "api/{controller}/{id?}" if no attributes are present).
        /// </summary>
        public static string Route(this Method method)
        {
            return Route(method, DefaultRoute);
        }

        /// <summary>
        /// Returns the route for the Web API action based on route attributes (or the supplied convension route if no attributes are present).
        /// </summary>
        public static string Route(this Method method, string route)
        {
            var routeAttribute = method.Attributes.FirstOrDefault(a => a.Name == "Route") ??
                                 method.Attributes.FirstOrDefault(a => a.Name.StartsWith("Http"));

            var routePrefix = GetRoutePrefix(method.Parent as Class);

            if (routeAttribute != null)
            {
                var value = ParseAttributeValue(routeAttribute.Value);

                if (string.IsNullOrEmpty(value))
                {
                    route = routePrefix ?? route;
                }
                else if (value.StartsWith("~"))
                {
                    route = value.Remove(0, 1);
                }
                else if (routePrefix == null)
                {
                    route = value;
                }
                else
                {
                    route = string.Concat(routePrefix, "/", value);
                }
            }
            else if(routePrefix != null)
            {
                route = string.Concat(routePrefix, "/", route);
            }

            return route;
        }

        private static string GetRoutePrefix(Class @class)
        {
            if (@class == null)
            {
                return null;
            }

            var routePrefix = @class?.Attributes.FirstOrDefault(a => a.Name == "RoutePrefix")?.Value?.TrimEnd('/');

            if (String.IsNullOrEmpty(routePrefix))
            {
                routePrefix = @class?.Attributes.FirstOrDefault(a => a.Name == "Route")?.Value?.TrimEnd('/');
            }

            if (String.IsNullOrEmpty(routePrefix) && @class.BaseClass != null)
            {
                routePrefix = GetRoutePrefix(@class.BaseClass);
            }

            return routePrefix;
        }

        private static string ParseAttributeValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (value.StartsWith("\"") == false) return value;

            // Extract the route part for named and/or ordered routes
            var expression = new Regex(@"(?<="")(?:\\.|[^""\\])*(?="")");
            return expression.Match(value).Value;
        }

        private static string RemoveUnmatchedOptionalParameters(Method method, string route)
        {
            // Todo: Handle {parameter:regex(...)?} containing ? and/or }
            var parameters = Regex.Matches(route, @"\{(\w+):*\w*\?\}").Cast<Match>().Select(m => m.Groups[1].Value);
            var unmatchedParameters = parameters.Where(o => method.Parameters.Any(p => p.Name.Equals(o, StringComparison.OrdinalIgnoreCase)) == false).ToList();

            foreach (var parameter in unmatchedParameters)
            {
                route = Regex.Replace(route, $"\\{{{parameter}:*\\w*\\?\\}}", string.Empty);
            }

            return route;
        }

        private static string ReplaceSpecialParameters(Method method, string route)
        {
            if ((route.Contains("{controller}") || route.Contains("[controller]")) && method.Parameters.Any(p => p.name == "controller") == false)
            {
                var parent = method.Parent as Class;
                if (parent != null)
                {
                    var controller = parent.Name;
                    if (controller.EndsWith("Controller"))
                    {
                        controller = controller.Substring(0, controller.Length - 10);
                    }

                    route = route.Replace("{controller}", controller).Replace("[controller]", controller);
                }
            }

            if ((route.Contains("{action}") || route.Contains("[action]")) && method.Parameters.Any(p => p.name == "action") == false)
            {
                var action = method.Attributes.FirstOrDefault(a => a.Name == "ActionName")?.Value ?? method.name;
                route = route.Replace("{action}", action).Replace("[action]", action);
            }

            return route;
        }

        private static string ConvertRouteParameters(Method method, string route)
        {
            return Regex.Replace(route, @"\{\*?(\w+):?\w*\??\}", m => $"${{{GetParameterValue(method, m.Groups[1].Value)}}}");
        }

        private static string AppendQueryString(Method method, string route)
        {
            // Todo: Add support for FromUri attribute

            //var httpMethod = method.HttpMethod();
            //if (httpMethod == "get" || httpMethod == "head")
            //{
            var prefix = route.Contains("?") ? "&" : "?";

            foreach (var parameter in method.Parameters.Where(p => p.Type.IsPrimitive && p.Attributes.Any(a => a.Name == "FromBody") == false))
            {
                if (route.Contains($"${{{GetParameterValue(method, parameter.Name)}}}") == false)
                {
                    route += $"{prefix}{parameter.Name}=${{{GetParameterValue(method, parameter.Name)}}}";
                    prefix = "&";
                }
            }
            //}

            return route;
        }

        internal static string GetParameterValue(Method method, string name)
        {
            var parameter = method.Parameters.FirstOrDefault(p => p.Name == name);
            if (parameter?.Type.Name == "string")
            {
                return $"encodeURIComponent({name})";
            }

            return name;
        }
    }
}
