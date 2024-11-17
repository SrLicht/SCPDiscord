using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class BanCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("ban", "Bans a player from the server")]
        public async Task OnExecute(InteractionContext command,
          [Option("SteamID", "Steam ID of the player to ban.")] string steamID,
          [Option("Duration", "Ban duration (ex: 2d is 2 days).")] string duration,
          [Option("Reason", "Reason for the ban.")] string reason)
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
                BanCommand = new Interface.BanCommand
                {
                    ChannelID = command.Channel.Id,
                    SteamID = parsedSteamID.ToString(),
                    Duration = duration,
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