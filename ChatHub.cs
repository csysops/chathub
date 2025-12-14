using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> OnlineUsers
        = new ConcurrentDictionary<string, string>();

    private readonly UserRepository _users = new UserRepository();
    private readonly MessageRepository _messages = new MessageRepository();

    // -------------------------------------------------------------
    // REGISTER
    // -------------------------------------------------------------
    public Task<bool> RegisterUser(string username, string password)
    {
        bool created = _users.CreateUser(username, password);
        return Task.FromResult(created);
    }

    // -------------------------------------------------------------
    // LOGIN
    // -------------------------------------------------------------
    public Task<bool> LoginUser(string username, string password)
    {
        if (_users.ValidateUser(username, password))
        {
            OnlineUsers[username] = Context.ConnectionId;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    // -------------------------------------------------------------
    // GET ALL USERS (for WinForms user list)
    // -------------------------------------------------------------
    public Task<List<string>> GetAllUsers()
    {
        var result = _users.GetAllUsers();
        return Task.FromResult(result);
    }

    // -------------------------------------------------------------
    // SEND PRIVATE MESSAGE (1-1 messaging)
    // -------------------------------------------------------------
    public async Task SendPrivate(string fromUser, string toUser, string message)
    {
        // Save to database
        _messages.SaveMessage(fromUser, toUser, message);

        // Deliver to receiver if online
        if (OnlineUsers.TryGetValue(toUser, out string connectionId))
        {
            await Clients.Client(connectionId)
                .SendAsync("ReceivePrivate", fromUser, message);
        }

        // Also send it back to sender UI for confirmation
        if (OnlineUsers.TryGetValue(fromUser, out string senderConnection))
        {
            await Clients.Client(senderConnection)
                .SendAsync("ReceivePrivate", fromUser, message);
        }
    }

    // -------------------------------------------------------------
    // LOAD HISTORY BETWEEN TWO USERS
    // -------------------------------------------------------------
    public Task<List<ChatMessage>> LoadHistory(string userA, string userB)
    {
        var history = _messages.LoadHistory(userA, userB);
        return Task.FromResult(history);
    }

    // -------------------------------------------------------------
    // CLEAN DISCONNECT
    // -------------------------------------------------------------
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = OnlineUsers
            .FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

        if (user != null)
            OnlineUsers.TryRemove(user, out _);

        return base.OnDisconnectedAsync(exception);
    }
}
