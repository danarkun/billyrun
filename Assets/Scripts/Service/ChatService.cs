using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
using System.Text;

public class ChatService
{
    private WebSocket _websocket;
    private string _connectionUrl;

    public event Action<string, string> OnMessageReceived;
    private readonly ConcurrentQueue<string> _mainThreadQueue = new ConcurrentQueue<string>();

    [Serializable]
    private class SignalRMessage
    {
        public int type;
        public string target;
        public string[] arguments;
    }

    public ChatService(string url)
    {
        // SignalR requires WebSockets to start with ws:// or wss://
        if (url.StartsWith("http://"))
            url = url.Replace("http://", "ws://");
        else if (url.StartsWith("https://"))
            url = url.Replace("https://", "wss://");

        _connectionUrl = url;

        _websocket = new WebSocket(_connectionUrl);

        _websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket Connected!");
            
            // 1. Send SignalR Handshake (Record Separator \x1E)
            string handshake = "{\"protocol\":\"json\",\"version\":1}\x1E";
            _websocket.SendText(handshake);
        };

        _websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket Error: " + e);
        };

        _websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket Closed!");
        };

        _websocket.OnMessage += (bytes) =>
        {
            // 2. Parse incoming SignalR messages which are \x1E delimited
            string messageStr = Encoding.UTF8.GetString(bytes);
            string[] messages = messageStr.Split((char)0x1E);

            foreach (var msg in messages)
            {
                if (string.IsNullOrEmpty(msg)) continue;
                if (msg == "{}") continue; // Empty handshake response

                try
                {
                    SignalRMessage parsedMsg = JsonUtility.FromJson<SignalRMessage>(msg);
                    
                    // SignalR Invocation type is 1
                    if (parsedMsg != null && parsedMsg.type == 1 && parsedMsg.target == "ReceiveMessage")
                    {
                        if (parsedMsg.arguments != null && parsedMsg.arguments.Length >= 2)
                        {
                            string user = parsedMsg.arguments[0];
                            string text = parsedMsg.arguments[1];
                            _mainThreadQueue.Enqueue($"{user}: {text}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Could not parse JSON: {msg} | Error: {e.Message}");
                }
            }
        };
    }
    
    // This method will be called by your ChatUI's Update()
    public void CheckForMessages(System.Action<string> onMessageReceived)
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket?.DispatchMessageQueue();
#endif
        
        while (_mainThreadQueue.TryDequeue(out var message))
        {
            Debug.Log("Deque message: " + message);
            onMessageReceived?.Invoke(message);
        }
    }

    public async Task Connect()
    {
        Debug.Log("Attempting to connect to: " + _connectionUrl);
        await _websocket.Connect();
    }

    public async Task SendChatMessage(string user, string message)
    {
        Debug.Log(">>> 1. Send method started.");
        
        if (_websocket.State == NativeWebSocket.WebSocketState.Open)
        {
            // 3. Format Target Invocation request (Type 1)
            string payload = $"{{\"type\":1,\"target\":\"SendMessage\",\"arguments\":[\"{user}\",\"{message}\"]}}\x1E";
            await _websocket.SendText(payload);
            Debug.Log(">>> 2. Invoke completed successfully!");
        }
        else
        {
            Debug.LogError("Message not sent. WebSocket not connected.");
        }
    }
}