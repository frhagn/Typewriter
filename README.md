# Typewriter
Automatic TypeScript template generation from C# source files

## Changelog
* 0.9.7
 * Basic support for custom methods
* 0.9.6
 * Support for IsDate eg. $IsDate[true template][false template]
* 0.9.5
 * Support for interface filters by starting filter string with ":" eg. $Classes(:IModel)[template]
 * Support for attribute filters by encapsulating filter string with "[]" eg. $Classes([Model])[template]
 * Fixed bug when renaming files
 * The project file is now saved when rendering templates

## Getting started
Step 1: Add a TypeScript Template file (.tst)  
Step 2: Add the following code in the template  
```typescript
$Classes(*Model)[ // Loop all classes with a name ending with Model
    class $Name {
        constructor($Properties[public $name: $Type][, ]) {
        }
    }
]
```
Step 3: Save the template  
Step 4: Add a c# class named TestModel  
Step 5: Add the following code to the class  
```c#
using System;

namespace TestApplication
{
    public class TestModel
    {
        public int Id { get; set; }
        public int Name { get; set; }
    }
}
```
Step 6: Save the class.  

Each time a class matching the filter in the template is saved a new TypeScript class matching the template is added to the project.

## Custom methods
You can add custom methods to your templates by placing a code block in the template file.
```typescript
${
    declare LoudName(IClassInfo classInfo)
    {
        return classInfo.Name.ToUpper();
    }
}

$Classes(*Model)[ // Loop all classes with a name ending with Model
    class $LoudName {
        constructor($Properties[public $name: $Type][, ]) {
        }
    }
]
```

## Links
[Documentation](http://frhagn.github.io/Typewriter)  
[Download from Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/e1d68248-f30e-4a5d-bf18-31399a0bcfa6)

More info coming soon :)
