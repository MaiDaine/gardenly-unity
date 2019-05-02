// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEngine;

// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine
{
    /// <inheritdoc />
    /// <summary>
    /// Game Event global message
    /// </summary>
    public class GameEventMessage : Message
    {
        #region Constants

        private const string NO_GAME_EVENT = "None";

        #endregion

        #region Properties

        /// <summary>
        /// Returns TRUE if this message contains a game event string.
        /// If FALSE, then this message was used only to send a GameObject reference.
        /// </summary>
        public bool HasGameEvent { get { return !EventName.Equals(NO_GAME_EVENT); } }

        /// <summary>
        /// Returns TRUE if this message contains a game event string that is also a system event.
        /// A game event string is considered to be a system event if it is contained in the SystemGameEvent enum values.
        /// </summary>
        public bool IsSystemEvent { get; private set; }

        #endregion

        #region Public Variables

        /// <summary>
        /// The game event string sent with this message.
        /// If "None", then no game event is considered to have been passed with this message.
        /// </summary>
        public readonly string EventName;

        /// <summary>
        /// The source GameObject reference that sent this message.
        /// If null, then no GameObject reference was passed with this message.
        /// </summary>
        public GameObject Source;

        #endregion

        #region Constuctors

        /// <summary> Initializes a new instance of the class with the passed SystemGameEvent </summary>
        /// <param name="systemGameEvent"> The game event string that will get sent with this message </param>
        public GameEventMessage(SystemGameEvent systemGameEvent)
        {
            EventName = systemGameEvent.ToString();
            Source = null;
            IsSystemEvent = true;
        }

        /// <summary> Initializes a new instance of the class with the passed game event string value </summary>
        /// <param name="gameEvent"> The game event string that will get sent with this message </param>
        public GameEventMessage(string gameEvent)
        {
            EventName = gameEvent;
            Source = null;
            IsSystemEvent = false;
        }

        /// <summary>
        /// Initializes a new instance of the class with a GameObject reference and no game event string.
        /// <para/> This overload can be used to send only a GameObject reference.
        /// </summary>
        /// <param name="source"> The game object reference that will get sent with this message </param>
        public GameEventMessage(GameObject source)
        {
            EventName = NO_GAME_EVENT;
            Source = source;
            IsSystemEvent = false;
        }

        /// <summary> Initializes a new instance of the class with a SystemGameEvent and a GameObject reference </summary>
        /// <param name="systemGameEvent"> The game event string that will get sent with this message </param>
        /// <param name="source"> The game object reference that will get sent with this message </param>
        public GameEventMessage(SystemGameEvent systemGameEvent, GameObject source)
        {
            EventName = systemGameEvent.ToString();
            Source = source;
            IsSystemEvent = true;
        }

        /// <summary> Initializes a new instance of the class with a game event string and a GameObject reference </summary>
        /// <param name="gameEvent"> The game event string that will get sent with this message </param>
        /// <param name="source"> The game object reference that will get sent with this message </param>
        public GameEventMessage(string gameEvent, GameObject source)
        {
            EventName = gameEvent;
            Source = source;
            IsSystemEvent = false;
        }

        #endregion

        #region Static Methods

        /// <summary> Sends a message with the passed SystemGameEvent </summary>
        /// <param name="systemGameEvent"> The game event string sent with this message </param>
        public static void SendEvent(SystemGameEvent systemGameEvent) { SendEvent(new GameEventMessage(systemGameEvent)); }

        /// <summary> Sends a message with the passed game event string </summary>
        /// <param name="gameEvent"> The game event string sent with this message </param>
        public static void SendEvent(string gameEvent) { SendEvent(new GameEventMessage(gameEvent)); }

        /// <summary> Sends a message with the passed GameObject reference </summary>
        /// <param name="source"> The source game object reference that sent this message </param>
        public static void SendEvent(GameObject source) { SendEvent(new GameEventMessage(source)); }

        /// <summary> Sends a message with a passed SystemGameEvent and a GameObject reference </summary>
        /// <param name="systemGameEvent"> The game event string sent with this message </param>
        /// <param name="source"> The source game object reference that sent this message </param>
        public static void SendEvent(SystemGameEvent systemGameEvent, GameObject source) { SendEvent(new GameEventMessage(systemGameEvent, source)); }

        /// <summary> Sends a message with a given game event string and a GameObject reference </summary>
        /// <param name="gameEvent"> The game event string sent with this message </param>
        /// <param name="source"> The source game object reference that sent this message </param>
        public static void SendEvent(string gameEvent, GameObject source) { SendEvent(new GameEventMessage(gameEvent, source)); }

        /// <summary> Sends a list of messages, each message with the given event string and GameObject reference </summary>
        /// <param name="gameEvents"> The game event strings sent with these messages </param>
        /// <param name="source"> The source game object reference that sent these messages </param>
        public static void SendEvents(List<string> gameEvents, GameObject source = null)
        {
            if (gameEvents == null || gameEvents.Count == 0) return;
            foreach (string gameEvent in gameEvents)
                SendEvent(gameEvent, source);
        }

        private static void SendEvent(GameEventMessage gameEventMessage)
        {
            GameEventManager.ProcessGameEvent(gameEventMessage);
            Send(gameEventMessage);
        }

        #endregion
    }
}