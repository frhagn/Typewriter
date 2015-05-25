$Classes(c => c.Name.EndsWith("Controller"))[
class $Name {     
    $Methods(m => m.Attributes.Any(a => a.Name == "HttpPost"))[
	public $name = ($Parameters[$name: $Type][, ]) : Q.Promise<any> => {
		var request = new Request("$Route", { $Parameters[$name: $name][, ] });$Type[$GenericTypeArguments[$IsPrimitive[][
        request.map = data => $IsEnumerable[data ? data.map(i => new $Class(i)) : [];][new $Class(data);]]]]$IsViewResult[
        request.jsonResponse = false;]
		return this.actionInvoker.invoke<$IsGeneric[$Type[$GenericTypeArguments[$Type]]][void]>(request, $ShouldCache[true][false]);
	};]
}]