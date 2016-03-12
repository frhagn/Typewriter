using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Typewriter.VisualStudio;

namespace Typewriter.Generation.Controllers
{
    public class SolutionMonitor : IVsSolutionEvents, IVsRunningDocTableEvents3, IVsTrackProjectDocumentsEvents2
    {
        private uint solutionCookie;
        private uint runningDocumentTableCookie;
        private uint trackProjectDocumentsCookie;

        private IVsSolution solution;
        private IVsRunningDocumentTable runningDocumentTable;
        private IVsTrackProjectDocuments2 trackProjectDocuments;

        public event SolutionOpenedEventHandler SolutionOpened;
        public event SolutionClosedEventHandler SolutionClosed;
        public event ProjectAddedEventHandler ProjectAdded;
        public event ProjectRemovedEventHandler ProjectRemoved;
        public event FileChangedEventHandler CsFileAdded;
        public event SingleFileChangedEventHandler CsFileChanged;
        public event FileChangedEventHandler CsFileDeleted;
        public event FileRenamedEventHandler CsFileRenamed;

        public event FileChangedEventHandler TemplateAdded;
        public event SingleFileChangedEventHandler TemplateChanged;
        public event FileChangedEventHandler TemplateDeleted;
        public event FileRenamedEventHandler TemplateRenamed;

        public SolutionMonitor()
        {
            AdviceSolutionEvents();
        }

        public void TriggerFileChanged(string path)
        {
            if (path.EndsWith(Constants.CsExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                var fileChanged = CsFileChanged;
                fileChanged?.Invoke(this, new SingleFileChangedEventArgs(FileChangeType.Changed, path));
            }
            else if (path.EndsWith(Constants.TemplateExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                var templateChanged = TemplateChanged;
                templateChanged?.Invoke(this, new SingleFileChangedEventArgs(FileChangeType.Changed, path));
            }


        }

        #region Event registration

        private void AdviceSolutionEvents()
        {
            solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (solution != null)
            {
                ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(this, out solutionCookie));
            }
        }

        private void UnadviceSolutionEvents()
        {
            if (solution != null)
            {
                ErrorHandler.ThrowOnFailure(solution.UnadviseSolutionEvents(solutionCookie));
                solution = null;
            }
        }

        private void AdviseRunningDocumentTableEvents()
        {
            runningDocumentTable = Package.GetGlobalService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (runningDocumentTable != null)
            {
                ErrorHandler.ThrowOnFailure(runningDocumentTable.AdviseRunningDocTableEvents(this, out runningDocumentTableCookie));
            }
        }

        private void UnadviseRunningDocumentTableEvents()
        {
            if (runningDocumentTable != null)
            {
                ErrorHandler.ThrowOnFailure(runningDocumentTable.UnadviseRunningDocTableEvents(runningDocumentTableCookie));
                runningDocumentTable = null;
            }
        }

        private void AdviseTrackProjectDocumentsEvents()
        {
            trackProjectDocuments = Package.GetGlobalService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
            if (trackProjectDocuments != null)
            {
                ErrorHandler.ThrowOnFailure(trackProjectDocuments.AdviseTrackProjectDocumentsEvents(this, out trackProjectDocumentsCookie));
            }
        }

        private void UnadviseTrackProjectDocumentsEvents()
        {
            if (trackProjectDocuments != null)
            {
                ErrorHandler.ThrowOnFailure(trackProjectDocuments.UnadviseTrackProjectDocumentsEvents(trackProjectDocumentsCookie));
                trackProjectDocuments = null;
            }
        }

        #endregion

        #region Solution events

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            if (ProjectAdded != null)
            {
                ProjectAdded(this, new ProjectAddedEventArgs());
            }

            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            if (SolutionOpened != null)
            {
                SolutionOpened(this, new SolutionOpenedEventArgs());
            }

            AdviseRunningDocumentTableEvents();
            AdviseTrackProjectDocumentsEvents();

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            if (ProjectRemoved != null)
            {
                ProjectRemoved(this, new ProjectRemovedEventArgs());
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            UnadviseRunningDocumentTableEvents();
            UnadviseTrackProjectDocumentsEvents();

            if (SolutionClosed != null)
            {
                SolutionClosed(this, new SolutionClosedEventArgs());
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region RunningDocumentTable events

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            uint flags, readlocks, editlocks;
            string name;
            IVsHierarchy hier;
            uint itemid;
            IntPtr docData;
            runningDocumentTable.GetDocumentInfo(docCookie, out flags, out readlocks, out editlocks, out name, out hier, out itemid, out docData);

            TriggerFileChanged(name);

            return VSConstants.S_OK;
        }


        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region TrackProjectDocuments events

        private IEnumerable<string> ExtractPath(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments)
        {
            var projItemIndex = 0;
            for (var changeProjIndex = 0; changeProjIndex < cProjects; changeProjIndex++)
            {
                var endProjectIndex = ((changeProjIndex + 1) == cProjects) ? rgpszMkDocuments.Length : rgFirstIndices[changeProjIndex + 1];
                for (; projItemIndex < endProjectIndex; projItemIndex++)
                {
                    if (rgpProjects[changeProjIndex] != null)
                    {
                        yield return rgpszMkDocuments[projItemIndex];
                    }
                }
            }
        }

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            HandleAddRemoveFiles(FileChangeType.Added, cProjects, cFiles, rgpProjects, rgFirstIndices, rgpszMkDocuments, CsFileAdded, TemplateAdded);

            return VSConstants.S_OK;
        }


        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            HandleAddRemoveFiles(FileChangeType.Deleted, cProjects, cFiles, rgpProjects, rgFirstIndices, rgpszMkDocuments, CsFileDeleted, TemplateDeleted);

            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            HandleRenameFiles(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkOldNames, rgszMkNewNames, CsFileRenamed, TemplateRenamed);

            return VSConstants.S_OK;
        }


        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            UnadviceSolutionEvents();
        }


