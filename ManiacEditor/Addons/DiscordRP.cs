using System;
using DiscordRPC;

namespace ManiacEditor
{
    public static class DiscordRP
	{
		#region Event Handlers

		private static bool DisableLogging = true;
		private static void Client_OnReady(object sender, DiscordRPC.Message.ReadyMessage e)
		{
			string output = string.Format("[Discord RPC] Received Ready from user {0}", e.User.Username);
			System.Diagnostics.Debug.Print(output);
			Console.WriteLine(output);
		}

		private static void Client_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage e)
		{
			if (!DisableLogging)
			{
				string output = string.Format("[Discord RPC] Received Update! {0}", e.Presence);
				System.Diagnostics.Debug.Print(output);
				Console.WriteLine(output);
			}
		}

		private static void Client_OnError(object sender, DiscordRPC.Message.ErrorMessage e)
		{
			if (!DisableLogging)
			{
				string output = string.Format("[Discord RPC] Failed Update! {0}", e.Message);
				System.Diagnostics.Debug.Print(output);
				Console.WriteLine(output);
			}
		}
		#endregion

		#region Timer
		private static System.Timers.Timer timer;
		private static TimeSpan UpdateInterval { get; set; } = new TimeSpan();
		private static DateTime StartTime { get; set; } = new DateTime();

		private static void StartTimer()
		{
			UpdateInterval = TimeSpan.FromSeconds(5);
			timer = new System.Timers.Timer(UpdateInterval.TotalMilliseconds);
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
		}


        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			UpdateDiscord(null, true);
		}
		#endregion

		#region Discord
		private static DiscordRpcClient client;
		private static string APP_ID = "657277616482549760";
		private static bool isInitilized = false;

		#region Presence Stats
		private static string CurrentDetails { get; set; } = null;
        #endregion

        private static void Init()
		{
			client = new DiscordRpcClient(APP_ID);
			client.OnReady += Client_OnReady;
			client.OnPresenceUpdate += Client_OnPresenceUpdate;
			client.OnError += Client_OnError;
			client.Initialize();
			isInitilized = true;
		}


        private static void SetPresence(string _details = null)
		{
			client.SetPresence(Presence.GetRichPresence(_details));
		}

		private static void Dispose()
		{
			client.Dispose();
			isInitilized = false;
		}

		public static class Presence
		{
			public static RichPresence GetRichPresence(string _details = null)
			{
				RichPresence richPresence = new RichPresence();
				richPresence.State = "Maniac Editor";
				GetDetails(ref richPresence, _details);
				GetAssets(ref richPresence);
				GetTimestamps(ref richPresence);

				return richPresence;
			}

			public static void GetAssets(ref RichPresence richPresence)
			{
				if (richPresence.Assets == null) richPresence.Assets = new Assets();
				richPresence.Assets.LargeImageKey = "maniac_main";

				//richPresence.Assets.LargeImageText = "";
				//richPresence.Assets.SmallImageKey = "";
				//richPresence.Assets.SmallImageText = "";
			}

			public static void GetDetails(ref RichPresence richPresence, string _details = null)
			{
				if (_details != null)
				{
					richPresence.Details = string.Format("Editing {0}", _details);
				}
				else
				{
					richPresence.Details = "Idle";
				}

			}


			#region Timestamps
			static bool timeStampSet = false;
			public static void GetTimestamps(ref RichPresence richPresence)
			{
				if (!timeStampSet)
				{
					timeStampSet = true;
					Timestamps timestamp = new Timestamps();
					timestamp.Start = Timestamps.Now.Start;
					richPresence.Timestamps = timestamp;
					StartTime = timestamp.Start.Value;
				}
				else
				{
					Timestamps timestamp = new Timestamps();
					timestamp.Start = StartTime;
					richPresence.Timestamps = timestamp;
				}

			}
			#endregion
		}

		#endregion

		#region Public Methods

		public static void InitDiscord()
		{
			StartTimer();
			if (Properties.Settings.MySettings.ShowDiscordRPC) Init();
			UpdateDiscord();
			StartTimer();
		}

		public static void UpdateDiscord(string _details = null, bool isLoop = false)
		{

			if (Properties.Settings.MySettings.ShowDiscordRPC && !isInitilized) Init();
			else if (!Properties.Settings.MySettings.ShowDiscordRPC && isInitilized) DisposeDiscord();

			if (isInitilized)
			{
				if (!isLoop) CurrentDetails = _details;
				SetPresence(CurrentDetails);
				client.Invoke();
			}
		}

		public static void DisposeDiscord()
		{
			timer.Stop();
			if (isInitilized) Dispose();
		}

        #endregion
    }
}
