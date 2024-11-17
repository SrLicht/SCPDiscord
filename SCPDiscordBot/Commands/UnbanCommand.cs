using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class UnbanCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("unban", "Unbans a player from the server")]
        public async Task OnExecute(InteractionContext command, [Option("SteamIDorIP", "Steam ID or IP of the user to unban.")] string steamIDOrIP)
        {
            await command.DeferAsync();
            Interface.MessageWrapper message = new Interface.MessageWrapper
            {
                UnbanCommand = new Interface.UnbanCommand
                {
                    ChannelID = command.Channel.Id,
                    SteamIDOrIP = steamIDOrIP,
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