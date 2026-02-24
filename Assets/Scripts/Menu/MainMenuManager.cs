using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    
    private IRestService _restService;
    private string backendUrl = "https://localhost:5001/api";

    void Awake()
    {
        _restService = new RestService(backendUrl);
    }

    public async void SaveGame()
    {
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
