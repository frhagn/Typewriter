using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using Typewriter.VisualStudio;
using InteropConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using Timer = System.Timers.Timer;

namespace Typewriter.Generation.Controllers
{
    public interface IEventQueue : IDisposable
    {
        void QueueRender(string path, Action<string> action);
        void QueueDelete(string path, Action<string> action);
        void QueueRename(string oldPath, string newPath, Action<string, string> action);
    }

    public sealed class EventQueue : IEventQueue
    {
        private readonly IVsStatusbar statusBar;
        private readonly ICollection<RenderEvent> queue = new HashSet<RenderEvent>();
        private readonly object locker = new object();
        private Timer timer = new Timer(500);

        private DateTime timestamp = DateTime.Now;

        public EventQueue(IVsStatusbar statusBar)
        {
            this.statusBar = statusBar;
            SetupTimer();
        }

        public void QueueRender(string path, Action<string> action)
        {
            if (CanHandle(path) == false) return;

            lock (locker)
            {
                timestamp = DateTime.Now;
                if (queue.Any(i => i.Path == path)) return;

                Log.Debug("Render queued {0}", path);
                queue.Add(new RenderEvent
                {
                    EventType = EventType.Changed,
                    Action = action,
                    Path = path
                });
            }
        }

        public void QueueDelete(string path, Action<string> action)
        {
            if (CanHandle(path) == false) return;

            lock (locker)
            {
                timestamp = DateTime.Now;
                if (queue.Any(i => i.Path == path)) return;

                Log.Debug("Delete queued {0}", path);
                queue.Add(new RenderEvent
                {
                    EventType = EventType.Deleted,
                    Action = action,
                    Path = path
                });
            }
        }

        public void QueueRename(string oldPath, string newPath, Action<string, string> action)
        {
            if (CanHandle(oldPath) == false) return;

            lock (locker)
            {
                timestamp = DateTime.Now;
                if (queue.Any(i => i.Path == oldPath)) return;

                Log.Debug("Rename queued {0} -> {1}", oldPath, newPath);
                queue.Add(new RenderEvent
                {
                    EventType = EventType.Renamed,
                    RenameAction = action,
                    Path = oldPath,
                    NewPath = newPath
                });
            }
        }

        private static bool CanHandle(string path)
        {
            if (path.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase) == false) return false;

            return true;
        }

        private void SetupTimer()
        {
            this.timer.Enabled = true;
            this.timer.Elapsed += (sender, args) =>
            {
                RenderEvent[] items;

                lock (locker)
                {
                    if (timestamp.AddMilliseconds(500) > DateTime.Now)
                    {
                        return;
                    }

                    items = queue.ToArray();
                    queue.Clear();
                }

                var count = (uint)items.Length;
                if (count > 0)
                {
                    uint cookie = 0;
                    object icon = (short)InteropConstants.SBAI_Save;
                    int frozen;
                    statusBar.IsFrozen(out frozen);
                    if (frozen == 0)
                    {
                        statusBar.Progress(ref cookie, 1, "", 0, 0);
                        statusBar.SetText("Rendering template...");
                        statusBar.Animation(1, ref icon);
                    }

                    var stopwatch = Stopwatch.StartNew();
                    uint i = 0;
                    var p = items.AsParallel();
                    p.ForAll(item =>
                    {
                        switch (item.EventType)
                        {
                            case EventType.Changed:
                                item.Action(item.Path);
                                break;
                            case EventType.Deleted:
                                item.Action(item.Path);
                                break;
                            case EventType.Renamed:
                                item.RenameAction(item.Path, item.NewPath);
                                break;
                        }
                        i++;
                        statusBar.Progress(ref cookie, 1, "", i, count);
                    });

                    statusBar.Animation(0, ref icon);
                    statusBar.SetText("Rendering complete");

                    Thread.Sleep(1000);

                    // Clear the progress bar.
                    statusBar.Progress(ref cookie, 0, "", 0, 0);
                    //statusBar.FreezeOutput(0);
                    //statusBar.Clear();

                    stopwatch.Stop();
                    Log.Debug("Queue flushed in {0} ms", stopwatch.ElapsedMilliseconds);
                }
                //foreach (var item in items)
                //{
                //    switch (item.EventType)
                //    {
                //        case EventType.Changed:
                //            item.Action(item.Path);
                //            break;
                //        case EventType.Deleted:
                //            item.Action(item.Path);
                //            break;
                //        case EventType.Renamed:
                //            item.RenameAction(item.Path, item.NewPath);
                //            break;
                //    }
                //}
            };
        }

        private class RenderEvent
        {
            public EventType EventType { get; set; }
            public string Path { get; set; }
            public string NewPath { get; set; }
            public Action<string> Action { get; set; }
            public Action<string, string> RenameAction { get; set; }
        }

        private enum EventType
        {
            Changed,
            Deleted,
            Renamed
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed) return;

            disposed = true;
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }
    }
}
