﻿using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            await _presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOnline" , Context.User.GetUserName());

            await GetOnlineUsers();
            

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            await GetOnlineUsers();

            await base.OnDisconnectedAsync(exception);
        }

        private async Task GetOnlineUsers()
        {
            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }
    }
}
