using System;
using System.Linq;
using EnvDTE;

namespace Typewriter
{
    public interface ILog
    {
        void Debug(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Warn(string message, params object[] parameters);
        void Error(string message, params object[] parameters);
    }

    public class Log : ILog
    {
        private static Log instance;

        public static void Print(string message, params object[] parameters)
        {
            message = string.Format("{0:HH:mm:ss}: ", DateTime.Now) + message;

            if (parameters.Any())
                instance.OutputWindow.OutputString(string.Format(message, parameters) + Environment.NewLine);
            else
                instance.OutputWindow.OutputString(message + Environment.NewLine);
        }

        private readonly DTE dte;
        private OutputWindowPane outputWindowPane;

        public Log(DTE dte)
        {
            this.dte = dte;
            instance = this;
        }

        public void Debug(string message, params object[] parameters)
        {
            Write("DEBUG", message, parameters);
        }

        public void Info(string message, params object[] parameters)
        {
            Write(" INFO", message, parameters);
        }

        public void Warn(string message, params object[] parameters)
        {
            Write(" WARN", message, parameters);
        }

        public void Error(string message, params object[] parameters)
        {
            Write("ERROR", message, parameters);
        }

        private void Write(string type, string message, object[] parameters)
        {
            message = string.Format("{0:HH:mm:ss} {1}: ", DateTime.Now, type) + message;

            try
            {
                if (parameters.Any())
                    OutputWindow.OutputString(string.Format(message, parameters) + Environment.NewLine);
                else
                    OutputWindow.OutputString(message + Environment.NewLine);
            }
            catch { }
        }

        private OutputWindowPane OutputWindow
        {
            get
            {
                if (outputWindowPane != null) return outputWindowPane;

                var window = dte.Windows.Item(Constants.vsWindowKindOutput);
                var outputWindow = (OutputWindow)window.Object;

                for (uint i = 1; i <= outputWindow.OutputWindowPanes.Count; i++)
                {
                    if (outputWindow.OutputWindowPanes.Item(i).Name.Equals("Typewriter", StringComparison.CurrentCultureIgnoreCase))
                    {
                        outputWindowPane = outputWindow.OutputWindowPanes.Item(i);
                        break;
                    }
                }

                if (outputWindowPane == null)
                {
                    outputWindowPane = outputWindow.OutputWindowPanes.Add("Typewriter");
                }

                return outputWindowPane;
            }
        }
    }
}