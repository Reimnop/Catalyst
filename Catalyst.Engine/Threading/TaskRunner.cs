namespace Catalyst.Engine.Threading;

/// <summary>
/// Runs a block of code on a self-contained thread.
/// </summary>
public class TaskRunner : IDisposable
{
    private readonly Thread thread;
    private readonly ManualResetEvent signalEvent = new ManualResetEvent(false);
    
    public bool IsBusy { get; private set; }

    private bool isRunning = true;
    
    private Action? currentAction;

    public TaskRunner()
    {
        thread = new Thread(Run);
        thread.IsBackground = true;
        thread.Start();
    }

    private void Run()
    {
        while (isRunning)
        {
            signalEvent.WaitOne();
            
            IsBusy = true;
            if (currentAction != null)
            {
                currentAction.Invoke();
                currentAction = null;
            }
            IsBusy = false;
        }
    }
    
    /// <summary>
    /// Runs the specified action on the thread.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <exception cref="InvalidOperationException">Thrown when this method is called when the thread is busy.</exception>
    public void Run(Action action)
    {
        if (IsBusy)
        {
            throw new InvalidOperationException("TaskRunner is already busy.");
        }
        
        currentAction = action;
        signalEvent.Set();
    }

    public void Dispose()
    {
        isRunning = false;
        signalEvent.Set();
        thread.Join();
    }
}