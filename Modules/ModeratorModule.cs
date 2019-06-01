﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Abyss.Attributes;
using Abyss.Checks.Command;
using Abyss.Checks.Parameter;
using Abyss.Entities;
using Abyss.Extensions;
using Abyss.Results;
using Qmmands;

namespace Abyss.Modules
{
    [Name("Moderation")]
    [Description("Commands that help you moderate and protect your server.")]
    public class ModeratorModule : AbyssModuleBase
    {
        [Command("Ban", "B")]
        [Description("Bans a member from this server.")]
        [Example("ban pyjamaclub Being stupid.", "ban \"The Mightiest One\" Breaking rule 5.", "ban pyjamaclub")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task<ActionResult> BanUserAsync(
            [Name("Victim")] [Description("The user to ban.")] [MustNotBeBot] [MustNotBeInvoker]
            SocketGuildUser target,
            [Name("Ban Reason")] [Description("The audit log reason for the ban.")] [Remainder] [Maximum(50)]
            string reason = null)
        {
            if (target.Hierarchy > Context.Invoker.Hierarchy)
                return BadRequest("That member is a higher rank than you!");

            try
            {
                await target.BanAsync(7,
                    $"{Context.Invoker} ({Context.Invoker.Id}){(reason != null ? $": {reason}" : "")}").ConfigureAwait(false);
            }
            catch (HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
            {
                return BadRequest(
                    $"Cannot ban '{target.Nickname ?? target.Username}' because that user is more powerful than me!");
            }

            return Ok(
                $"Banned {target.Format()}{(reason != null ? " with reason: " + reason : "")}.");
        }

        [Command("Bans")]
        [Example("bans")]
        [Description("Shows a list of all users banned in this server.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<ActionResult> Command_GetBansListAsync()
        {
            var bans = await Context.Guild.GetBansAsync().ConfigureAwait(false);

            return Ok(a =>
            {
                a.WithAuthor($"Bans List for {Context.Guild.Name}");
                a.WithDescription(bans.Count > 0
                    ? string.Join("\n", bans.Select(b => $"{b.User} - Reason: \"{b.Reason}\""))
                    : "No bans");
            });
        }

        [Command("Clear")]
        [Description("Clears a number of messages from a source message, in a certain direction.")]
        [Example("clear 100")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task<ActionResult> Command_ClearAllAsync(
            [Name("Count")] [Description("The number of messages to delete.")] [Range(1, 100)]
            int count = 50
        )
        {
            var messages =
                (await Context.Channel.GetMessagesAsync(Context.Message.Id, Direction.Before, count).FlattenAsync().ConfigureAwait(false))
                .ToArray();

            await Context.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);

            return Ok(a => a.WithDescription($"Deleted `{messages.Length}` messages."));
        }
    }
}