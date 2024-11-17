using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class KickAllCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("kickall", "Kicks all players on the server.")]
        public async Task OnExecute(InteractionContext command, [Option("Reason", "Kick reason.")] string kickReason = "")
        {
            await command.DeferAsync();
            Interface.MessageWrapper message = new Interface.MessageWrapper
            {
                KickallCommand = new Interface.KickallCommand
                {
                    ChannelID = command.Channel.Id,
                    Reason = kickReason,
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