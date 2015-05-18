# Typewriter
Automatic TypeScript template generation from C# source files

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

Each time a class matching the filter in the template is saved a TypeScript class matching the template is updated or added to the project.

## Custom methods
You can add custom methods to your templates by placing a code block in the template file.
```typescript
${
    var LoudName = (Class c) => c.Name.ToUpper();
    var QuietName = (Class c) => 
    {
        return c.Name.ToLower();
    }
}

$Classes(*Model)[ // Loop all classes with a name ending with Model
    class $LoudName {
        constructor($Properties[public $name: $Type][, ]) {
        }
    }
]
```

## Lambda filters
You can add custom lambda filter expressions in addition to the simple string based filters.
```typescript
$Classes(c => c.Attributes.Any(a => a.Name == "Model"))[
    class $Name {
        constructor($Properties[public $name: $Type][, ]) {
        }
    }
]
```

## Changelog
* 0.9.12
 * Added support for Visual Studio 2015 RC
 * Changed the source file path mapping to be relative to the project directory instead of the solution directory 
 * Minor bug fixes
* 0.9.11
 * Performance optimizations when scanning solution for template files
* 0.9.10
 * Performance optimizations when saving template file with advanced filters
 * Added one retry when failing to render when saving template file
* 0.9.9
 * Fixed bug when renaming files
* 0.9.8
 * Support for lambda expressions in filters
 * Changed syntax for custom methods
 * Minor bug fixes
* 0.9.7
 * Basic support for custom methods
* 0.9.6
 * Support for IsDate eg. ```$IsDate[true template][false template]```
* 0.9.5
 * Support for interface filters by starting filter string with ":" eg. ```$Classes(:IModel)[template]```
 * Support for attribute filters by encapsulating filter string with "[]" eg. ```$Classes([Model])[template]```
 * Fixed bug when renaming files
 * The project file is now saved when rendering templates

## Links
[Documentation](http://frhagn.github.io/Typewriter)  
[Download from Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/e1d68248-f30e-4a5d-bf18-31399a0bcfa6)

More info coming soon :)
