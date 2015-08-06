
module $rootnamespace$ {
    
    // $Classes/Enums/Interfaces(filter)[template][separator]
    // filter (optional): Matches the name or full name of the current item. * = match any, wrap in [] to match attributes or prefix with : to match interfaces or base classes.
    // template: The template to repeat for each matched item
    // separator (optional): A separator template that is placed between all templates e.g. $Properties[public $name: $Type][, ]
	
	// http://frhagn.github.io/Typewriter/

    $Classes(Filter)[
    export class $Name {
        
    }]
}