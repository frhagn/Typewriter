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
    public sealed class EventQueue : IDisposable
    {
        private readonly IVsStatusbar statusBar;
        private readonly ICollection<GenerationEvent> queue = new HashSet<GenerationEvent>();
        private readonly object locker = new object();
        private Timer timer = new Timer(500);

        private DateTime timestamp = DateTime.Now;

        public EventQueue(IVsStatusbar statusBar)
        {
            this.statusBar = statusBar;
            SetupTimer();
        }

        public void Enqueue(Action<GenerationEvent> action, GenerationType type, params string[] paths)
        {
            if (paths[0].EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase) == false) return;

            lock (locker)
            {
                this.timestamp = DateTime.Now;

                var generationEvent = new GenerationEvent { Action = action, Type = type, Paths = paths };
                if (queue.Any(e => e.Equals(generationEvent)))
                {
                    return;
                }

                queue.Add(generationEvent);

                Log.Debug("{0} queued {1}", generationEvent.Type, string.Join(" -> ", generationEvent.Paths));
            }
        }

        private void SetupTimer()
        {
            this.timer.Enabled = true;
            this.timer.Elapsed += (sender, args) =>
            {
                GenerationEvent[] items;

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
                if (count <= 0) return;


                object icon;
                uint cookie;
                
                PrepareStatusbar(out cookie, out icon);
                
                var stopwatch = Stopwatch.StartNew();

                uint i = 0;
                var parallellQueue = items.AsParallel();
                parallellQueue.ForAll(item =>
                {
                    item.Action(item);
                    UpdateStatusbarProgress(ref cookie, ++i, count);
                });

                stopwatch.Stop();
                Log.Debug("Queue flushed in {0} ms", stopwatch.ElapsedMilliseconds);

                ClearStatusbar(cookie, ref icon);
            };
        }

        private void PrepareStatusbar(out uint cookie, out object icon)
        {
            cookie = 0;
            icon = (short)InteropConstants.SBAI_Save;

            try
            {
                int frozen;
                statusBar.IsFrozen(out frozen);

                if (frozen != 0) return;

                statusBar.Progress(ref cookie, 1, string.Empty, 0, 0);
                statusBar.SetText("Rendering template...");
                statusBar.Animation(1, ref icon);
            }
            catch (Exception exception)
            {
                Log.Debug("Failed to prepare statusbar: {0}", exception.Message);
            }
        }

        private void UpdateStatusbarProgress(ref uint cookie, uint current, uint total)
        {
            try
            {
                statusBar.Progress(ref cookie, 1, string.Empty, current, total);
            }
            catch (Exception exception)
            {
                Log.Debug("Failed to update statusbar progress: {0}", exception.Message);
            }
        }

        private void ClearStatusbar(uint cookie, ref object icon)
        {
            try
            {
                //object icon = (short)InteropConstants.SBAI_Save;
                statusBar.Animation(0, ref icon);
                statusBar.SetText("Rendering complete");

                Thread.Sleep(1000);

                statusBar.Progress(ref cookie, 0, "", 0, 0);
            }
            catch (Exception exception)
            {
                Log.Debug("Failed to clear statusbar: {0}", exception.Message);
            }
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

    public enum GenerationType
    {
        Render,
        Delete,
        Rename
    }

    public class GenerationEvent
    {
        public GenerationType Type { get; set; }
        public string[] Paths { get; set; }
        public Action<GenerationEvent> Action { get; set; }

        public bool Equals(GenerationEvent other)
        {
            if (other == null) return false;
            return (Type == other.Type) && Action == other.Action && (Paths.SequenceEqual(other.Paths));
        }
    }
}
