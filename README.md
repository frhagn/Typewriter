# Typewriter (Unofficial fork)

Typewriter is a Visual Studio extensions that generates TypeScript files from c# code files using TypeScript Templates.
This allows you to create fully typed TypeScript representations of server side API, models, controllers, SignalR hubs etc. that automatically updates when you make changes to your c# code.

[Original Documentation](http://frhagn.github.io/Typewriter)  
[Download from Visual Studio Gallery old version 1.22.0](https://visualstudiogallery.msdn.microsoft.com/e1d68248-f30e-4a5d-bf18-31399a0bcfa6)

## Why new fork?

It seems that Fredrik Hagnelius author of Typewriter is very busy and do not have enough time for supporting that extensions.
From the other side that extensions really helps fulstack developers and work on it should be continued.

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
