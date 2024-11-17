using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands;

public class UnmuteCommand : ApplicationCommandModule
{
    [SlashRequireGuild]
    [SlashCommand("unmute", "Unmutes a player from the server")]
    public async Task OnExecute(InteractionContext command, [Option("SteamID", "Steam ID of the user to unmute.")] string steamID)
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
                Duration = "0",
                DiscordUserID = command.Member.Id,
                Reason = "",
                InteractionID = command.InteractionId,
                DiscordDisplayName = command.Member.DisplayName,
                DiscordUsername = command.Member.Username
            }
        };
        MessageScheduler.CacheInteraction(command);
        await NetworkSystem.SendMessage(message, command);
    }
}