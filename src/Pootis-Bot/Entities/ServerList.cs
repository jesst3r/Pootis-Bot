﻿using System.Collections.Generic;
using System.Linq;
using Pootis_Bot.Structs.Server;

namespace Pootis_Bot.Entities
{
	public class ServerList
	{
		/// <summary>
		/// The ID of the guild
		/// </summary>
		public ulong GuildId { get; set; }

		/// <summary>
		/// People who are allowed to do owner level commands, the owner of the Discord guild will always override
		/// </summary>
		public List<ulong> GuildOwnerIds { get; set; }

		/// <summary>
		/// How many points to give
		/// </summary>
		public uint PointGiveAmount { get; set; }

		/// <summary>
		/// What is the delay between each give points
		/// </summary>
		public int PointsGiveCooldownTime { get; set; }

		/// <summary>
		/// Channels were the bot isn't allowed to except commands from
		/// </summary>
		public List<ulong> BannedChannels { get; set; }

		/// <summary>
		/// What role to give after a user gets a certain amount of points
		/// </summary>
		public List<ServerRolePoints> ServerRolePoints { get; set; }

		/// <summary>
		/// Command permissions
		/// </summary>
		public List<CommandPermission> CommandPermissions { get; set; }

		/// <summary>
		/// Role to role mentions
		/// </summary>
		public List<ServerRoleToRoleMention> RoleToRoleMentions { get; set; }

		/// <summary>
		/// Auto voice channels
		/// </summary>
		public List<ServerVoiceChannel> AutoVoiceChannels { get; set; }

		/// <summary>
		/// Any active channels that were created from an auto-vc channel
		/// </summary>
		public List<ulong> ActiveAutoVoiceChannels { get; set; }

		/// <summary>
		/// Role gives, key is role to give, Value is role to require, if one
		/// </summary>
		public List<RoleGive> RoleGives { get; set; }

		/// <summary>
		/// Do we have custom welcome/goodbye messages enabled
		/// </summary>
		public bool WelcomeMessageEnabled { get; set; }

		/// <summary>
		/// What is the channel id were we put the messages
		/// </summary>
		public ulong WelcomeChannelId { get; set; }

		/// <summary>
		/// The welcome message
		/// </summary>
		public string WelcomeMessage { get; set; }

		/// <summary>
		/// Is the goodbye message enabled?
		/// </summary>
		public bool GoodbyeMessageEnabled { get; set; }

		/// <summary>
		/// The goodbye message
		/// </summary>
		public string WelcomeGoodbyeMessage { get; set; }

		/// <summary>
		/// Is the rule reaction feature enabled?
		/// </summary>
		public bool RuleEnabled { get; set; }

		/// <summary>
		/// What is the message that needs to be reacted
		/// </summary>
		public ulong RuleMessageId { get; set; }

		/// <summary>
		/// The channel were that gosh darn rule message is located
		/// </summary>
		public ulong RuleMessageChannelId { get; set; }

		/// <summary>
		/// The role that will be given to the user after reacting with the right emoji
		/// </summary>
		public ulong RuleRoleId { get; set; }

		/// <summary>
		/// The emoji that needs to be used
		/// </summary>
		public string RuleReactionEmoji { get; set; }

		/// <summary>
		/// Anti-spam settings
		/// </summary>
		public AntiSpamSettingsInfo AntiSpamSettings { get; set; }

		#region Server Role Points Functions

		/// <summary>
		/// Gets a server role points
		/// </summary>
		/// <param name="pointAmount"></param>
		/// <returns></returns>
		public ServerRolePoints GetServerRolePoints(uint pointAmount)
		{
			IEnumerable<ServerRolePoints> result = from a in ServerRolePoints
				where a.PointsRequired == pointAmount
				select a;

			ServerRolePoints serverRolePoints = result.FirstOrDefault();
			return serverRolePoints;
		}

		#endregion

		#region Permissions Functions

