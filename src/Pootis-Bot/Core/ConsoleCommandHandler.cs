﻿using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Pootis_Bot.Core.Managers;
using Pootis_Bot.Entities;
using Pootis_Bot.Helpers;
using Pootis_Bot.Services.Audio;
using Console = Pootis_Bot.ConsoleCommandHandler.Console;

namespace Pootis_Bot.Core
{
	public class ConsoleCommandHandler : Console
	{
		private readonly DiscordSocketClient _client;

		public ConsoleCommandHandler(DiscordSocketClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Sets up the the Pootis-Bot's <see cref="Console"/> to handle commands and such
		/// </summary>
		public void SetupConsole()
		{
			UnknownCommandError =
				$"Unknown command! Visit {Global.websiteConsoleCommands} for a list of console commands.";
			UnknownCommandErrorColor = ConsoleColor.Red;

			//Add all of our commands
			AddCommand("exit", ExitCmd);
			AddCommand("config", OpenConfigCmd);
			AddCommand("version", VersionCmd);
			AddCommand("about", AboutCmd);
			AddCommand("setgame", SetGameStatusCmd);
			AddCommand("togglestream", SetStreamingStatusCmd);
			AddCommand("deletemusic", DeleteMusicCmd);
			AddCommand("toggleaudio", ToggleAudioCmd);
			AddCommand("forceaudioupdate", ForceAudioUpdateCmd);
			AddCommand("status", StatusCmd);
			AddCommand("clear", ClearCmd);
			AddCommand("resethelpmodules", ResetHelpModulesCmd);
			AddCommand("save config", SaveConfigCmd);
			AddCommand("save accounts", SaveAccountsCmd);
			AddCommand("save servers", SaveServersCmd);

			ConsoleHandleLoop();
		}

		public override void LogMessage(string message, ConsoleColor color)
		{
			Global.Log(message, color);
		}

		private async void ExitCmd()
		{
			IsExiting = true;

			Bot.IsRunning = false;

			Global.Log("Shutting down...");
			await _client.SetGameAsync("Bot shutting down");
			foreach (ServerMusicItem channel in AudioService.currentChannels)
			{
				channel.IsExit = true;

				if (channel.FfMpeg != null)
				{
					channel.FfMpeg.Kill();
					channel.FfMpeg.Dispose();
				}

				//Just wait a moment
				await Task.Delay(100);

				await channel.AudioClient.StopAsync();

				channel.IsPlaying = false;
			}

			Environment.Exit(0);
		}

		private static void OpenConfigCmd()
		{
			new ConfigMenu().OpenConfig();
		}

		private static void VersionCmd()
		{
			Global.Log($"You are running version {VersionUtils.GetAppVersion()} of Pootis-Bot!");
		}

		private static void AboutCmd()
		{
			System.Console.WriteLine(Global.aboutMessage);
		}

		private async void SetGameStatusCmd()
		{
			System.Console.WriteLine("Enter in what you want to set the bot's game to:");
			Global.BotStatusText = System.Console.ReadLine();

			ActivityType activity = ActivityType.Playing;
			string twitch = null;
			if (Bot.IsStreaming)
			{
				activity = ActivityType.Streaming;
				twitch = Config.bot.TwitchStreamingSite;
			}

			await _client.SetGameAsync(Global.BotStatusText, twitch, activity);

			Global.Log($"Bot's game status was set to '{Global.BotStatusText}'");
		}

		private async void SetStreamingStatusCmd()
		{
			if (Bot.IsStreaming)
			{
				Bot.IsStreaming = false;
				await _client.SetGameAsync(Global.BotStatusText, "");
				Global.Log("Bot no longer shows streaming status.");
			}
			else
			{
				Bot.IsStreaming = true;
				await _client.SetGameAsync(Global.BotStatusText, Config.bot.TwitchStreamingSite,
					ActivityType.Streaming);

				Global.Log("Bot now shows streaming status.");
			}
		}

		private static async void DeleteMusicCmd()
		{
			foreach (ServerMusicItem channel in AudioService.currentChannels)
			{
				channel.IsExit = true;

				if (channel.FfMpeg != null)
				{
					channel.FfMpeg.Kill();
					channel.FfMpeg.Dispose();
				}

				//Just wait a moment
				await Task.Delay(100);

				await channel.AudioClient.StopAsync();

				channel.IsPlaying = false;
			}

			AudioService.currentChannels.Clear();

			Global.Log("Deleting music directory...", ConsoleColor.Blue);
			if (Directory.Exists("Music/"))
			{
				Directory.Delete("Music/", true);
				Global.Log("Done!", ConsoleColor.Blue);
			}
			else
			{
				Global.Log("The music directory doesn't exist!", ConsoleColor.Blue);
			}
		}

		private static void ToggleAudioCmd()
		{
			Config.bot.AudioSettings.AudioServicesEnabled = !Config.bot.AudioSettings.AudioServicesEnabled;
			Config.SaveConfig();

			Global.Log($"The audio service was set to {Config.bot.AudioSettings.AudioServicesEnabled}",
				ConsoleColor.Blue);
			if (Config.bot.AudioSettings.AudioServicesEnabled)
				AudioCheckService.CheckAudioService();
		}

		private static async void ForceAudioUpdateCmd()
		{
			Global.Log("Updating audio files.", ConsoleColor.Blue);
			foreach (ServerMusicItem channel in AudioService.currentChannels)
				channel.AudioClient.Dispose();

			await Task.Delay(1000);

			//Delete old files first
			Directory.Delete("External/", true);
			File.Delete("libsodium.dll");
			File.Delete("opus.dll");

			AudioCheckService.UpdateAudioFiles();
			Global.Log("Audio files were updated.", ConsoleColor.Blue);
		}

		private void StatusCmd()
		{
			Global.Log(
				$"Bot status: {_client.ConnectionState.ToString()}\nServer count: {_client.Guilds.Count}\nLatency: {_client.Latency}");
		}

		private static void ClearCmd()
		{
			System.Console.Clear();
		}

		private static void ResetHelpModulesCmd()
		{
			HelpModulesManager.ResetHelpModulesToDefault();
			HelpModulesManager.SaveHelpModules();

			Global.Log("The help modules were reset to there defaults.");
		}

		private static void SaveConfigCmd()
		{
			Config.SaveConfig();
			Global.Log("Config saved!");
		}

		private static void SaveAccountsCmd()
		{
			UserAccountsManager.SaveAccounts();
			Global.Log("User accounts saved!");
		}

		private static void SaveServersCmd()
		{
			ServerListsManager.SaveServerList();
			Global.Log("Server list saved!");
		}
	}
}