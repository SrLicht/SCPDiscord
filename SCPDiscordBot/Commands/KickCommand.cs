using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class KickCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("kick", "Kicks a player from the server.")]
        public async Task OnExecute(InteractionContext command,
          [Option("SteamID", "Steam ID of the user to kick.")] string steamID,
          [Option("Reason", "Reason for the kick.")] string reason = "")
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
                KickCommand = new Interface.KickCommand
                {
                    ChannelID = command.Channel.Id,
                    SteamID = parsedSteamID.ToString(),
                    Reason = reason,
                    InteractionID = command.InteractionId,
                    DiscordDisplayName = command.Member.DisplayName,
                    DiscordUsername = command.Member.Username,
                    DiscordUserID = command.Member.Id
                }
            };
            MessageScheduler.CacheInteraction(command);
            await NetworkSystem.SendMessage(message, command);
        }
    }
}