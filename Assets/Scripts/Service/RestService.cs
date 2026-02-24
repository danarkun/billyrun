using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static MainMenuManager;

public class RestService : IRestService
{
    private readonly string _baseUrl;

    public RestService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async Task<bool> SavePlayerProfile(string username, int level)
    {
        var command = new SaveProfileCommand { Username = username, Level = level };
        string json = JsonUtility.ToJson(command);
        
        return await PostRequest("/Players", json);
    }

    private async Task<bool> PostRequest(string endpoint, string jsonData)
    {
        using var request = new UnityWebRequest(_baseUrl + endpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        // Essential for local development with self-signed certs
        request.certificateHandler = new BypassCertificate();

        var operation = request.SendWebRequest();

        // Wait for request to finish without blocking the main thread
        while (!operation.isDone)
        {
            await Task.Yield(); 
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"REST Error: {request.error} | {request.downloadHandler.text}");
            return false;
        }

        return true;
    }
}