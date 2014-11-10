using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Typewriter
{
    public interface IEventQueue
    {
        void QueueRender(string path, Action<string> action);
        void QueueDelete(string path, Action<string> action);
        void QueueRename(string oldPath, string newPath, Action<string, string> action);
    }

    public class EventQueue : IEventQueue
    {
        private readonly ILog log;
        private readonly ICollection<RenderEvent> queue = new HashSet<RenderEvent>();
        private readonly Timer timer = new Timer(100);
        private readonly object locker = new object();

        private DateTime timestamp = DateTime.Now;

        public EventQueue(ILog log)
        {
            this.log = log;
            SetupTimer();
        }

        public void QueueRender(string path, Action<string> action)
        {
            if (CanHandle(path) == false) return;

            lock (locker)
            {
                timestamp = DateTime.Now;
                if (queue.Any(i => i.Path == path)) return;

                log.Debug("Render queued {0}", path);
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

                log.Debug("Delete queued {0}", path);
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

                log.Debug("Rename queued {0} -> {1}", oldPath, newPath);
                queue.Add(new RenderEvent
                {
                    EventType = EventType.Changed,
                    RenameAction = action,
                    Path = oldPath,
                    NewPath = newPath
                });
            }
        }

        private static bool CanHandle(string path)
        {
            if (path.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase) == false) return false;
            if (path.EndsWith(".tst.cs", StringComparison.InvariantCultureIgnoreCase)) return false;

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
                    if (timestamp.AddMilliseconds(500) > DateTime.Now) return;

                    items = queue.ToArray();
                    queue.Clear();
                }

                foreach (var item in items)
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
                }
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
    }
}
