using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    
    private IRestService _restService;

    async void Start()
    {
        var config = await ConfigLoader.GetConfig();
        if (config != null)
        {
            _restService = new RestService(config.ApiBaseUrl);
        }
    }

    public async void SaveGame()
    {
        if (_restService == null)
        {
            Debug.LogError("RestService not initialized. Config might still be loading.");
            return;
        }
        bool success = await _restService.SavePlayerProfile("Unity_Tester_01", 100);
        
        if (success)
            Debug.Log("Saved Successfully to In-Memory DB!");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public class BypassCertificate : CertificateHandler {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }

}
