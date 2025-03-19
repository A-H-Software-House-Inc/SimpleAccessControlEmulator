using Newtonsoft.Json;

namespace SimpleAccessControlEmulator
{
	class CardHolderData
	{
		[JsonProperty("id")]
		public string? Id { get; set; }

		[JsonProperty("firstName")]
		public string? FirstName { get; set; }

		[JsonProperty("lastName")]
		public string? LastName { get; set; }
	}
}
