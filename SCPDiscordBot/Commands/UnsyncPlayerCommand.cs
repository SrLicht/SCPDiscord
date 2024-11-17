using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class UnsyncPlayerCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("unsyncplayer", "Unsyncs a player's Discord account from the SCP:SL server.")]
        public async Task OnExecute(InteractionContext command, [Option("Player", "Player to unsync.")] DiscordUser user)
        {
            await command.DeferAsync();
            Interface.MessageWrapper message = new Interface.MessageWrapper
            {
                UnsyncRoleCommand = new Interface.UnsyncRoleCommand
                {
                    ChannelID = command.Channel.Id,
                    DiscordUserID = user.Id,
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