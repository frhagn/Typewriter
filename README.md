# Typewriter (Unofficial fork)

Typewriter is a Visual Studio extensions that generates TypeScript files from c# code files using TypeScript Templates.
This allows you to create fully typed TypeScript representations of server side API, models, controllers, SignalR hubs etc. that automatically updates when you make changes to your c# code.

[Original Documentation](http://frhagn.github.io/Typewriter)  
[Download from Visual Studio Gallery old version 1.22.0](https://visualstudiogallery.msdn.microsoft.com/e1d68248-f30e-4a5d-bf18-31399a0bcfa6)

## Why new fork?

It seems that Fredrik Hagnelius author of Typewriter is very busy and do not have enough time for supporting that extensions.
From the other side that extensions really helps fulstack developers and work on it should be continued.

## Main differences between original and this fork

## Version 2.7.0

- latest updates for Visual Studio 17.1.x

## Version 2.6.1

- fix relative paths in output directory - now relative to template location

## Version 2.6.0

- ability to setup different output directory in settings.OutputDirectory
- ability to skip adding generated files in project in template by using settings.SkipAddingGeneratedFilesToProject

### Version 2.5.0

- file scoped namespaces C# 10 compatibility

### Version 2.3.1

- exposing solution path in settings

### Version 2.2.6

- compatible with Visual Studio 2022
- [IsNullable support for C# 8 nullable reference types](https://github.com/AdaskoTheBeAsT/Typewriter/pull/1) [RudeySH](https://github.com/RudeySH)

### Version 1.32.0

- added DefaultValue

### Version 1.31.0

- [PR 306 fix typo](https://github.com/frhagn/Typewriter/pull/306) [Shamus03](https://github.com/Shamus03)
- [PR 313 Ignores the Assembler if versions are equals.](https://github.com/frhagn/Typewriter/pull/313) [augustinbcu01](https://github.com/agustinbcu01)
- [PR 328 Url parameters of type DateTime[Offset] should be urlencoded](https://github.com/frhagn/Typewriter/pull/328) [rodro75](https://github.com/rodro75)
- [PR 329 Added IsVirtual to Property metadata](https://github.com/frhagn/Typewriter/pull/329) [Ackhuman](https://github.com/Ackhuman)

### Version 1.30.0

- Added support for C# 9.0 records
- Fixed error for 'string?' type - invalid marked as IsEnumerable

## Issues

The issue tracker is the preferred channel for bug reports, features requests and submitting pull requests.   
For personal support requests Stack Overflow is a better place to get help. Please use the   [typewriter](http://stackoverflow.com/questions/tagged/typewriter) tag when posting your questions.

## Getting started

Step 1: Add a TypeScript Template file (.tst)  
Step 2: Add the following code in the template

```typescript
$Classes(*Model)[ // Find all classes with a name ending with Model
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
