﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Pootis_Bot.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Pootis_Bot.Modules
{
    public class ServerSetup : ModuleBase<SocketCommandContext>
    {
        // Module Infomation
        // Orginal Author   - Creepysin
        // Description      - Helps the server owner set up the bot for use
        // Contributors     - Creepysin, 

        [Command("setup")]
        [Summary("Displays setup info")]
        [RequireOwner]
        public async Task Setup()
        {
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            var server = ServerLists.GetServer(Context.Guild);
            var embed = new EmbedBuilder();

            await Context.Channel.SendMessageAsync("Setup status was set to ur dms");

            embed.WithTitle("Setup");
            embed.WithColor(new Color(255, 81, 168));
            embed.WithDescription($"<:Menu:537572055760109568> Here your setup for **{Context.Guild.Name}**.\nSee [here](https://creepysin.github.io/Pootis-Bot/server-setup/) for more help.\n\n");
            embed.WithThumbnailUrl(Context.Guild.IconUrl);
            embed.WithCurrentTimestamp();

            string welcometitle = "<:Cross:537572008574189578> Welcome Channel Disabled";                           // Welcome Message and channel
            string welocmedes = "Welcome channel is disabled\n";
            if(server.WelcomeMessageEnabled == true && server.WelcomeChannel != 0)
            {
                welcometitle = "<:Check:537572054266806292> Welcome Channel Enabled";
                welocmedes = $"Welcome channel is enabled and is set to channel: {server.WelcomeChannel}\n";
            }
            embed.AddField(welcometitle, welocmedes);

            string rulereactiontitle = "<:Cross:537572008574189578> Rule Reaction Disabled";                        // Rule Reaction feature
            string rulereactiondes = "Rule reaction is disabled.\n";
            if(server.RuleEnabled)
            {
                rulereactiontitle = "<:Check:537572054266806292> Rule Reaction Enabled";
                rulereactiondes = $"The rule reaction feature is enabled and is set to the message id '{server.RuleMessageID}' with the emoji '{server.RuleReactionEmoji}'";
            }
            embed.AddField(rulereactiontitle, rulereactiondes);

            await dm.SendMessageAsync("", false, embed.Build());
        }

        [Command("togglewelcomemessage")]
        [Summary("Enables / Disabled the welcome and goodbye message")]
        [RequireOwner]
        public async Task ToggleWelcomeMessage([Remainder]SocketTextChannel channel = null)
        {
            var server = ServerLists.GetServer(Context.Guild);

            if (server.WelcomeMessageEnabled)
            {
                server.WelcomeMessageEnabled = false;
                await Context.Channel.SendMessageAsync("The welcome message was disabled");
            }
            else
            {
                if(channel == null && Bot._client.GetChannel(server.WelcomeChannel) != null)
                {
                    server.WelcomeMessageEnabled = true;
                    await Context.Channel.SendMessageAsync("The welcome message was enabled");
                }
                else if(channel != null)
                {
                    server.WelcomeMessageEnabled = true;
                    await Context.Channel.SendMessageAsync($"The welcome message was enabled and set the channel to **{channel.Name}**");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("You need to input a channel");
                }
            }

            ServerLists.SaveServerList();
        }
        
        [Command("setupwelcomemessage")]
        [Summary("Setups the welcome message and channel. Use [user] to mention the user. User [server] to insert the server name.")]
        [RequireOwner]
        public async Task SetupWelcomeMessage([Remainder]string message = "")
        {
            var server = ServerLists.GetServer(Context.Guild);
            server.WelcomeMessage = message;

            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"The welcome message was set to '{message}'.");
        }

        [Command("setupgoodbyemessage")]
        [Summary("Sets up the goodbye message")]
        [RequireOwner]
        public async Task SetupGoodbyeMessage([Remainder]string message = "")
        {
            var server = ServerLists.GetServer(Context.Guild);

            if (!string.IsNullOrWhiteSpace(message))
                server.WelcomeGoodbyeMessage = message;

            ServerLists.SaveServerList();

            await Context.Channel.SendMessageAsync($"The goodbye message was set to '{message}'. For this to work the welcome message needs to be set up and enabled");
        }

        [Command("setuprulesmessage")]
        [Summary("Sets the rules message by the message id. Needs to be in the same channel with the message!")]
        [RequireOwner]
        public async Task SetupRuleMessage(ulong id = 0)
        {
            if (id != 0)
            {
                if (Context.Channel.GetMessageAsync(id) != null)
                {
                    var server = ServerLists.GetServer(Context.Guild);
                    server.RuleMessageID = id;

                    ServerLists.SaveServerList();

                    await Context.Channel.DeleteMessageAsync(Context.Message.Id);
                }
                else
                {
                    var message = await Context.Channel.SendMessageAsync("That message doesn't exist");
                    await Task.Delay(2500);
                    await message.DeleteAsync();
                    await Context.Channel.DeleteMessageAsync(Context.Message.Id);
                }
                    
            }
            else
            {
                var message = await Context.Channel.SendMessageAsync("The rules message was disabled");
                var server = ServerLists.GetServer(Context.Guild);
                server.RuleMessageID = 0;

                ServerLists.SaveServerList();

                await Task.Delay(2500);
                await message.DeleteAsync();
                await Context.Channel.DeleteMessageAsync(Context.Message.Id);
            }
                
        }

        [Command("setupruleemoji")]
        [Summary("Sets the rule reaction emoji. MUST BE UNICODE. Refer to here for more details. https://unicode.org/emoji/charts/full-emoji-list.html")]
        [RequireOwner]
        public async Task SetupRuleEmoji(string emoji)
        {
            if(!Global.ContainsUnicodeCharacter(emoji))
            {
                await Context.Channel.SendMessageAsync("This emoji is not unicode. Copy the emoji you want to be reacted with from here: https://unicode.org/emoji/charts/full-emoji-list.html");
            }
            else
            {
                var server = ServerLists.GetServer(Context.Guild);
                server.RuleReactionEmoji = emoji;
                ServerLists.SaveServerList();

                await Context.Channel.SendMessageAsync("The emoji was set to " + emoji);
            }
        }

        [Command("togglerulereaction")]
        [Summary("Enables or disables the rule reaction feature is on or off")]
        [RequireOwner]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task ToggleRuleReaction()
        {
            var server = ServerLists.GetServer(Context.Guild);

            if(server.RuleEnabled) //The rule reaction is enabled, disable it
            {
                server.RuleEnabled = false;
                ServerLists.SaveServerList();

                await Context.Channel.SendMessageAsync("The rule reaction feature was disabled.");
            }
            else //Enable the rule reaction feature, but first check to make sure everthing else is setup first
            {
                if(Context.Guild.Roles.FirstOrDefault(x => x.Name == server.RuleRole) != null) //Check to see if the role exist and is not null
                {
                    if (server.RuleMessageID != 0)
                    {
                        if(Global.ContainsUnicodeCharacter(server.RuleReactionEmoji))
                        {
                            server.RuleEnabled = true;

                            ServerLists.SaveServerList();

                            await Context.Channel.SendMessageAsync("The rule reaction feature is enabled. Make sure that the @ everyone can't access the channels that you want people who reacted can and also that @ everyone can react in that channel.");
                        }
                        else
                            await Context.Channel.SendMessageAsync("The emoji isn't set or is incorrect, use the command `setupruleemoji` to set the emoji.");
                    }
                    else
                        await Context.Channel.SendMessageAsync("The rule message id isn't set, use the command `setuprulesmessage` to set the rule message id.");
                }
                else
                    await Context.Channel.SendMessageAsync($"The reaction role doesn't exist, use the command `setuprulerole` to set the role.");
            }
        }

        [Command("setuprulerole")]
        [Summary("Sets the role to give to the user after they have reacted")]
        [RequireOwner]
        public async Task SetupRuleRole([Remainder] string role)
        {
            if (Context.Guild.Roles.FirstOrDefault(x => x.Name == role) != null)
            {
                var server = ServerLists.GetServer(Context.Guild);
                server.RuleRole = role;

                ServerLists.SaveServerList();

                await Context.Channel.SendMessageAsync("The role was set to " + role);
            }
            else
                await Context.Channel.SendMessageAsync("That role doesn't exist!");
                
        }
    }
}
