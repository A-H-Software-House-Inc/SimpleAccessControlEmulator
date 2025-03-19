namespace SimpleAccessControlEmulator
{
	class IntervalTriggerTimer : IDisposable
	{
		public IntervalTriggerTimer()
		{
			TimerEvent = new AutoResetEvent(false);
		}

		public void Start(TimeSpan triggerInterval, bool isRepeated = true)
		{
			if (_cancellationToken == null)
				_cancellationToken = new CancellationTokenSource();

			_runningTask = Task.Run(async () =>
			{
				while (!_cancellationToken.IsCancellationRequested)
				{
					await Task.Delay(triggerInterval, _cancellationToken.Token);
					TimerEvent?.Set();

					if (!isRepeated)
						break;
				}
			}, _cancellationToken.Token);
		}

		public async void Stop()
		{
			_cancellationToken?.Cancel();
			_cancellationToken?.Dispose();
			_cancellationToken = null;

			if (_runningTask != null && !_runningTask.IsCompleted)
				try
				{
					await _runningTask; // Wait for the task completion
				}
				catch
				{
					// ignore
				}

			_runningTask?.Dispose();
			_runningTask = null;
		}

		public void Dispose()
		{
			Stop();
		}

		public bool IsStarted => _runningTask != null;

		public AutoResetEvent TimerEvent { get; }



		private Task? _runningTask;
		private CancellationTokenSource? _cancellationToken;
	}
}
