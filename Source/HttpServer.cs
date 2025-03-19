using System.Net;

namespace SimpleAccessControlEmulator
{
	class HttpServer : IDisposable
	{
		public HttpServer(int maxThreads)
		{
			_workers = new Thread[maxThreads];
			_contextQueue = new Queue<HttpListenerContext>();
			_stop = new ManualResetEvent(false);
			_ready = new ManualResetEvent(false);
			_listener = new HttpListener();
			_listenerThread = new Thread(ProcessRequests);
		}

		public void Dispose()
		{
			Stop();

			_stop.Dispose();
			_ready.Dispose();
			_listener.Close();
		}



		public event Action<HttpListenerContext, EventWaitHandle>? ProcessRequest;



		public void Start(int port, string realm)
		{
			_listener.Prefixes.Add($"http://*:{port}/");

			_listener.Realm = realm;
			_listener.Start();
			_listenerThread.Start();

			for (int i = 0; i < _workers.Length; i++)
			{
				_workers[i] = new Thread(Worker);
				_workers[i].Start();
			}
		}

		public void Stop()
		{
			_stop.Set();

			if (_listenerThread.IsAlive)
				_listenerThread.Join();

			foreach (var worker in _workers)
			{
				if (worker.IsAlive)
					worker.Join();
			}

			if (_listener.IsListening)
				_listener.Stop();
		}

		private void ProcessRequests()
		{
			WaitHandle[] handles = [_stop, null];

			while (_listener.IsListening)
			{
				var context = _listener.BeginGetContext(OnContextReady, null);

				handles[1] = context.AsyncWaitHandle;

				if (0 == WaitHandle.WaitAny(handles))
					return;
			}
		}

		private void OnContextReady(IAsyncResult asyncResult)
		{
			try
			{
				lock (_contextQueue)
				{
					_contextQueue.Enqueue(_listener.EndGetContext(asyncResult));
					_ready.Set();
				}
			}
			catch
			{
				// ignored
			}
		}

		private void Worker()
		{
			WaitHandle[] handles = [_ready, _stop];

			while (0 == WaitHandle.WaitAny(handles))
			{
				HttpListenerContext context;
				lock (_contextQueue)
				{
					if (_contextQueue.Count > 0)
					{
						context = _contextQueue.Dequeue();
					}
					else
					{
						_ready.Reset();
						continue;
					}
				}

				try
				{
					ProcessRequest?.Invoke(context, _stop);
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
				}
			}
		}



		private readonly HttpListener _listener;
		private readonly Thread _listenerThread;
		private readonly Thread[] _workers;
		private readonly ManualResetEvent _stop;
		private readonly ManualResetEvent _ready;
		private readonly Queue<HttpListenerContext> _contextQueue;
	}
}
