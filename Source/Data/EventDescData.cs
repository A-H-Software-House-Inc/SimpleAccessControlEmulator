﻿using Newtonsoft.Json;

namespace SimpleAccessControlEmulator
{
	class EventDescData
	{
		[JsonProperty("id")]
		public string? Id { get; set; }

		[JsonProperty("description")]
		public string? Description { get; set; }

		[JsonProperty("category")]
		public string? Category { get; set; }
	}
}
