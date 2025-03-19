using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleAccessControlEmulator
{
	class AccessControlDataUpdater : IDisposable
	{
		public AccessControlDataUpdater(AccessControlData data)
		{
			_data = data;

			StartAccessControlPollThread();
		}



		public void Dispose()
		{
			StopExternalDataParseThread();

			_dataUpdaterThreadTerminationEvent.Dispose();
		}


		void StartAccessControlPollThread()
		{
			if (_dataUpdaterThread != null)
				return;

			_dataUpdaterThreadTerminationEvent.Reset();
			_dataUpdaterThread = new Thread(DataUpdaterThreadProc);
			_dataUpdaterThread.Start();
		}

		void StopExternalDataParseThread()
		{
			if (_dataUpdaterThread == null)
				return;

			_dataUpdaterThreadTerminationEvent.Set();
			_dataUpdaterThread.Join();
			_dataUpdaterThread = null;
		}

		void DataUpdaterThreadProc()
		{
			using (var doorsUpdateTimer = new IntervalTriggerTimer())
			using (var zonesUpdateTimer = new IntervalTriggerTimer())
			using (var partitionsUpdateTimer = new IntervalTriggerTimer())
			using (var outputsUpdateTimer = new IntervalTriggerTimer())
			using (var fireEventTimer = new IntervalTriggerTimer())
			{
				var waitHandles = new WaitHandle[]
				{
					_dataUpdaterThreadTerminationEvent,
					doorsUpdateTimer.TimerEvent,
					zonesUpdateTimer.TimerEvent,
					partitionsUpdateTimer.TimerEvent,
					outputsUpdateTimer.TimerEvent,
					fireEventTimer.TimerEvent
				};

				var rnd = new Random();

				var doorsUpdateTimeout = (int)(DoorsUpdateTimeoutMin + (DoorsUpdateTimeoutMax - DoorsUpdateTimeoutMin) * rnd.NextDouble());
				doorsUpdateTimer.Start(new TimeSpan(doorsUpdateTimeout * TimeSpan.TicksPerSecond), false);

				var zonesUpdateTimeout = (int)(ZonesUpdateTimeoutMin + (ZonesUpdateTimeoutMax - ZonesUpdateTimeoutMin) * rnd.NextDouble());
				zonesUpdateTimer.Start(new TimeSpan(zonesUpdateTimeout * TimeSpan.TicksPerSecond), false);

				var partitionsUpdateTimeout = (int)(PartitionsUpdateTimeoutMin + (PartitionsUpdateTimeoutMax - PartitionsUpdateTimeoutMin) * rnd.NextDouble());
				partitionsUpdateTimer.Start(new TimeSpan(partitionsUpdateTimeout * TimeSpan.TicksPerSecond), false);

				var outputsUpdateTimeout = (int)(OutputsUpdateTimeoutMin + (OutputsUpdateTimeoutMax - OutputsUpdateTimeoutMin) * rnd.NextDouble());
				outputsUpdateTimer.Start(new TimeSpan(outputsUpdateTimeout * TimeSpan.TicksPerSecond), false);

				var fireEventTimeout = (int)(FireEventTimeoutMin + (FireEventTimeoutMax - FireEventTimeoutMin) * rnd.NextDouble());
				fireEventTimer.Start(new TimeSpan(fireEventTimeout * TimeSpan.TicksPerSecond), false);

				while (true)
				{
					var wr = WaitHandle.WaitAny(waitHandles);
					if (wr == 0) // _dataUpdaterThreadTerminationEvent
						break;

					if (wr == 1) // doorsUpdateTimer
					{
						_data.UpdateRandomDoorStates();

						doorsUpdateTimeout = (int)(DoorsUpdateTimeoutMin + (DoorsUpdateTimeoutMax - DoorsUpdateTimeoutMin) * rnd.NextDouble());
						doorsUpdateTimer.Start(new TimeSpan(doorsUpdateTimeout * TimeSpan.TicksPerSecond), false);
					}
					else if (wr == 2) // zonesUpdateTimer
					{
						_data.UpdateRandomZoneStates();

						zonesUpdateTimeout = (int)(ZonesUpdateTimeoutMin + (ZonesUpdateTimeoutMax - ZonesUpdateTimeoutMin) * rnd.NextDouble());
						zonesUpdateTimer.Start(new TimeSpan(zonesUpdateTimeout * TimeSpan.TicksPerSecond), false);
					}
					else if (wr == 3) // partitionsUpdateTimer
					{
						_data.UpdateRandomPartitionStates();

						partitionsUpdateTimeout = (int)(PartitionsUpdateTimeoutMin + (PartitionsUpdateTimeoutMax - PartitionsUpdateTimeoutMin) * rnd.NextDouble());
						partitionsUpdateTimer.Start(new TimeSpan(partitionsUpdateTimeout * TimeSpan.TicksPerSecond), false);
					}
					else if (wr == 4) // outputsUpdateTimer
					{
						_data.UpdateRandomOutputStates();

						outputsUpdateTimeout = (int)(OutputsUpdateTimeoutMin + (OutputsUpdateTimeoutMax - OutputsUpdateTimeoutMin) * rnd.NextDouble());
						outputsUpdateTimer.Start(new TimeSpan(outputsUpdateTimeout * TimeSpan.TicksPerSecond), false);
					}
					else if (wr == 5) // fireEventTimer
					{
						_data.FireEvent();

						fireEventTimeout = (int)(FireEventTimeoutMin + (FireEventTimeoutMax - FireEventTimeoutMin) * rnd.NextDouble());
						fireEventTimer.Start(new TimeSpan(fireEventTimeout * TimeSpan.TicksPerSecond), false);
					}
				}
			}
		}



		Thread? _dataUpdaterThread;
		readonly ManualResetEvent _dataUpdaterThreadTerminationEvent = new ManualResetEvent(false);
		readonly AccessControlData _data;

		static readonly int DoorsUpdateTimeoutMin = 10;
		static readonly int DoorsUpdateTimeoutMax = 60;
		static readonly int ZonesUpdateTimeoutMin = 10;
		static readonly int ZonesUpdateTimeoutMax = 60;
		static readonly int PartitionsUpdateTimeoutMin = 10;
		static readonly int PartitionsUpdateTimeoutMax = 60;
		static readonly int OutputsUpdateTimeoutMin = 10;
		static readonly int OutputsUpdateTimeoutMax = 60;
		static readonly int FireEventTimeoutMin = 10;
		static readonly int FireEventTimeoutMax = 60;
	}
}
