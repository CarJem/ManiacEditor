using System;

namespace ManiacEditor
{
    public class EditorDiscordRP
	{
		//Used For Discord Rich Pressence
		public SharpPresence.Discord.RichPresence RPCcontrol = new SharpPresence.Discord.RichPresence();
		public SharpPresence.Discord.EventHandlers RPCEventHandler = new SharpPresence.Discord.EventHandlers();
		public string ScenePath = "";
		public Editor Instance;

		//Shorthanding Setting Files
		public Properties.Settings mySettings = Properties.Settings.Default;

		public EditorDiscordRP(Editor instance)
		{
			Instance = instance;
		}
		public void InitDiscord()
		{
			if (!Environment.Is64BitProcess)
			{
				SharpPresence.Discord.Initialize("484279851830870026", RPCEventHandler);

				if (mySettings.ShowDiscordRPC)
				{
					RPCcontrol.state = "Maniac Editor";
					RPCcontrol.details = "Idle";

					RPCcontrol.largeImageKey = "maniac";
					RPCcontrol.largeImageText = "maniac-small";

					TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
					int secondsSinceEpoch = (int)t.TotalSeconds;

					RPCcontrol.startTimestamp = secondsSinceEpoch;

					SharpPresence.Discord.RunCallbacks();
					SharpPresence.Discord.UpdatePresence(RPCcontrol);
				}
				else
				{
					RPCcontrol.state = "Maniac Editor";
					RPCcontrol.details = "";

					RPCcontrol.largeImageKey = "maniac";
					RPCcontrol.largeImageText = "Maniac Editor";

					TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
					int secondsSinceEpoch = (int)t.TotalSeconds;

					RPCcontrol.startTimestamp = secondsSinceEpoch;

					SharpPresence.Discord.RunCallbacks();
					SharpPresence.Discord.UpdatePresence(RPCcontrol);
				}
			}

		}

		public void UpdateDiscord(string Details = null)
		{
			if (!Environment.Is64BitProcess)
			{
				try
				{
					if (mySettings.ShowDiscordRPC)
					{
						SharpPresence.Discord.RunCallbacks();
						if (Details != null)
						{
							RPCcontrol.details = Details;
						}
						else
						{
							RPCcontrol.details = "Idle";
						}
						SharpPresence.Discord.UpdatePresence(RPCcontrol);
					}
					else
					{
						RPCcontrol.state = "Maniac Editor";
						RPCcontrol.details = "";

						RPCcontrol.largeImageKey = "maniac";
						RPCcontrol.largeImageText = "Maniac Editor";

						SharpPresence.Discord.RunCallbacks();
						SharpPresence.Discord.UpdatePresence(RPCcontrol);
					}
				}
				catch
				{

				}
			}
		}

		public void DisposeDiscord()
		{
			RPCcontrol.startTimestamp = 0;
			SharpPresence.Discord.Shutdown();
		}
	}
}
