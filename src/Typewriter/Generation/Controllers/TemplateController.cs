using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using Typewriter.VisualStudio;

namespace Typewriter.Generation.Controllers
{
    public class TemplateController
    {

        private readonly DTE _dte;
        private readonly Func<ProjectItem, Template> _templateFactory;
        private ICollection<Template> _templates;

        public TemplateController(DTE dte, Func<ProjectItem, Template> templateFactory = null)
        {
            _dte = dte;

            _templateFactory = templateFactory ?? (item => new Template(item));
        }

        public bool TemplatesLoaded
        {
            get { return _templates != null; }
        }

        public ICollection<Template> Templates => LoadTemplates();

        private ICollection<Template> LoadTemplates()
        {
            var stopwatch = Stopwatch.StartNew();
            
            if (_templates == null)
            {
                var items = GetProjectItems();
                _templates = items.Select(i =>
                {
                    try
                    {
                        return _templateFactory(i);

                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.Message);
                        Log.Warn($"Template {i.Path()} will be ignored until the errors are removed.");

                        return null;
                    }
                }).Where(t => t != null).ToList();

                stopwatch.Stop();
                Log.Debug("{1} Templates loaded in {0} ms", stopwatch.ElapsedMilliseconds, _templates.Count);

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
                stopwatch.Stop();
                Log.Debug("{1} Templates verified in {0} ms", stopwatch.ElapsedMilliseconds, _templates.Count);

            }

            return _templates;
        }

        public void ResetTemplates()
        {
            _templates = null;
        }

        private IEnumerable<ProjectItem> GetProjectItems()
        {
            var result = _dte.Solution.AllProjects().SelectMany(m => m.AllProjectItems(Constants.TemplateExtension));
            
            return result;
        }

        public Template GetTemplate(ProjectItem projectItem)
        {
            var template = TemplatesLoaded
                    ? Templates.FirstOrDefault(m => m.TemplatePath.Equals(projectItem.Path(), StringComparison.InvariantCultureIgnoreCase))
                    : null;

            if (template == null)
            {
                template = _templateFactory(projectItem);
                ResetTemplates();
            }
            else
            {
                template.Reload();
            }

            return template;
        }
    }
}
