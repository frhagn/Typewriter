using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Typewriter.VisualStudio
{
    public interface ISolutionMonitor
    {
        event SolutionOpenedEventHandler SolutionOpened;
        event SolutionClosedEventHandler SolutionClosed;
        event ProjectAddedEventHandler ProjectAdded;
        event ProjectRemovedEventHandler ProjectRemoved;
        event FileAddedEventHandler FileAdded;
        event FileChangedEventHandler FileChanged;
        event FileDeletedEventHandler FileDeleted;
        event FileRenamedEventHandler FileRenamed;
    }

    public class SolutionMonitor : ISolutionMonitor, IVsSolutionEvents, IVsRunningDocTableEvents3, IVsTrackProjectDocumentsEvents2
    {
        private readonly ILog log;

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
        public event FileAddedEventHandler FileAdded;
        public event FileChangedEventHandler FileChanged;
        public event FileDeletedEventHandler FileDeleted;
        public event FileRenamedEventHandler FileRenamed;

        public SolutionMonitor(ILog log)
        {
            this.log = log;
            AdviceSolutionEvents();
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

            if (SolutionClosed!= null)
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

            if (FileChanged != null)
            {
                FileChanged(this, new FileChangedEventArgs(name));
            }

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
            if (FileAdded != null)
            {
                var paths = ExtractPath(cProjects, cFiles, rgpProjects, rgFirstIndices, rgpszMkDocuments);
                foreach (var path in paths)
                {
                    FileAdded(this, new FileAddedEventArgs(path));
                }
            }

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            var paths = ExtractPath(cProjects, cFiles, rgpProjects, rgFirstIndices, rgpszMkDocuments);
            if (FileDeleted != null)
            {
                foreach (var path in paths)
                {
                    FileDeleted(this, new FileDeletedEventArgs(path));
                }
            }

            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            if (FileRenamed != null)
            {
                var oldPaths = ExtractPath(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkOldNames).ToArray();
                var newPaths = ExtractPath(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkNewNames).ToArray();

                for (var i = 0; i < oldPaths.Length; i++)
                {
                    FileRenamed(this, new FileRenamedEventArgs(oldPaths[i], newPaths[i]));
                }
            }

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
    }
}