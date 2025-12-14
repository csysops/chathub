Database.Initialize();

var builder = WebApplication.CreateBuilder(args);

// IMPORTANT: Allow external devices to connect via LAN
builder.WebHost.UseUrls("http://0.0.0.0:5291");

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chat");

app.Run();
