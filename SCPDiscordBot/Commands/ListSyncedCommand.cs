using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class ListSyncedCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("listsynced", "Lists players synced to Discord.")]
        public async Task OnExecute(InteractionContext command, [Option("IncludeOffline", "List all synced players, even offline.")] bool includeOffline = false)
        {
            await command.DeferAsync();
            Interface.MessageWrapper message = new Interface.MessageWrapper
            {
                ListSyncedCommand = new Interface.ListSyncedCommand
                {
                    ChannelID = command.Channel.Id,
                    DiscordUserID = command.User.Id,
                    ListAll = includeOffline,
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