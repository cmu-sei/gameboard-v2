// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Gameboard.Hubs
{
    /// <summary>
    /// SignalR hub for Gameboard member presence
    /// </summary>
    [Authorize(Policy = "OneTimeTicket")]
    public class GameboardHub: Hub<IGameboardEvent>
    {
        public GameboardHub (ILogger<GameboardHub> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<GameboardHub> _logger;

        /// <summary>
        /// On Disconnected, announce departure to others.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string sessionId = Context.Items["SessionId"].ToString();

            _logger.LogDebug($"disconnected: {sessionId} {Context.ConnectionId} {Context.UserIdentifier}");

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                Clients.OthersInGroup(sessionId).PresenceUpdated(
                    new MemberPresence
                    {
                        Id = Context.UserIdentifier,
                        TeamId = sessionId,
                        EventType = PresenceEvent.Departure
                    }
                );
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task Listen(MemberPresence presence)
        {
            _logger.LogDebug($"listen {presence.TeamId} {presence.Id} {Context.ConnectionId}");

            if (Context.Items.ContainsKey("SessionId"))
                Context.Items["SessionId"] = presence.TeamId;
            else
                Context.Items.Add("SessionId", presence.TeamId);

            Groups.AddToGroupAsync(Context.ConnectionId, presence.TeamId);
            presence.EventType = PresenceEvent.Arrival;
            return Clients.OthersInGroup(presence.TeamId).PresenceUpdated(presence);
        }

        public Task Leave(MemberPresence presence)
        {
            if (presence == null)
                return null;

            _logger.LogDebug($"leave {presence.TeamId} {presence.Id} {Context.ConnectionId}");
            Groups.RemoveFromGroupAsync(Context.ConnectionId, presence.TeamId);
            presence.EventType = PresenceEvent.Departure;
            return Clients.OthersInGroup(presence.TeamId).PresenceUpdated(presence);
        }

        public Task Greet(MemberPresence presence)
        {
            presence.EventType = PresenceEvent.Greeting;
            return Clients.OthersInGroup(presence.TeamId).PresenceUpdated(presence);
        }
    }

    public interface IGameboardEvent
    {
        Task ProblemUpdated(ProblemDetail problem);
        Task PresenceUpdated(MemberPresence presence);
        Task TeamUpdated(TeamDetail team);
        Task BoardReset(TeamDetail team);
        Task LeaderboardUpdated(Leaderboard updated);
        Task GameUpdated(GameDetail game);
        Task SystemMessage(string message);
    }

    public class MemberPresence
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TeamId { get; set; }
        public string Picture { get; set; }
        public string Picture_o { get; set; }
        public string Picture_ou { get; set; }
        public PresenceEvent EventType { get; set; }
    }

    public enum PresenceEvent
    {
        Arrival,
        Departure,
        Greeting,
        Kicked
    }
}

