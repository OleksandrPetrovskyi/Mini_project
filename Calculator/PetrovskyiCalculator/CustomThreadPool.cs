namespace Calculator
{
    public delegate void WithParameter(object? parameter);
    internal class CustomThreadPool
    {
        private Thread[] _Threads;

        private readonly Queue<(Action<object?> Work, object? Parameter)> _Works = new();
        private readonly AutoResetEvent _WorkingEvent = new(false);
        private readonly AutoResetEvent _ExecuteEvent = new(true);

        public void Initialization(int maxThreads)
        {
            if (maxThreads <= 0)
                throw new Exception($"Threads in thread pull must be more than 1.{Environment.NewLine}");

            _Threads = new Thread[maxThreads];
            for (int i = 0; i < maxThreads; i++)
            {
                _Threads[i] = new Thread(ThreadManager);
                _Threads[i].IsBackground = true;
                _Threads[i].Start();
            }
        }

        public void Execute(Action Work) => Execute(_ => Work(), null);

        public void Execute(Action<object?> Work, object? Parameter)
        {
            _ExecuteEvent.WaitOne();

            _Works.Enqueue((Work, Parameter));
            _ExecuteEvent.Set();

            _WorkingEvent.Set();
        }

        private void ThreadManager()
        {
            _WorkingEvent.WaitOne();

            _ExecuteEvent.WaitOne();

            var (work, parameter) = _Works.Dequeue();    //It is cortege

            _ExecuteEvent.Set();

            work(parameter);    //Cortege is used
        }
    }
}