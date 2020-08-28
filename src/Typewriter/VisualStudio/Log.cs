using System;
using System.Globalization;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Typewriter.VisualStudio
{
    public class Log
    {
        private static Log instance;

        private readonly DTE dte;
        private OutputWindowPane outputWindowPane;

        internal Log(DTE dte)
        {
            this.dte = dte;
            instance = this;
        }

        public static void Debug(string message, params object[] parameters)
        {
#if DEBUG
            instance?.Write("DEBUG", message, parameters);
#endif
        }

        public static void Info(string message, params object[] parameters)
        {
            instance?.Write("INFO", message, parameters);
        }

        public static void Warn(string message, params object[] parameters)
        {
            instance?.Write("WARNING", message, parameters);
        }

        public static void Error(string message, params object[] parameters)
        {
            instance?.Write("ERROR", message, parameters);
        }

        private void Write(string type, string message, object[] parameters)
        {
            message = $"{DateTime.Now:HH:mm:ss.fff} {type}: {message}";

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                try
                {
                    if (parameters.Any())
                        OutputWindow.OutputString(string.Format(CultureInfo.InvariantCulture, message, parameters) +
                                                  Environment.NewLine);
                    else
                        OutputWindow.OutputString(message + Environment.NewLine);
                }
                catch
                {
                }
            });
        }

        private OutputWindowPane OutputWindow
        {
            get
            {
                return ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (outputWindowPane != null) return outputWindowPane;

                    var window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
                    var outputWindow = (OutputWindow) window.Object;

                    for (uint i = 1; i <= outputWindow.OutputWindowPanes.Count; i++)
                    {
                        if (outputWindow.OutputWindowPanes.Item(i).Name
                            .Equals("Typewriter", StringComparison.CurrentCultureIgnoreCase))
                        {
                            outputWindowPane = outputWindow.OutputWindowPanes.Item(i);
                            break;
                        }
                    }

                    return outputWindowPane ?? (outputWindowPane = outputWindow.OutputWindowPanes.Add("Typewriter"));
                });
            }
        }
    }
}