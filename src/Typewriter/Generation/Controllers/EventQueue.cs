﻿using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Typewriter.VisualStudio;
using InteropConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using Task = System.Threading.Tasks.Task;

namespace Typewriter.Generation.Controllers
{

    public interface IEventQueue : IDisposable
    {
        void Enqueue(Action action);
    }

    public sealed class EventQueue : IEventQueue
    {
        private readonly IVsStatusbar _statusBar;
        private readonly BlockingQueue<Action> _queue;
        private readonly Task _queueTask;

        public EventQueue(IVsStatusbar statusBar)
        {
            this._statusBar = statusBar;
            _queue = new BlockingQueue<Action>();

            _queueTask = Task.Run(() => ProcessQueue());
        }

        private void ProcessQueue()
        {
            do
            {
                try
                {
                    var action = _queue.Dequeue();
                    Thread.Sleep(100);

                    PrepareStatusbar(out var icon);

                    try
                    {
                        var stopwatch = Stopwatch.StartNew();
                        action();

                        while (_queue.Count > 0)
                        {
                            action = _queue.Dequeue();
                            action();
                        }

                        stopwatch.Stop();

                        Log.Debug("Queue flushed in {0} ms", stopwatch.ElapsedMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error processing queue: " + ex.Message + "\n" + ex.StackTrace);
                    }

                    ClearStatusbar(icon);
                }
                catch (InvalidOperationException ex)
                {
                    Log.Debug("Queue Closed: " + ex.Message);
                }
            } while (!(_queue.Closed));
        }

        public void Enqueue(Action action)
        {
            _queue.Enqueue(action);
        }

        private void PrepareStatusbar(out object icon)
        {
            icon = (short)InteropConstants.SBAI_Save;
            var tmpIcon = icon;

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                // You're now on the UI thread.

                try
                {

                    _statusBar.IsFrozen(out var frozen);

                    if (frozen != 0)
                        return;

                    _statusBar.SetText("Rendering template...");
                    _statusBar.Animation(1, ref tmpIcon);
                }
                catch (Exception exception)
                {
                    Log.Debug("Failed to prepare statusbar: {0}", exception.Message);
                }
            });
        }

        private void ClearStatusbar(object icon)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                try
                {
                    _statusBar.Animation(0, ref icon);
                    _statusBar.SetText("Rendering complete");
                    _statusBar.FreezeOutput(0);
                }
                catch (Exception exception)
                {
                    Log.Debug("Failed to clear statusbar: {0}", exception.Message);
                }
            });
        }

        private bool disposed;

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            _queue.Close();
            _queueTask.Wait(1000);
            _queueTask.Dispose();
        }
    }

    public enum GenerationType
    {
        Render,
        Delete,
        Rename,
        Template
    }

}
