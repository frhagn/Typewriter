
module $rootnamespace$ { $Classes(Filter)[

    export class $Name {
        $Properties[
        public $name: $Type = $IsEnumerable[[]][$Default];]
		
        constructor(data: any = null) {
            this.map(data);
        }

        public map = (data: any) => {
            if(data) {$Properties[
                this.$name = $IsPrimitive[$IsDate[new Date(data.$name)][data.$name]][$IsEnumerable[data.$name ? data.$name.map(i => new $Class(i)) : []][data.$name ? new $Class(data.$name) : null]];]
            }
        };
    }]
}