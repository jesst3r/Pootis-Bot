﻿namespace Pootis_Bot.ConsoleCommandHandler
{
	public class ConsoleCommand
	{
		public string CommandName { get; set; }
		public string CommandSummary { get; set; }
		public Method Method { get; set; }

		public ConsoleCommand(string name, string summary, Method method)
		{
			CommandName = name;
			CommandSummary = summary;
			Method = method;
		}
	}
}