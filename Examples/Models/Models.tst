$Classes(c => c.Name.EndsWith("Model"))[
class $Name {
    $Properties[
    public $name: $Type = $IsEnumerable[[]][$Default];
	]
	    
    constructor(json: any) {
        $Properties[
		this.$name = json.$name;
		]    
    }
}
]