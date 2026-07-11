using System.Text.RegularExpressions;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace fr34kyn01535.Uconomy.Utils
{
    /// <summary>
    /// Provides utility methods for handling command responses
    /// across both players and the console.
    /// </summary>
    public static class CommandUtils
    {
        /// <summary>
        /// Sends a reply message to the specified player.
        /// If the caller is an in-game player, the message is sent via chat.
        /// If the caller is the console, the message is logged instead.
        /// </summary>
        /// <param name="player">The player or console to send the message to.</param>
        /// <param name="message">The message content to display.</param>
        public static void SendCommandReply(IRocketPlayer player, string message)
        {
            if (player is UnturnedPlayer uPlayer)
            {
                ChatManager.serverSendMessage(message, 
                    Color.green, 
                    null, 
                    uPlayer.SteamPlayer(), 
                    EChatMode.GLOBAL, 
                    null, 
                    true);
                return;
            }

            string strippedText = Regex.Replace(message, @"<[^>]*>|\[.*?\]", string.Empty);
            Rocket.Core.Logging.Logger.Log(strippedText);
        }
    }
}