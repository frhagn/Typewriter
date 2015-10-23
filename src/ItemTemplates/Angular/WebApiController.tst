${
    using System.Text.RegularExpressions;
    using Typewriter.Extensions.WebApi;
     
    //TODO: Manage AcceptVerbAttribute
    string Verb(Method m) {
        var v = m.Attributes.Where(a=>a.Name.StartsWith("Http")).FirstOrDefault()?.name.Remove(0, 4) ;
        if (v != null)
            return v;
            
        var acceptedVerbs = new string [] {
            "get",
            "post",
            "put",
            "delete",
            "patch",
            "head",
            "options"
        };
                
        return acceptedVerbs.FirstOrDefault(a=> m.name.StartsWith(a))??"";
    }

    string RouteRegex(string body) => @"\{" + body + @":?\w*\??\}";

    string ParameterRoute(Method m) {
        var pId = m.Parameters.FirstOrDefault(p=>p.name == "id");

       var route = Regex.Replace(m.Route()??"", RouteRegex(@"(\w+)"), x => $"${{{x.Groups[1].Value}}}");
       var parent = m.Parent as Class;

       if (pId != null && route == "")
        route = "${id}";

       var routePrefix = parent.Attributes.FirstOrDefault(a=>a.Name == "RoutePrefix")?.Value ;
       if (routePrefix != null)
        return route;
       
       routePrefix = routePrefix  ?? $"api/{parent.name.Replace("Controller", "")}/";

       return routePrefix + route;
    }
    

    string Data(Method m)
    { 
        //var data = m.Parameters.FirstOrDefault(p => !p.Type.IsPrimitive);
        var data = m.Parameters.Where(p=>p.Attributes.Any(a=>a.Name == "FromBody")).FirstOrDefault();
        if (data != null)
            return $"{{{data}: {data}}}";
        
        data = m.Parameters.FirstOrDefault(p => !p.Type.IsPrimitive);
        return data == null ? "null" : $"{{{data}: {data}}}";
    } 

    string Params(Method m)
    { 
        var route = m.Route()??""; 
      
        var parameters = m.Parameters
                .Where(p=>p.name != "id")
                .Where(p => p.Type.IsPrimitive && !Regex.IsMatch(route, RouteRegex(p)) && !p.Attributes.Any(a=>a.Name == "FromBody"))
                .Select(p => $"{p}: {p}");

        if(parameters.Any() == false)
            return "null";
            

       return $"{{ params: {{ {string.Join(", ", parameters)} }} }}";            
    }

    string HttpConfig(Method m){
        var url = ParameterRoute(m);
        var method = Verb(m);
        var parameters = Params(m);
        var data = Data(m);

        var result = $"{{ url: '{url}', method: \"{method}\", params: {parameters}, data: {data} }}";
        result = result.Replace(", params: null", "");
        result = result.Replace(", data: null", "");

        return result;

    }

    string ServiceName(Class c){
        return c.name.Replace("Controller", "");
    }

    string CalculatedType(Method m){
        string typeName = m.Type.Name;
        if (typeName.StartsWith("Task") && m.Type.IsGeneric)
            typeName = m.Type.TypeArguments.First().Name;

        if (typeName == "IHttpActionResult")
            return "void";

        return typeName;
        
    }
}

module $rootnamespace$ { $Classes(c=>c.BaseClass.Name == "ApiController")[ 
    export class $Name { 
        constructor(private $http: ng.IHttpService) {
        } $Methods[

        public $name = ($Parameters[$name: $Type][, ]) : ng.IHttpPromise<$CalculatedType> => {
            return this.$http<$CalculatedType>($HttpConfig);
        };] 
    }
    
    angular.module("$rootnamespace$").service("$ServiceName", ["$http", $Name]);]    
}