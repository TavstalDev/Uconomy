﻿using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using fr34kyn01535.Uconomy;

namespace Tavstal.TLibrary.Utils
{
    /// <summary>
    /// Unturned chat helper
    /// </summary>
    public static class ChatHelper
    {
        //private static string Translate(bool addPrefix, string key, params object[] args) => TAdvancedHealthMain.Instance.Translate(addPrefix, key, args);

        public static void ServerSendChatMessage(string text, string icon = null, SteamPlayer fromPlayer = null, SteamPlayer toPlayer = null, EChatMode mode = EChatMode.GLOBAL)
        => ChatManager.serverSendMessage(text, Color.white, fromPlayer, toPlayer, mode, icon, true);

        /// <summary>
        /// Send plain text chat message to a specific player
        /// </summary>
        /// <param name="toPlayer"></param>
        /// <param name="text"></param>
        public static void SendPlainChatMessage(SteamPlayer toPlayer, string text)
        {
            string icon = "";
            ServerSendChatMessage(text, icon, null, toPlayer, EChatMode.GLOBAL);
        }

        /// <summary>
        /// Send chat message as command reply to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendCommandReply(object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
                ServerSendChatMessage(FormatHelper.FormatTextV2(Uconomy.Instance.Translate(translation, args)), icon, null, steamPlayer, EChatMode.GLOBAL);
            else if (toPlayer is UnturnedPlayer player)
                ServerSendChatMessage(FormatHelper.FormatTextV2(Uconomy.Instance.Translate(translation, args)), icon, null, player.SteamPlayer(), EChatMode.GLOBAL);
            else
                LoggerHelper.LogRichCommand(Uconomy.Instance.Translate(translation, args));
        }

        /// <summary>
        /// Send chat message as command reply to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendPlainCommandReply(object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
                ServerSendChatMessage(FormatHelper.FormatTextV2(string.Format(translation, args)), icon, null, steamPlayer, EChatMode.GLOBAL);
            else if (toPlayer is UnturnedPlayer player)
                ServerSendChatMessage(FormatHelper.FormatTextV2(string.Format(translation, args)), icon, null, player.SteamPlayer(), EChatMode.GLOBAL);
            else
                LoggerHelper.LogRichCommand(string.Format(translation, args));
        }

        /// <summary>
        /// Send chat message to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendChatMessage(SteamPlayer toPlayer, string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(FormatHelper.FormatTextV2(Uconomy.Instance.Translate(translation, args)), icon, null, toPlayer, EChatMode.GLOBAL);
        }


        /// <summary>
        /// Send chat message to all players
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendChatMessage(string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(Uconomy.Instance.Translate(translation, args), icon, null, null, EChatMode.GLOBAL);
        }
    }
}
