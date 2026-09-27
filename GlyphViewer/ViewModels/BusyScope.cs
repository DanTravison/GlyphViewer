namespace GlyphViewer.ViewModels;

using GlyphViewer.Diagnostics;
using GlyphViewer.ObjectModel;
using System.ComponentModel;

/// <summary>
/// Provides a class for managing the UI busy state.
/// </summary>
internal sealed class BusyScope : ObservableObject
{
    private int _busyCount;

    class BusyToken : IDisposable
    {
        private BusyScope _scope;
        private readonly string _operation;

        public BusyToken(BusyScope scope, string operation)
        {
            _scope = scope;
            _operation = operation;
            if (!string.IsNullOrEmpty(operation))
            {
                Trace.Value(TraceFlag.Application, this, "Begin", operation);
            }
        }

        public void Dispose()
        {
            if (_scope is not null)
            {
                BusyScope scope = _scope;
                _scope = null;
                if (!string.IsNullOrEmpty(_operation))
                {
                    Trace.Value(TraceFlag.Application, this, "End", _operation);
                }
                scope.Leave();
            }
        }
    }

    /// <summary>
    /// Gets the value indicating if the scope is busy.
    /// </summary>
    public bool IsBusy
    {
        get => _busyCount > 0;
    }

    /// <summary>
    /// Enters the busy scope.
    /// </summary>
    /// <param name="operation">The optional name of the operation.</param>
    /// <returns>An <see cref="IDisposable"/> to dispose to leave the scope.</returns>
    public async Task<IDisposable> EnterAsync(string operation = null)
    {
        _busyCount++;
        if (_busyCount == 1)
        {
            if (!string.IsNullOrEmpty(operation))
            {
                Trace.Value(TraceFlag.Application, this, nameof(EnterAsync));
            }
            OnPropertyChanged(IsBusyChangedEventArgs);
            // give the ui thread a chance to process.
            await Task.Yield();
        }
        return new BusyToken(this, operation);
    }

    /// <summary>
    /// Invoked by the <see cref="BusyToken"/> to leave the caller's scope.
    /// </summary>
    private void Leave()
    {
        if (_busyCount > 0)
        {
            _busyCount--;
            if (_busyCount == 0)
            {
                OnPropertyChanged(IsBusyChangedEventArgs);
                Trace.Value(TraceFlag.Application, this, nameof(Leave));
            }
        }
    }

    /// <summary>
    /// Provides a <see cref="PropertyChangedEventArgs"/> for <see cref="IsBusy"/>.
    /// </summary>
    private static readonly PropertyChangedEventArgs IsBusyChangedEventArgs = new(nameof(IsBusy));
}

