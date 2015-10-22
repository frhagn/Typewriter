using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using Typewriter.CodeModel.Implementation;
using Typewriter.Metadata.Providers;
using Typewriter.VisualStudio;

namespace Typewriter.Generation.Controllers
{
    public class TemplateController
    {
        private readonly DTE _dte;
        private readonly IMetadataProvider _metadataProvider;
        private readonly SolutionMonitor _solutionMonitor;
        private readonly EventQueue _eventQueue;

        private bool _solutionOpen;
        private ICollection<Template> _templates;

        public TemplateController(DTE dte, IMetadataProvider metadataProvider, SolutionMonitor solutionMonitor, EventQueue eventQueue)
        {
            _dte = dte;
            _metadataProvider = metadataProvider;
            _solutionMonitor = solutionMonitor;
            _eventQueue = eventQueue;

            solutionMonitor.SolutionOpened += (sender, args) => SolutionOpened();
            solutionMonitor.SolutionClosed += (sender, args) => SolutionClosed();
            solutionMonitor.ProjectAdded += (o, e) => ProjectChanged();
            solutionMonitor.ProjectRemoved += (o, e) => ProjectChanged();

            solutionMonitor.FileAdded += (o, e) => FileChanged(e.Path);
            solutionMonitor.FileChanged += (o, e) => FileSaved(e.Path);
            solutionMonitor.FileDeleted += (o, e) => FileChanged(e.Path);
            solutionMonitor.FileRenamed += (o, e) =>
            {
                FileChanged(e.OldPath);
                FileChanged(e.NewPath);
            };
        }

        private void SolutionOpened()
        {
            Log.Debug("Solution Opened");
            _solutionOpen = true;
        }

        private void SolutionClosed()
        {
            Log.Debug("Solution Closed");
            _solutionOpen = false;
        }

        private void ProjectChanged()
        {
            if (_solutionOpen == false) return;
            _templates = null;
        }

        private bool FileChanged(string path)
        {
            if (path.EndsWith(Constants.Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                _templates = null;
                return true;
            }

            return false;
        }

        private void FileSaved(string path)
        {
            if (!FileChanged(path)) return;

            try
            {
                var projectItem = _dte.Solution.FindProjectItem(path);
                var template = new Template(projectItem);

                ErrorList.Clear();

                foreach (var filename in template.GetFilesToRender())
                {
                    _eventQueue.Enqueue(generationEvent => Render(template, generationEvent), GenerationType.Render, filename);
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        private void Render(Template template, GenerationEvent generationEvent)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                var path = generationEvent.Paths[0];
                Log.Debug("Render {0}", path);

                var metadata = _metadataProvider.GetFile(path);
                var file = new FileImpl(metadata);

                var success = template.RenderFile(file, false);

                if (success == false)
                {
                    _solutionMonitor.TriggerFileChanged(path);
                }

                stopwatch.Stop();
                Log.Debug("Render completed in {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                Log.Error("Render Exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }

        public ICollection<Template> Templates => LoadTemplates();

        private ICollection<Template> LoadTemplates()
        {
            var stopwatch = Stopwatch.StartNew();

            if (this._templates == null)
            {
                var items = GetProjectItems();
                _templates = items.Select(LoadTemplate).Where(t => t != null).ToList();
            }
            else
            {
                foreach (var template in _templates)
                {
                    try
                    {
                        template.VerifyProjectItem();
                    }
                    catch
                    {
                        Log.Debug("Invalid template");
                        _templates = null;

                        return LoadTemplates();
                    }
                }
            }

            stopwatch.Stop();
            Log.Debug("Templates loaded in {0} ms", stopwatch.ElapsedMilliseconds);

            return this._templates;
        }

        private static Template LoadTemplate(ProjectItem projectItem)
        {
            try
            {
                return new Template(projectItem);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Warn($"Template {projectItem.Path()} will be ignored until the errors are removed.");

                return null;
            }
        }

        private IEnumerable<ProjectItem> GetProjectItems()
        {
            var projects = _dte.Solution.AllProjetcs().Select(p =>
            {
                try
                {
                    return p.FileName;
                }
                catch (Exception ex)
                {
                    Log.Debug("Error finding project file name ({0})", ex.Message);
                    return null;
                }
            });

            var files = projects.Where(p => string.IsNullOrWhiteSpace(p) == false)
                .SelectMany(p => new System.IO.FileInfo(p).Directory?.GetFiles("*.tst", SearchOption.AllDirectories))
                .Where(f => f != null);

            return files.Select(a => _dte.Solution.FindProjectItem(a.FullName));
        }
    }
}
