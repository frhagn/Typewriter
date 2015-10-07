using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using Typewriter.VisualStudio;
using InteropConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace Typewriter.Generation.Controllers
{
    public sealed class EventQueue : IDisposable
    {
        private readonly IVsStatusbar statusBar;
        private readonly BlockingQueue<GenerationEvent> _queue;
        private readonly Task _queueTask;

        public EventQueue(IVsStatusbar statusBar)
        {
            this.statusBar = statusBar;
            _queue = new BlockingQueue<GenerationEvent>();

            _queueTask = Task.Run(() => ProcessQueue());
        }

        private void ProcessQueue()
        {
            do
            {
                try
                {
                    var generationEvent = _queue.Dequeue();

                    Thread.Sleep(100);

                    try
                    {

                        var count = (uint)_queue.Count+1;
                        
                        object icon;
                        uint cookie;

                        PrepareStatusbar(out cookie, out icon);

                        var stopwatch = Stopwatch.StartNew();
                        generationEvent.Action(generationEvent);
                        uint i = 1;
                        while (_queue.Count > 0)
                        {
                            generationEvent = _queue.Dequeue();

                            generationEvent.Action(generationEvent);

                            UpdateStatusbarProgress(ref cookie, ++i, count);

                        }
                        
                        stopwatch.Stop();

                        Log.Debug("Queue flushed in {0} ms", stopwatch.ElapsedMilliseconds);

                        ClearStatusbar(cookie, ref icon);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error processing queue: " + ex.Message);
                    }


                }
                catch (InvalidOperationException ex)
                {
                    Log.Debug("Queue Closed: " + ex.Message);
                }



            } while (!(_queue.Closed));

        }


        public void Enqueue(Action<GenerationEvent> action, GenerationType type, params string[] paths)
        {
            if (paths[0].EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase) == false) return;
            
            var generationEvent = new GenerationEvent { Action = action, Type = type, Paths = paths };

            var added = _queue.EnqueueIfNotContains(generationEvent);
            if (added)
            {

                Log.Debug("{0} queued {1}", generationEvent.Type, string.Join(" -> ", generationEvent.Paths));
            }
            else
            {
                Log.Debug("{0} already in queue {1}", generationEvent.Type, string.Join(" -> ", generationEvent.Paths));

            }

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
            _queue.Close();
            _queueTask.Dispose();
        }
    }

    public enum GenerationType
    {
        Render,
        Delete,
        Rename
    }

    public class GenerationEvent : IEquatable<GenerationEvent>
    {
        public GenerationType Type { get; set; }
        public string[] Paths { get; set; }
        public Action<GenerationEvent> Action { get; set; }

        public bool Equals(GenerationEvent other)
        {
            if (other == null) return false;
            return (Type == other.Type) && (Paths.SequenceEqual(other.Paths));
        }
    }
}