        private void HandleAddRemoveFiles(FileChangeType changeType, int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, FileChangedEventHandler csFileEvent, FileChangedEventHandler templateEvent)
        {
            if (csFileEvent == null && templateEvent == null)
            {
                return;
            }

            var paths = ExtractPath(cProjects, cFiles, rgpProjects, rgFirstIndices, rgpszMkDocuments);
            var templates = ImmutableArray.CreateBuilder<string>();
            var csFiles = ImmutableArray.CreateBuilder<string>(cFiles);
            foreach (var path in paths)
            {
                if (path.EndsWith(Constants.CsExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    csFiles.Add(path);
                }
                else if (path.EndsWith(Constants.TemplateExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    templates.Add(path);
                }
            }

            if (csFileEvent != null && csFiles.Count > 0)
            {
                csFileEvent(this, new FileChangedEventArgs(changeType, csFiles.ToArray()));
            }
            if (templateEvent != null && templates.Count > 0)
            {
                templateEvent(this, new FileChangedEventArgs(changeType, templates.ToArray()));
            }
        }



        private void HandleRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, FileRenamedEventHandler csFileEvent, FileRenamedEventHandler templateEvent)
        {
            if (csFileEvent == null && templateEvent == null)
            {
                return;
            }

            var oldPaths = ExtractPath(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkOldNames).ToArray();
            var newPaths = ExtractPath(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkNewNames).ToArray();

            var templates = ImmutableArray.CreateBuilder<int>();
            var csFiles = ImmutableArray.CreateBuilder<int>(cFiles);


            for (var i = 0; i < oldPaths.Length; i++)
            {
                var oldPath = oldPaths[i];

                if (oldPath.EndsWith(Constants.CsExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    csFiles.Add(i);
                }
                else if (oldPath.EndsWith(Constants.TemplateExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    templates.Add(i);
                }
            }

            if (csFileEvent != null && csFiles.Count > 0)
            {
                csFileEvent(this, new FileRenamedEventArgs(csFiles.Select(i => oldPaths[i]).ToArray(), csFiles.Select(i => newPaths[i]).ToArray()));
            }
            if (templateEvent != null && templates.Count > 0)
            {
                templateEvent(this, new FileRenamedEventArgs(templates.Select(i => oldPaths[i]).ToArray(), templates.Select(i => newPaths[i]).ToArray()));
            }
        }

    }
}