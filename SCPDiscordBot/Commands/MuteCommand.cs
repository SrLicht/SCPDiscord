using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands;

public class MuteCommand : ApplicationCommandModule
{
    [SlashRequireGuild]
    [SlashCommand("mute", "Mutes a player on the server")]
    public async Task OnExecute(InteractionContext command,
      [Option("SteamID", "Steam ID of the player to mute.")] string steamID,
      [Option("Duration", "Mute duration (ex: 2d is 2 days).")] string duration,
      [Option("Reason", "Reason for the mute.")] string reason)
    {
        if (!Utilities.IsPossibleSteamID(steamID, out ulong parsedSteamID))
        {
            DiscordEmbed error = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Description = "That SteamID doesn't seem to be valid."
            };
            await command.CreateResponseAsync(error);
            return;
        }

        await command.DeferAsync();
        Interface.MessageWrapper message = new Interface.MessageWrapper
        {
            MuteCommand = new Interface.MuteCommand
            {
                ChannelID = command.Channel.Id,
                SteamID = parsedSteamID.ToString(),
                Duration = duration,
                DiscordUserID = command.Member.Id,
                Reason = reason,
                InteractionID = command.InteractionId,
                DiscordDisplayName = command.Member.DisplayName,
                DiscordUsername = command.Member.Username
            }
        };

        MessageScheduler.CacheInteraction(command);
        await NetworkSystem.SendMessage(message, command);
    }
}