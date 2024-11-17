using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCPDiscord.Commands
{
    public class GetUserIdCommand : ApplicationCommandModule
    {
        private readonly HttpClient _httpClient = new HttpClient(); // Cliente HTTP para hacer solicitudes web

        [SlashRequireGuild]
        [SlashCommand("getid", "Obtienes la UserID de un perfil de Steam a partir de la URL")]
        public async Task OnExecute(InteractionContext command, [Option("url", "La URL del perfil de Steam")] string url)
        {
            // Validar la URL
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                await command.CreateResponseAsync("La URL proporcionada no es válida.");
                return;
            }

            try
            {
                // Hacer una solicitud al perfil de Steam en formato XML
                var response = await _httpClient.GetStringAsync($"{url}?xml=1");

                // Expresión regular para extraer el SteamID64
                string pattern = @"<steamID64>(\d+)</steamID64>";
                var match = Regex.Match(response, pattern);

                if (match.Success)
                {
                    string steamId64 = match.Groups[1].Value;

                    // Crear un embed para mostrar el resultado
                    DiscordEmbed embed = new DiscordEmbedBuilder()
                        .WithTitle("Steam UserID")
                        .WithDescription($"La UserID64 de este perfil de Steam es: {steamId64}")
                        .WithColor(DiscordColor.Cyan)
                        .Build();

                    await command.CreateResponseAsync(embed);
                }
                else
                {
                    await command.CreateResponseAsync("No se pudo encontrar el UserID64 en la URL proporcionada.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error
                await command.CreateResponseAsync($"Ocurrió un error al procesar la solicitud: {ex.Message}");
            }
        }
    }
}
