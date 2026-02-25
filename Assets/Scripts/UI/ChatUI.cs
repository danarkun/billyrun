using UnityEngine;
using TMPro; // Use TextMeshPro for better quality
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class ChatUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private Transform messageContainer; // The "Content" object
    [SerializeField] private GameObject messagePrefab;
    public TextMeshProUGUI chatDisplayText;

    private ChatService _chatService;
    private string _username = "Player1";

    async void Start()
    {
        Debug.Log(">>> ChatUI Start is running!");
        // This will print to your browser console every 5 seconds
        InvokeRepeating(nameof(Heartbeat), 1f, 5f);
        _chatService = new ChatService("http://localhost:5000/chatHub");
        // _chatService = new ChatService("https://localhost:5001/chatHub");
        _chatService.OnMessageReceived += HandleNewMessage;

        sendButton.onClick.AddListener(OnSendClicked);
        
        await _chatService.Connect();
    }

    void Update()
    {
        // 2. Check for new messages from the SignalR thread
        if (_chatService != null)
        {
            _chatService.CheckForMessages((fullMessage) => 
            {
                // This now safely runs on the Main Thread!
                chatDisplayText.text += "\n" + fullMessage;
                Debug.Log("UI Updated with: " + fullMessage);
            });
        }
    }
// The InputField passes the current text automatically to this event
    public void OnInputSubmit(string outputText)
    {
        Debug.Log("Enter pressed! Text: " + outputText);
        OnSendClicked();
    }

    private async void OnSendClicked()
    {
        string messageToSend = inputField.text;

        if (!string.IsNullOrEmpty(messageToSend))
        {
            // 2. Disable the button so the player can't spam the 'Send' button 
            // while the current message is still uploading.
            sendButton.interactable = false;

            try 
            {
                // 3. Use 'await' to pause here until the server acknowledges the message
                await _chatService.SendChatMessage(_username, messageToSend);
                
                // 4. Only clear the text if the send was successful
                inputField.text = ""; 
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to send message: {ex.Message}");
                // Optional: Show a "Send Failed" popup to the player here
            }
            finally 
            {
                // 5. Re-enable the button
                sendButton.interactable = true;
                inputField.ActivateInputField(); // Refocus the text box for the next message
            }
        }
    }

    private async void HandleNewMessage(string user, string message)
    {
        // This magic line hops from the SignalR thread back to the Unity Main Thread
        await Task.Yield(); 

        // Now you can safely use Unity's API
        GameObject newMsg = Instantiate(messagePrefab, messageContainer);
        newMsg.GetComponent<TMP_Text>().text = $"<b>{user}:</b> {message}";
        
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        // We wait until the end of the frame so the Layout Group has 
        // time to calculate the height of the new message.
        StartCoroutine(ScrollCoroutine());
    }

    private System.Collections.IEnumerator ScrollCoroutine()
    {
        yield return new WaitForEndOfFrame();
        var scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 0f; // 0 is the bottom
    }

    void Heartbeat() 
    {
        Debug.Log(">>> WebGL Engine Heartbeat: " + Time.time);
    }
}