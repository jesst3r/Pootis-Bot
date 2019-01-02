﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Pootis_Bot.Core.ServerList;

namespace Pootis_Bot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string seletion = options[r.Next(0, options.Length)];
            await Context.Channel.SendMessageAsync("Choice for " + Context.Message.Author.Mention + "\nI Choose: " + seletion);
        }

        [Command("creepysin")]
        public async Task Creepysin()
        {
            await Context.Channel.SendMessageAsync("Heres creepysin channel: \n https://bit.ly/2KfKeAf");
        }

        [Command("embedmessage")]
        [Alias("embed")]
        public async Task CmdEmbedMessage(string title = "", [Remainder]string msg = "")
        {
            var server = ServerLists.GetServer(Context.Guild);
            var _user = Context.User as SocketGuildUser;
            var setrole = (_user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == server.permEmbedMessage);

            if(server.permEmbedMessage != null && server.permEmbedMessage.Trim() != "")
            {
                if (_user.Roles.Contains(setrole))
                {
                    await Context.Channel.SendMessageAsync("", false, EmbedMessage(title, msg).Build());
                }
            }
            else
                await Context.Channel.SendMessageAsync("", false, EmbedMessage(title, msg).Build());
        }

        #region Functions

        EmbedBuilder EmbedMessage(string title, string msg)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(msg);

            return embed;
        }

        #endregion
    }
}
