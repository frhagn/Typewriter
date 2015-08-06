${
    using System.Text.RegularExpressions;

    string Verb(Method m) => m.Attributes.First(a => a.Name.StartsWith("Http")).Name.Remove(0, 4).ToLowerInvariant();

    string RouteRegex(string body) => @"\{" + body + @":?\w*\??\}";

    string ParameterRoute(Method m) => Regex.Replace(m.Route(), RouteRegex(@"(\w+)"), x => $"${{{x.Groups[1].Value}}}");

    string Data(Method m)
    {
        var data = m.Parameters.FirstOrDefault(p => !p.Type.IsPrimitive);
        return data == null ? "" : $", {{{data}: {data}}}";
    }

    string Params(Method m)
    {
        var route = Extensions.Route(m);
        var parameters = m.Parameters.Where(p => p.Type.IsPrimitive && !Regex.IsMatch(route, RouteRegex(p))).Select(p => $"{p}: {p}");

        if(parameters.Any() == false)
            return string.Empty;

        return $", {{ params: {{ {string.Join(", ", parameters)} }} }}";
    }
}
module $rootnamespace$ { $Classes(Filter)[

    export class $Name {
        constructor(private $http: ng.IHttpService) {
        } $Methods[

        public $name = ($Parameters[$name: $Type][, ]) => {
            return this.$http.$Verb<$Type>(`$ParameterRoute`$Data$Params);
        };]
    }
    
    angular.module("$rootnamespace$").service("$name", ["$http", $Name]);]
}