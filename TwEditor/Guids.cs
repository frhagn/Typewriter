// Guids.cs
// MUST match guids.h
using System;

namespace Company.TwEditor
{
    static class GuidList
    {
        public const string guidTwEditorPkgString = "7b2f8ea6-5c1a-4e2c-9d53-f0a5d02d9c38";
        public const string guidTwEditorCmdSetString = "411c515e-848f-4b89-9ac5-b7df47dd9bff";
        public const string guidTwLanguageServiceString = "bb5d6809-9c5d-443c-a37c-c29e6af2fe15";

        public static readonly Guid guidTwEditorCmdSet = new Guid(guidTwEditorCmdSetString);
        public static readonly Guid guidTwLanguageService = new Guid(guidTwLanguageServiceString);
    };
}