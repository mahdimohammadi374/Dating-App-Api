namespace API.SignalR
{
    public class PresenceTracker
    {
        private readonly Dictionary<string , List<string>> onlineUsers=new Dictionary<string , List<string>>();

        public Task UserConnected(string username , string connectionId)
        {
            lock (onlineUsers)
            {
                if (onlineUsers.ContainsKey(username))
                    onlineUsers[username].Add(connectionId);
                else
                    onlineUsers.Add(username, new List<string>() { connectionId});
                   
            }
            return Task.CompletedTask;
        }


        public Task UserDisconnected(string username, string connectionId)
        {
            lock (onlineUsers)
            {
                if (!onlineUsers.ContainsKey(username)) return Task.CompletedTask;
                onlineUsers[username].Remove(connectionId);
                if (onlineUsers[username].Count == 0)
                    onlineUsers.Remove(username);
            }
            return Task.CompletedTask;
        }
        public Task<string[]> GetOnlineUsers()
        {
            string[] users;
            lock (onlineUsers)
            {
                users = onlineUsers.OrderBy(x=> x.Key).Select(x=>x.Key).ToArray();
                
            }
            return Task.FromResult(users);
        }

        public Task<List<string>> GetUserConnections(string userName)
        {
            List<string> connections = new ();
            lock (onlineUsers)
            {
                 connections=onlineUsers.GetValueOrDefault(userName);
            }
            return Task.FromResult(connections);

        }
    }
}
