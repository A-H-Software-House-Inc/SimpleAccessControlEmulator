using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleAccessControlEmulator
{
	class HttpRequestProcessor
	{
		public HttpRequestProcessor(AccessControlData data)
		{
			_data = data;
		}

		public void OnProcessRequest(HttpListenerContext httpListenerContext, EventWaitHandle stopEventHandle)
		{
			var request = httpListenerContext.Request;
			var response = httpListenerContext.Response;

			Console.WriteLine("Processing {0} request \"{1}\" from {2}", request?.HttpMethod, request?.Url, request?.RemoteEndPoint);

			response.StatusCode = (int)HttpStatusCode.OK;

			var jsonData = string.Empty;
			if (request?.HttpMethod == "POST")
			{
				if (!HandlePostRequest(request.Url?.LocalPath, request.Url?.Query, out jsonData))
					response.StatusCode = (int)HttpStatusCode.NotFound;
			}
			else if (request?.HttpMethod == "GET")
			{
				if (!HandleGetRequest(request.Url?.LocalPath, out jsonData))
					response.StatusCode = (int)HttpStatusCode.NotFound;
			}
			else
				response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;

			if (!string.IsNullOrEmpty(jsonData) && response.OutputStream.CanWrite)
			{
				var jsonDataBytes = Encoding.UTF8.GetBytes(jsonData);

				response.ContentType = "application/json; charset=utf-8";

				response.ContentLength64 = jsonDataBytes.Length;
				response.OutputStream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
				response.OutputStream.Flush();
			}

			response.Close();
		}



		bool HandleGetRequest(string? requestPath, out string? jsonData)
		{
			jsonData = null;

			if (requestPath == null)
				return false;

			if (requestPath == "/doors")
			{
				jsonData = JsonConvert.SerializeObject(_data.DoorsList);
				return true;
			}

			if (requestPath == "/zones")
			{
				jsonData = JsonConvert.SerializeObject(_data.ZonesList);
				return true;
			}

			if (requestPath == "/partitions")
			{
				jsonData = JsonConvert.SerializeObject(_data.PartitionsList);
				return true;
			}

			if (requestPath == "/outputs")
			{
				jsonData = JsonConvert.SerializeObject(_data.OutputsList);
				return true;
			}

			if (requestPath == "/cardholders")
			{
				jsonData = JsonConvert.SerializeObject(_data.CardHoldersList);
				return true;
			}

			if (requestPath == "/events")
			{
				jsonData = JsonConvert.SerializeObject(_data.EventsList);
				return true;
			}

			if (requestPath == "/fired_events")
			{
				jsonData = JsonConvert.SerializeObject(_data.GetFiredEvents());
				return true;
			}

			return false;
		}

		bool HandlePostRequest(string? requestPath, string? query, out string? jsonData)
		{
			jsonData = null;

			if (requestPath == null)
				return false;

			if (requestPath == "/system_action")
			{
				Console.WriteLine("System action was run successfully");
				return true;
			}

			if (requestPath.StartsWith("/door/"))
			{
				var regex = new Regex("^\\/door\\/(.*)\\/");
				var res = regex.Match(requestPath);
				if (res.Success && res.Groups.Count == 2)
				{
					var door = _data.GetDoor(res.Groups[1].Value);
					if (door != null)
					{
						if (requestPath.EndsWith("/lock"))
						{
							var statesHashSet = door.States.ToHashSet();
							statesHashSet.Add("locked");
							statesHashSet.Remove("unlocked");
							door.States = statesHashSet.ToArray();

							_data.SetDoor(door);

							jsonData = JsonConvert.SerializeObject(door);
							return true;
						}

						if (requestPath.EndsWith("/unlock"))
						{
							var statesHashSet = door.States.ToHashSet();
							statesHashSet.Remove("locked");
							statesHashSet.Add("unlocked");
							door.States = statesHashSet.ToArray();

							_data.SetDoor(door);

							jsonData =  JsonConvert.SerializeObject(door);
							return true;
						}
					}
				}
			}

			if (requestPath.StartsWith("/partition/"))
			{
				var regex = new Regex("^\\/partition\\/(.*)\\/");
				var res = regex.Match(requestPath);
				if (res.Success && res.Groups.Count == 2)
				{
					var partition = _data.GetPartition(res.Groups[1].Value);
					if (partition != null)
					{
						if (query == null || !query.EndsWith("pin=123"))
							return false;

						if (requestPath.EndsWith("/arm"))
						{
							var statesHashSet = partition.States.ToHashSet();
							statesHashSet.Add("partitionArmed");
							statesHashSet.Remove("partitionDisarmed");
							partition.States = statesHashSet.ToArray();

							_data.SetPartition(partition);

							jsonData = JsonConvert.SerializeObject(partition);
							return true;
						}

						if (requestPath.EndsWith("/disarm"))
						{
							var statesHashSet = partition.States.ToHashSet();
							statesHashSet.Remove("partitionArmed");
							statesHashSet.Add("partitionDisarmed");
							partition.States = statesHashSet.ToArray();

							_data.SetPartition(partition);

							jsonData = JsonConvert.SerializeObject(partition);
							return true;
						}
					}
				}
			}

			return false;
		}



		readonly AccessControlData _data;
	}
}
