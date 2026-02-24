using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

public class ChatService
{
    private HubConnection _connection;

    public event Action<string, string> OnMessageReceived;

    public ChatService(string url)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(url)
            .WithAutomaticReconnect()
            .Build();

        // Listen for messages from the server
        _connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            OnMessageReceived?.Invoke(user, message);
        });
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