using System;

namespace Typewriter.Generation.Controllers
{
    public delegate void SolutionOpenedEventHandler(object sender, SolutionOpenedEventArgs e);
    public delegate void SolutionClosedEventHandler(object sender, SolutionClosedEventArgs e);
    public delegate void ProjectAddedEventHandler(object sender, ProjectAddedEventArgs e);
    public delegate void ProjectRemovedEventHandler(object sender, ProjectRemovedEventArgs e);
    public delegate void FileAddedEventHandler(object sender, FileAddedEventArgs e);
    public delegate void FileChangedEventHandler(object sender, FileChangedEventArgs e);
    public delegate void FileDeletedEventHandler(object sender, FileDeletedEventArgs e);
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

    public class FileAddedEventArgs : EventArgs
    {
        public string Path { get; private set; }

        public FileAddedEventArgs(string path)
        {
            Path = path;
        }
    }

    public class FileChangedEventArgs : EventArgs
    {
        public string Path { get; private set; }

        public FileChangedEventArgs(string path)
        {
            Path = path;
        }
    }

    public class FileDeletedEventArgs : EventArgs
    {
        public string Path { get; private set; }

        public FileDeletedEventArgs(string path)
        {
            Path = path;
        }
    }

    public class FileRenamedEventArgs : EventArgs
    {
        public string OldPath { get; private set; }
        public string NewPath { get; private set; }

        public FileRenamedEventArgs(string oldPath, string newPath)
        {
            OldPath = oldPath;
            NewPath = newPath;
        }
    }
}