		/// <summary>
		/// Get an command permission
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public CommandPermission GetCommandInfo(string command)
		{
			IEnumerable<CommandPermission> result = from a in CommandPermissions
				where a.Command == command
				select a;

			CommandPermission commandPermission = result.FirstOrDefault();
			return commandPermission;
		}

		#endregion

		#region Role Give Functions

		public RoleGive GetRoleGive(string roleGiveName)
		{
			IEnumerable<RoleGive> result = from a in RoleGives
				where a.Name == roleGiveName
				select a;

			RoleGive roleToRoleMention = result.FirstOrDefault();
			return roleToRoleMention;
		}

		#endregion

		#region Guild Owner Functions

		public ulong GetAGuildOwner(ulong id)
		{
			IEnumerable<ulong> result = from a in GuildOwnerIds
				where a == id
				select a;

			return result.FirstOrDefault();
		}

		#endregion

		public class CommandPermission
		{
			/// <summary>
			/// The name of the command
			/// </summary>
			public string Command { get; set; }

			/// <summary>
			/// Roles that are allowed to use the command
			/// </summary>
			public List<ulong> Roles { get; set; }

			public ulong GetRole(ulong roleId)
			{
				IEnumerable<ulong> result = from a in Roles
					where a == roleId
					select a;

				ulong role = result.FirstOrDefault();
				return role;
			}
		}

		public class AntiSpamSettingsInfo
		{
			/// <summary>
			/// Is the mass user anti-spam feature enabled?
			/// </summary>
			public bool MentionUserEnabled { get; set; }

			/// <summary>
			/// What is the threshold of how many users can be mentioned
			/// </summary>
			public int MentionUsersPercentage { get; set; }

			/// <summary>
			/// How many times can a user violate a role to role mention before warnings are given out
			/// </summary>
			public int RoleToRoleMentionWarnings { get; set; }
		}

		#region Banned Channel Functions

		/// <summary>
		/// Gets a banned channel
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ulong GetBannedChannel(ulong id)
		{
			IEnumerable<ulong> result = from a in BannedChannels
				where a == id
				select a;

			ulong channel = result.FirstOrDefault();
			if (channel == 0) channel = CreateBannedChannel(id);
			return channel;
		}

		/// <summary>
		/// Creates a banned channel
		/// </summary>
		/// <param name="channelId"></param>
		/// <returns></returns>
		public ulong CreateBannedChannel(ulong channelId)
		{
			BannedChannels.Add(channelId);
			return channelId;
		}

		#endregion

		#region Auto VC Functions

		/// <summary>
		/// Gets an auto voice channel
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ServerVoiceChannel GetAutoVoiceChannel(ulong id)
		{
			IEnumerable<ServerVoiceChannel> result = from a in AutoVoiceChannels
				where a.Id == id
				select a;

			ServerVoiceChannel channel = result.FirstOrDefault();
			return channel;
		}

		/// <summary>
		/// Get an active auto voice channel
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ulong GetActiveVoiceChannel(ulong id)
		{
			IEnumerable<ulong> result = from a in ActiveAutoVoiceChannels
				where a == id
				select a;

			ulong channel = result.FirstOrDefault();
			if (channel == 0) channel = 0;
			return channel;
		}

		#endregion

		#region Role To Role Functions

		/// <summary>
		/// Get a role to role ping
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public List<ServerRoleToRoleMention> GetRoleToRoleMention(ulong roleId)
		{
			IEnumerable<ServerRoleToRoleMention> result = from a in RoleToRoleMentions
				where a.RoleId == roleId
				select a;


			List<ServerRoleToRoleMention> roleToRoleMention = result.ToList();
			return roleToRoleMention;
		}

		/// <summary>
		/// Create a role to role ping
		/// </summary>
		/// <param name="roleNotMention"></param>
		/// <param name="role"></param>
		/// <returns></returns>
		public ServerRoleToRoleMention CreateRoleToRoleMention(ulong roleNotMention, ulong role)
		{
			ServerRoleToRoleMention roleToRole = new ServerRoleToRoleMention(roleNotMention, role);
			RoleToRoleMentions.Add(roleToRole);
			return roleToRole;
		}

		#endregion
	}
}