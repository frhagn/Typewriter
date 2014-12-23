using System;
using System.Drawing;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Classification;
using Color = System.Windows.Media.Color;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    internal abstract class FormatDefinitionBase : ClassificationFormatDefinition
    {
        static readonly Lazy<IVsFontAndColorStorage> storage = new Lazy<IVsFontAndColorStorage>(() => Package.GetGlobalService(typeof(SVsFontAndColorStorage)) as IVsFontAndColorStorage, false);
        static readonly Lazy<ColorFormat> plainText = new Lazy<ColorFormat>(() => TryGetItem(storage.Value, "Plain Text"), false);

        protected FormatDefinitionBase(bool foreground, bool background)
        {
            StyleForeground = foreground;
            StyleBackground = background;

            Apply();
        }

        bool StyleForeground { get; set; }
        bool StyleBackground { get; set; }

        protected abstract ColorFormat Light { get; }
        protected abstract ColorFormat Dark { get; }

        private void Apply()
        {
            var value = plainText.Value;
            if (value == null)
                return;

            if (StyleForeground && ForegroundColor == null)
                ForegroundColor = Pick(value.Background, Light.Foreground, Dark.Foreground);

            if (StyleBackground && BackgroundColor == null)
                BackgroundColor = Pick(value.Background, Light.Background, Dark.Background);
        }

        private static Color? Pick(Color? background, Color? light, Color? dark)
        {
            if (background == null) return null;

            // HSP equation from http://alienryderflex.com/hsp.html
            byte r = background.Value.R, g = background.Value.G, b = background.Value.B;
            var hsp = Math.Sqrt(0.299 * (r * r) + 0.587 * (g * g) + 0.114 * (b * b));

            return hsp > 127.5 ? light : dark;
        }

        private static void InCategory(IVsFontAndColorStorage storage, Guid category, Action callback)
        {
            var hresult = storage.OpenCategory(ref category, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS | __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS));

            try
            {
                if (hresult == 0) callback();
            }
            finally
            {
                storage.CloseCategory();
            }
        }

        private static ColorFormat TryGetItem(IVsFontAndColorStorage fontAndColorStorage, string item)
        {
            var result = new ColorFormat();
            // load specific category to prevent our own format classifications being loaded
            InCategory(fontAndColorStorage, Microsoft.VisualStudio.Editor.DefGuidList.guidTextEditorFontCategory, () =>
            {
                var colors = new ColorableItemInfo[1];
                var hresult = fontAndColorStorage.GetItem(item, colors);
                if (hresult == 0)
                {
                    result.Foreground = ParseColor(colors[0].crForeground);
                    result.Background = ParseColor(colors[0].crBackground);
                }
            });

            return result;
        }

        private static Color ParseColor(uint color)
        {
            var dcolor = ColorTranslator.FromOle(Convert.ToInt32(color));
            return Color.FromArgb(dcolor.A, dcolor.R, dcolor.G, dcolor.B);
        }

        protected class ColorFormat
        {
            public Color? Foreground { get; set; }
            public Color? Background { get; set; }
        }
    }
}