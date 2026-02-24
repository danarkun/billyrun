using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

public class ChatService
{
    private HubConnection _connection;

    public event Action<string, string> OnMessageReceived;
    private readonly ConcurrentQueue<string> _mainThreadQueue = new ConcurrentQueue<string>();
    public ChatService(string url)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(url)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            _mainThreadQueue.Enqueue($"{user}: {message}");
        });
    }
    
    // This method will be called by your ChatUI's Update()
    public void CheckForMessages(System.Action<string> onMessageReceived)
    {
        while (_mainThreadQueue.TryDequeue(out var message))
        {
            onMessageReceived?.Invoke(message);
        }
    }

    public async Task Connect()
    {
        try
        {
            await _connection.StartAsync();
            Debug.Log("SignalR Connected!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SignalR Connection Error: {ex.Message}");
        }
    }

    public async Task SendChatMessage(string user, string message)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("SendMessage", user, message);
        }
    }
}