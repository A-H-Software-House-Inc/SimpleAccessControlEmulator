namespace SimpleAccessControlEmulator
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var httpPort = 8095;
				if (args.Length > 0)
					int.TryParse(args[0], out httpPort);
				else
					Console.WriteLine($"Application arguments list is empty. Default port {httpPort} will be used.");

				if (httpPort < 0 || httpPort > 65535)
				{
					Console.WriteLine($"Wrong port number {httpPort}.");
					return;
				}

				var accessControlData = new AccessControlData();
				var requestProcessor = new HttpRequestProcessor(accessControlData);

				using (var accessControlDataUpdater = new AccessControlDataUpdater(accessControlData))
				using (var httpServer = new HttpServer(16))
				{
					httpServer.ProcessRequest += (httpListenerContext, stopEventHandle) =>
					{
						requestProcessor.OnProcessRequest(httpListenerContext, stopEventHandle);
					};
					httpServer.Start(httpPort, "Simple Access Control Emulator");

					Console.WriteLine($"Start listening on port {httpPort}.");
					Console.WriteLine("Press any key to exit...");
					Console.ReadKey();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unhandled exception was caught: {ex.Message}.");
			}
		}
	}
}