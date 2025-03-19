namespace SimpleAccessControlEmulator
{
	class AccessControlData
	{
		public AccessControlData()
		{
			GenerateDoors(5);
			GenerateZones(5);
			GeneratePartitions(5);
			GenerateOutputs(5);
			GenerateCardHolders(5);
			GenerateEvents(10);
		}



		public List<DoorData> DoorsList { get; private set; } = new List<DoorData>();

		public List<ZoneData> ZonesList { get; private set; } = new List<ZoneData>();

		public List<PartitionData> PartitionsList { get; private set; } = new List<PartitionData>();

		public List<OutputData> OutputsList { get; private set; } = new List<OutputData>();

		public List<CardHolderData> CardHoldersList { get; private set; } = new List<CardHolderData>();

		public List<EventDescData> EventsList { get; private set; } = new List<EventDescData>();



		public DoorData? GetDoor(string id)
		{
			lock (DoorsList)
			{
				var door = DoorsList.FirstOrDefault(d => d.Id == id);
				return door?.Clone() as DoorData;
			}
		}

		public void SetDoor(DoorData door)
		{
			lock (DoorsList)
			{
				for (var n = 0; n < DoorsList.Count; ++n)
				{
					if (DoorsList[n].Id == door.Id)
					{
						DoorsList[n] = door;
						return;
					}
				}
			}
		}

		public ZoneData? GetZone(string id)
		{
			lock (ZonesList)
			{
				var zone = ZonesList.FirstOrDefault(d => d.Id == id);
				return zone?.Clone() as ZoneData;
			}
		}

		public void SetZone(ZoneData zone)
		{
			lock (ZonesList)
			{
				for (var n = 0; n < ZonesList.Count; ++n)
				{
					if (ZonesList[n].Id == zone.Id)
					{
						ZonesList[n] = zone;
						return;
					}
				}
			}
		}

		public PartitionData? GetPartition(string id)
		{
			lock (PartitionsList)
			{
				var partition = PartitionsList.FirstOrDefault(d => d.Id == id);
				return partition?.Clone() as PartitionData;
			}
		}

		public void SetPartition(PartitionData partition)
		{
			lock (PartitionsList)
			{
				for (var n = 0; n < PartitionsList.Count; ++n)
				{
					if (PartitionsList[n].Id == partition.Id)
					{
						PartitionsList[n] = partition;
						return;
					}
				}
			}
		}

		public OutputData? GetOutput(string id)
		{
			lock (OutputsList)
			{
				var output = OutputsList.FirstOrDefault(d => d.Id == id);
				return output?.Clone() as OutputData;
			}
		}

		public void SetOutput(OutputData output)
		{
			lock (OutputsList)
			{
				for (var n = 0; n < OutputsList.Count; ++n)
				{
					if (OutputsList[n].Id == output.Id)
					{
						OutputsList[n] = output;
						return;
					}
				}
			}
		}

		public List<FireEventData> GetFiredEvents()
		{
			var firedEventsList = new List<FireEventData>();
			lock (_firedEventsList)
			{
				foreach (var e in _firedEventsList)
					firedEventsList.Add(e);
				_firedEventsList.Clear();
			}

			return firedEventsList;
		}

		public void UpdateRandomDoorStates()
		{
			lock (DoorsList)
			{
				var door = DoorsList[_rnd.Next() % DoorsList.Count];
				var statesIndex = _rnd.Next() % PossibleDoorStates.Length;
				door.States = PossibleDoorStates[statesIndex];
			}
		}

		public void UpdateRandomZoneStates()
		{
			lock (ZonesList)
			{
				var zone = ZonesList[_rnd.Next() % ZonesList.Count];
				var statesIndex = _rnd.Next() % PossibleZoneStates.Length;
				zone.States = PossibleZoneStates[statesIndex];
			}
		}

		public void UpdateRandomPartitionStates()
		{
			lock (PartitionsList)
			{
				var partition = PartitionsList[_rnd.Next() % PartitionsList.Count];
				var statesIndex = _rnd.Next() % PossiblePartitionStates.Length;
				partition.States = PossiblePartitionStates[statesIndex];
			}
		}

		public void UpdateRandomOutputStates()
		{
			lock (OutputsList)
			{
				var output = OutputsList[_rnd.Next() % OutputsList.Count];
				var statesIndex = _rnd.Next() % PossibleOutputStates.Length;
				output.States = PossibleOutputStates[statesIndex];
			}
		}

		public void FireEvent()
		{
			EventDescData? eventDesc = null;
			lock (EventsList)
			{
				var eventIndex = _rnd.Next() % EventsList.Count;
				eventDesc = EventsList[eventIndex];
			}

			CardHolderData? cardHolder = null;
			lock (CardHoldersList)
			{
				var cardHolderIndex = _rnd.Next() % CardHoldersList.Count;
				cardHolder = CardHoldersList[cardHolderIndex];
			}

			string? objectId = null;
			if (eventDesc.Category == "door")
			{
				lock (DoorsList)
				{
					var door = DoorsList[_rnd.Next() % DoorsList.Count];
					objectId = door.Id;
				}
			}
			else if (eventDesc.Category == "zone")
			{
				lock (ZonesList)
				{
					var zone = ZonesList[_rnd.Next() % ZonesList.Count];
					objectId = zone.Id;
				}
			}
			else if (eventDesc.Category == "partition")
			{
				lock (PartitionsList)
				{
					var partition = PartitionsList[_rnd.Next() % PartitionsList.Count];
					objectId = partition.Id;
				}
			}

			var fireEvent = new FireEventData
			{
				EventId = eventDesc.Id,
				UtcTime = DateTime.UtcNow.ToFileTimeUtc(),
				CardHolderId = cardHolder.Id,
				ObjectType = eventDesc.Category,
				ObjectId = objectId
			};

			lock (_firedEventsList)
			{
				_firedEventsList.Add(fireEvent);
			}
		}



		void GenerateDoors(uint amount)
		{
			var doorsList = new List<DoorData>();
			for (uint n = 0; n < amount; ++n)
			{
				var door = new DoorData();
				door.Id = $"door-{n + 1}";
				door.Title = $"Door #{n + 1}";

				var statesIndex = _rnd.Next() % PossibleDoorStates.Length;
				door.States = PossibleDoorStates[statesIndex];

				doorsList.Add(door);
			}

			DoorsList = doorsList;
		}

		void GenerateZones(uint amount)
		{
			var zonesList = new List<ZoneData>();
			for (uint n = 0; n < amount; ++n)
			{
				var zone = new ZoneData();
				zone.Id = $"zone-{n + 1}";
				zone.Title = $"Zone #{n + 1}";

				var statesIndex = _rnd.Next() % PossibleZoneStates.Length;
				zone.States = PossibleZoneStates[statesIndex];

				zonesList.Add(zone);
			}

			ZonesList = zonesList;
		}

		void GeneratePartitions(uint amount)
		{
			var partitionsList = new List<PartitionData>();
			for (uint n = 0; n < amount; ++n)
			{
				var partition = new PartitionData();
				partition.Id = $"partition-{n + 1}";
				partition.Title = $"Partition #{n + 1}";

				var statesIndex = _rnd.Next() % PossiblePartitionStates.Length;
				partition.States = PossiblePartitionStates[statesIndex];

				partitionsList.Add(partition);
			}

			PartitionsList = partitionsList;
		}

		void GenerateOutputs(uint amount)
		{
			var outputsList = new List<OutputData>();
			for (uint n = 0; n < amount; ++n)
			{
				var output = new OutputData();
				output.Id = $"output-{n + 1}";
				output.Title = $"Output #{n + 1}";

				var statesIndex = _rnd.Next() % PossibleOutputStates.Length;
				output.States = PossibleOutputStates[statesIndex];

				outputsList.Add(output);
			}

			OutputsList = outputsList;
		}

		void GenerateCardHolders(uint amount)
		{
			var cardHoldersList = new List<CardHolderData>();
			for (uint n = 0; n < amount; ++n)
			{
				var cardHolder = new CardHolderData();
				cardHolder.Id = $"cardholder-{n + 1}";
				cardHolder.FirstName = $"Name{n + 1}";
				cardHolder.LastName = $"Surname{n + 1}";

				cardHoldersList.Add(cardHolder);
			}

			CardHoldersList = cardHoldersList;
		}

		void GenerateEvents(uint amount)
		{
			string[] possibleEventCategories =
			[
				"door",
				"partition",
				"zone",
				"output",
				"system"
			];

			var eventsList = new List<EventDescData>();
			for (uint n = 0; n < amount; ++n)
			{
				var category = possibleEventCategories[_rnd.Next() % possibleEventCategories.Length];

				var e = new EventDescData();
				e.Id = $"event-{n + 1}";
				e.Description = $"Sample description of {category} event #{n + 1}";
				e.Category = category;

				eventsList.Add(e);
			}

			EventsList = eventsList;
		}



		readonly List<FireEventData> _firedEventsList = new List<FireEventData>();
		readonly Random _rnd = new Random();



		static readonly string[][] PossibleDoorStates =
		[
			["closed", "locked"],
			["opened", "unlocked"],
			["closed", "unlocked"]
		];

		static readonly string[][] PossibleZoneStates =
		[
			["zoneArmed"],
			["zoneDisarmed"],
			["zoneReady"],
			["zoneUnbypassed"],
			["zoneReady", "zoneDisarmed"],
			["zoneReady", "zoneArmed"]
		];

		static readonly string[][] PossiblePartitionStates =
		[
			["partitionArmed"],
			["partitionDisarmed"],
			["partitionInAlarm"],
			["partitionReady"],
			["partitionArmed", "partitionInAlarm"]
		];

		static readonly string[][] PossibleOutputStates =
		[
			["outputActivated"],
			["outputDeactivated"]
		];
	}
}
