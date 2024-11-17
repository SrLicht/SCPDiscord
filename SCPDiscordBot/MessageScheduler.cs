using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCPDiscord;

// Separate class to run the thread
public class StartMessageScheduler
{
    public StartMessageScheduler()
    {
        Task _ = MessageScheduler.Init();
    }
}

public static class MessageScheduler
{
    private static ConcurrentDictionary<ulong, ConcurrentQueue<string>> messageQueues = new ConcurrentDictionary<ulong, ConcurrentQueue<string>>();
    private static List<InteractionContext> interactionCache = new List<InteractionContext>();

    public static async Task Init()
    {
        while (true)
        {
            Thread.Sleep(1000);

            // If we haven't connected to discord yet wait until we do
            if (!DiscordAPI.instance?.connected ?? false)
            {
                continue;
            }

            // Clean old interactions from cache
            interactionCache.RemoveAll(x => x.InteractionId.GetSnowflakeTime() < DateTimeOffset.Now - TimeSpan.FromSeconds(30));

            try
            {
                foreach (KeyValuePair<ulong, ConcurrentQueue<string>> channelQueue in messageQueues)
                {
                    StringBuilder finalMessage = new StringBuilder();
                    while (channelQueue.Value.TryPeek(out string nextMessage))
                    {
                        // If message is too long, abort and send the rest next time
                        if (finalMessage.Length + nextMessage.Length >= 2000)
                        {
                            Logger.Warn("Tried to send too much at once (Current: " + finalMessage.Length + " Next: " + nextMessage.Length +
                                        "), waiting one second to send the rest.", LogID.DISCORD);
                            break;
                        }

                        if (channelQueue.Value.TryDequeue(out nextMessage))
                        {
                            finalMessage.Append(nextMessage);
                            finalMessage.Append('\n');
                        }
                    }

                    string finalMessageStr = finalMessage.ToString();
                    if (string.IsNullOrWhiteSpace(finalMessageStr))
                    {
                        continue;
                    }

                    if (finalMessageStr.EndsWith('\n'))
                    {
                        finalMessageStr = finalMessageStr.Remove(finalMessageStr.Length - 1);
                    }

                    await DiscordAPI.SendMessage(channelQueue.Key, finalMessageStr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public static void QueueMessage(ulong channelID, string message)
    {
        ConcurrentQueue<string> channelQueue = messageQueues.GetOrAdd(channelID, new ConcurrentQueue<string>());
        channelQueue.Enqueue(message);
    }

    public static bool TryUncacheInteraction(ulong interactionID, out InteractionContext interaction)
    {
        interaction = interactionCache.FirstOrDefault(x => x.InteractionId == interactionID);
        return interactionCache.Remove(interaction);
    }

    public static void CacheInteraction(InteractionContext interaction)
    {
        interactionCache.Add(interaction);
    }
}