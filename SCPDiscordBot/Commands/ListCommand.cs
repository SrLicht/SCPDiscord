using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class ListCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("list", "Lists online players.")]
        public async Task OnExecute(InteractionContext command)
        {
            await command.DeferAsync();
            Interface.MessageWrapper message = new Interface.MessageWrapper
            {
                ListCommand = new Interface.ListCommand
                {
                    ChannelID = command.Channel.Id,
                    DiscordUserID = command.User.Id,
                    InteractionID = command.InteractionId,
                    DiscordDisplayName = command.Member.DisplayName,
                    DiscordUsername = command.Member.Username
                }
            };
            MessageScheduler.CacheInteraction(command);
            await NetworkSystem.SendMessage(message, command);
        }
    }
}