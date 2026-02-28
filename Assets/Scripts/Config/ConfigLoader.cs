using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class ConfigLoader
{
    private static AppConfig _config;

    public static async Task<AppConfig> GetConfig()
    {
        if (_config != null) return _config;

        string path = Application.streamingAssetsPath + "/config.json";
        
        // For WebGL, streamingAssetsPath is a URL
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load config from {path}: {request.error}");
                // Fallback to default or throw
                return null;
            }

            _config = JsonUtility.FromJson<AppConfig>(request.downloadHandler.text);
            Debug.Log("Config loaded successfully!");
            return _config;
        }
    }
}
