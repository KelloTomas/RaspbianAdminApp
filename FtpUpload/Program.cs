using Renci.SshNet;
using System;
using System.IO;

namespace FtpUpload
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Reading IP from file IP.txt");
			try
			{
				using (StreamReader file = new StreamReader(@"IP.txt"))
				{
					string IP;
					while ((IP = file.ReadLine()) != null)
					{
						try
						{
							UpdateApp(IP);
						}
						catch (Exception e)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine($"     FAIL");
							Console.ForegroundColor = ConsoleColor.White;
							Console.WriteLine($"      Error occured: {e.Message}");
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error occured: {e.Message}");
			}
			Console.WriteLine("Press any key to finish");
			Console.ReadKey();
		}

		private static void UpdateApp(string IP)
		{
			Console.WriteLine($"Connection to: {IP}");
			ConnectionInfo ConnNfo = new ConnectionInfo(IP, 22, "pi",
							new PasswordAuthenticationMethod("pi", "D1amant"));
			ConnNfo.Timeout = new TimeSpan(0, 0, 3);
			Console.Write("   Removing old file... ");
			ExecudeCommand(ConnNfo, "rm QTTcpServer");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"  OK");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("   Uploading new program... ");
			UploadFile(ConnNfo, "QTTcpServer");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"  OK");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("   Making new file executable...");
			ExecudeCommand(ConnNfo, "chmod 777 QTTcpServer");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"  OK");
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void ExecudeCommand(ConnectionInfo ConnNfo, string command)
		{
			using (var sshclient = new SshClient(ConnNfo))
			{
				sshclient.Connect();
				using (var cmd = sshclient.CreateCommand(command))
				{
					cmd.Execute();
				}
				sshclient.Disconnect();
			}
		}

		private static void UploadFile(ConnectionInfo ConnNfo, string filename)
		{
			using (var client = new SftpClient(ConnNfo))
			{
				client.Connect();
				using (var fileStream = new MemoryStream(global::FtpUpload.Properties.Resources.QTTcpServer))
				{
					client.BufferSize = 1024 * 1024;
					client.UploadFile(fileStream, filename);
				}
				client.Disconnect();
			}

		}
	}
}
