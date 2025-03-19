﻿using Newtonsoft.Json;

namespace SimpleAccessControlEmulator
{
	class ZoneData
	{
		[JsonProperty("id")]
		public string? Id { get; set; }

		[JsonProperty("title")]
		public string? Title { get; set; }

		[JsonProperty("states")]
		public string[] States { get; set; } = [];



		public object Clone()
		{
			return new ZoneData
			{
				Id = Id?.Clone() as string,
				Title = Title?.Clone() as string,
				States = (string[])States.Clone()
			};
		}
	}
}
