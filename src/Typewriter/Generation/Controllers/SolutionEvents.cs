using System;
using System.IO;

namespace Typewriter.Generation.Controllers
{
    public delegate void SolutionOpenedEventHandler(object sender, SolutionOpenedEventArgs e);
    public delegate void SolutionClosedEventHandler(object sender, SolutionClosedEventArgs e);
    public delegate void ProjectAddedEventHandler(object sender, ProjectAddedEventArgs e);
    public delegate void ProjectRemovedEventHandler(object sender, ProjectRemovedEventArgs e);
    public delegate void FileChangedEventHandler(object sender, FileChangedEventArgs e);
    public delegate void SingleFileChangedEventHandler(object sender, SingleFileChangedEventArgs e);
    public delegate void FileRenamedEventHandler(object sender, FileRenamedEventArgs e);

    public class SolutionOpenedEventArgs : EventArgs
    {
    }

    public class SolutionClosedEventArgs : EventArgs
    {
    }

    public class ProjectAddedEventArgs : EventArgs
    {
    }

    public class ProjectRemovedEventArgs : EventArgs
    {
    }

    public enum FileChangeType
    {
        Added,
        Changed,
        Deleted,
        Renamed
    }

    public class FileChangedEventArgs : EventArgs
    {
        public FileChangeType ChangeType { get; private set; }
        public string[] Paths { get; private set; }

        public FileChangedEventArgs(FileChangeType changeType, params string[] paths)
        {
            ChangeType = changeType;
            Paths = paths;
        }
    }
    
    public class FileRenamedEventArgs : FileChangedEventArgs
    {
        public string[] OldPaths { get; private set; }
        
        public FileRenamedEventArgs(string[] oldPaths, string[] newPaths) : base(FileChangeType.Renamed, newPaths)
        {
            OldPaths = oldPaths;
        }
    }
    public class SingleFileChangedEventArgs : FileChangedEventArgs
    {
        public string Path { get; private set; }

        public SingleFileChangedEventArgs(FileChangeType changeType, string path) : base(changeType,path)
        {
            Path = path;
        }
    }

}
