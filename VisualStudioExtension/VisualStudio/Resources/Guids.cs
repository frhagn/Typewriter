using System;

namespace Typewriter.VisualStudio.Resources
{
    static class GuidList
    {
        public const string VisualStudioExtensionPackageId = "45b6b392-ce2f-409c-a39f-bbf90b34349e";
        //public const string VisualStudioExtensionCommandSetId = "3e0266e4-433b-4a29-b647-c0bf3b08003b";
        public const string LanguageServiceString = "aa5d6809-9c5d-443c-a37c-c29e6af2fe15";

        //public static readonly Guid VisualStudioExtensionCommandSet = new Guid(VisualStudioExtensionCommandSetId);
        public static readonly Guid LanguageService = new Guid(LanguageServiceString);
    }
}