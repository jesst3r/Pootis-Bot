﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Pootis_Bot.Core;
using Pootis_Bot.Core.Logging;

namespace Pootis_Bot.Modules.BotOwner
{
	public class BotThrowException : ModuleBase<SocketCommandContext>
	{
		// Module Information
		// Original Author  - Creepysin
		// Description      - Makes the bot thrown an exception
		// Contributors     - Creepysin, 

		[Command("throwexception")]
		[Alias("throwexcept")]
		[Summary("Makes the bot throw an exception")]
		[RequireOwner]
#pragma warning disable 1998
		public async Task ThrowExcept([Remainder] string message = "Manually thrown exception")
#pragma warning restore 1998
		{
			Logger.Log($"Manually thrown exception at: {Global.TimeNow()}.", LogVerbosity.Warn);
			throw new Exception(message);
		}
	}
}