using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Typewriter.Generation.Controllers;
using Typewriter.Metadata.CodeDom;
using Typewriter.Metadata.Providers;
using Typewriter.VisualStudio.ContextMenu;
using Task = System.Threading.Tasks.Task;

namespace Typewriter.VisualStudio
{
    [ProvideOptionPage(typeof(TypewriterOptionsPage), "Typewriter", "General", 101, 106, true)]
    [Guid(Constants.ExtensionPackageId)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [InstalledProductRegistration("#110", "#112", "1.31.0", IconResourceID = 401)]
    [ProvideLanguageService(typeof(LanguageService), Constants.LanguageName, 100, DefaultToInsertSpaces = true)]
    [ProvideLanguageExtension(typeof(LanguageService), Constants.TemplateExtension)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class ExtensionPackage : AsyncPackage, IDisposable
    {
        private Log _log;
        private IVsStatusbar _statusBar;
        private SolutionMonitor _solutionMonitor;
        private TemplateController _templateController;
        private IEventQueue _eventQueue;
        private IMetadataProvider _metadataProvider;
        private GenerationController _generationController;

        /// <summary>
        /// This read-only property returns the package instance.
        /// </summary>
        internal static ExtensionPackage Instance { get; private set; }

        internal DTE Dte { get; private set; }

        private TypewriterOptionsPage _options;

        public bool AddGeneratedFilesToProject { get; set; }
        public bool RenderOnSave { get; set; }
        public bool TrackSourceFiles { get; set; }

        protected override async System.Threading.Tasks.Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(true);

            RegisterLanguageService();
            RegisterIcons();
            ClearTempDirectory();

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await GetDteAsync().ConfigureAwait(true);
            await GetStatusBarAsync().ConfigureAwait(true);

            GetOptions();
            GetCodeModelProvider();

            _solutionMonitor = new SolutionMonitor();
            _templateController = new TemplateController(Dte);
            _eventQueue = new EventQueue(_statusBar);
            _generationController = new GenerationController(Dte, _metadataProvider, _templateController, _eventQueue);
            RenderTemplate.Initialize(this);

            WireUpEvents();
            ErrorList.Initialize(this);

            Instance = this;
        }

        private Events _dteEvents;

        private DocumentEvents _documentEvents;

        private SolutionEvents _solutionEvents;

        private void WireUpEvents()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _dteEvents = Dte.Events;

                _solutionEvents = _dteEvents.SolutionEvents;
                _solutionEvents.ProjectAdded += project => _templateController.ResetTemplates();
                _solutionEvents.ProjectRemoved += project => _templateController.ResetTemplates();
                _solutionEvents.ProjectRenamed += (project, oldName) => _templateController.ResetTemplates();

                _solutionMonitor.AdviseTrackProjectDocumentsEvents();
                _solutionMonitor.TemplateAdded += (sender, args) => _templateController.ResetTemplates();
                _solutionMonitor.TemplateDeleted += (sender, args) => _templateController.ResetTemplates();
                _solutionMonitor.TemplateRenamed += (sender, args) => _templateController.ResetTemplates();

                _solutionMonitor.CsFileAdded += (sender, args) => _generationController.OnCsFileChanged(args.Paths);
                _solutionMonitor.CsFileDeleted += (sender, args) => _generationController.OnCsFileDeleted(args.Paths);
                _solutionMonitor.CsFileRenamed += (sender, args) =>
                    _generationController.OnCsFileRenamed(args.Paths, args.OldPaths);

                _documentEvents = _dteEvents.DocumentEvents;
                _documentEvents.DocumentSaved += document =>
                {
                    if (document.FullName.EndsWith(Constants.CsExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _generationController.OnCsFileChanged(new[] {document.FullName});
                    }
                    else if (document.FullName.EndsWith(Constants.TemplateExtension,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        _generationController.OnTemplateChanged(document.FullName);
                    }
                };

                RenderTemplate.Instance.RenderTemplateClicked += (sender, args) =>
                    _generationController.OnTemplateChanged(args.Path, true);
            });
        }

        private async Task GetDteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Dte = await GetServiceAsync(typeof(DTE)).ConfigureAwait(true) as DTE;
                Assumes.Present(Dte);
                if (Dte == null)
                    ErrorHandler.ThrowOnFailure(1);
                _log = new Log(Dte);
            });
        }

        private async Task GetStatusBarAsync()
        {
            await ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _statusBar = await GetServiceAsync(typeof(SVsStatusbar)).ConfigureAwait(true) as IVsStatusbar;

                if (_statusBar == null)
                    ErrorHandler.ThrowOnFailure(1);
            });
        }

        private void GetCodeModelProvider()
        {
            try
            {
                var version = GetVisualStudioVersion();
                Log.Debug($"Visual Studio Version: {version}");

                if (version.Major >= 14 || version.Major == 0)
                {
                    var assembly = Assembly.LoadFrom(Path.Combine(Constants.TypewriterDirectory, "Typewriter.Metadata.Roslyn.dll"));
                    var type = assembly.GetType("Typewriter.Metadata.Roslyn.RoslynMetadataProvider");
                    var provider = (IMetadataProvider)Activator.CreateInstance(type);

                    Log.Debug("Using Roslyn");
                    Constants.RoslynEnabled = true;
                    _metadataProvider = provider;

                    return;
                }
            }
            catch (Exception exception)
            {
                Log.Debug(exception.Message);
            }

            Log.Debug("Using CodeDom");
            _metadataProvider = new CodeDomMetadataProvider(Dte);
        }

        private void GetOptions()
        {
            _options = (TypewriterOptionsPage) GetDialogPage(typeof(TypewriterOptionsPage));
            _options.OptionsChanged += (sender, args) =>
            {
                AddGeneratedFilesToProject = _options.AddGeneratedFilesToProject;
                RenderOnSave = _options.RenderOnSave;
                TrackSourceFiles = _options.TrackSourceFiles;
            };
            AddGeneratedFilesToProject = _options.AddGeneratedFilesToProject;
            RenderOnSave = _options.RenderOnSave;
            TrackSourceFiles = _options.TrackSourceFiles;
            _options.Watch();
        }

        private static Version GetVisualStudioVersion()
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msenv.dll");

                if (File.Exists(path))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(path);
                    var productVersion = versionInfo.ProductVersion;

                    for (var i = 0; i < productVersion.Length; i++)
                    {
                        if (char.IsDigit(productVersion, i) == false && productVersion[i] != '.')
                        {
                            productVersion = productVersion.Substring(0, i);
                            break;
                        }
                    }

                    return new Version(productVersion);
                }
            }
            catch (Exception exception)
            {
                Log.Debug(exception.Message);
            }

            return new Version(0, 0); // Not running inside Visual Studio!
        }

        private void RegisterLanguageService()
        {
            AddService(typeof(LanguageService), (container, cancellationToken, type) =>
            {
                object languageService = new LanguageService();
                return Task.FromResult(languageService);
            });
        }

        private static void RegisterIcons()
        {
            try
            {
                var icon = ThemeInfo.IsDark ? "dark.ico" : "light.ico";
                var path = Path.Combine(Constants.ResourcesDirectory, icon);

                using (RegistryKey classes = Registry.CurrentUser.OpenSubKey("SoftWare\\Classes", true))
                {
                    if (classes == null) return;

                    using (var key = classes.CreateSubKey(Constants.TemplateExtension + "\\DefaultIcon"))
                    {
                        key?.SetValue(string.Empty, path);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("Failed to register icons: {0}", e.Message);
            }
        }

        private static void ClearTempDirectory()
        {
            try
            {
                if (Directory.Exists(Constants.TempDirectory))
                {
                    Directory.Delete(Constants.TempDirectory, true);
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            if (_eventQueue != null)
            {
                _eventQueue.Dispose();
                _eventQueue = null;
            }

            Instance = null;
            _options?.Dispose();
        }
    }
}