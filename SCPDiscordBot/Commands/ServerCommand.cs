using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class ServerCommand : ApplicationCommandModule
    {
        [SlashRequireGuild]
        [SlashCommand("server", "Runs a server console command.")]
        public async Task OnExecute(InteractionContext command, [Option("Command", "Server console command to run.")] string serverCommand)
        {
            if (!ConfigParser.HasPermission(command.Member, serverCommand))
            {
                DiscordEmbed error = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = "You do not have permission to use that command."
                };
                await command.CreateResponseAsync(error);
                return;
            }

            await command.DeferAsync();
            Interface.MessageWrapper message = new Interface.MessageWrapper
            {
                ConsoleCommand = new Interface.ConsoleCommand
                {
                    ChannelID = command.Channel.Id,
                    DiscordUserID = command.Member?.Id ?? 0,
                    Command = serverCommand,
                    InteractionID = command.InteractionId,
                    DiscordDisplayName = command.Member?.DisplayName,
                    DiscordUsername = command.Member?.Username
                }
            };
            MessageScheduler.CacheInteraction(command);
            await NetworkSystem.SendMessage(message, command);
        }
    }
}