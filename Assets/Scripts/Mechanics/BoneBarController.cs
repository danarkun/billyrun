using UnityEngine;
using UnityEngine.UI;
using TMPro; // Make sure you have TextMeshPro installed!
using System.Collections;

public class BoneBarController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Image with Image Type set to 'Filled'")]
    public Image fillImage; 
    
    [Tooltip("The Text component that shows '0 / 100'")]
    public TextMeshProUGUI tokenText;

    [Header("Animation Settings")]
    [Range(0.1f, 2f)]
    public float fillSpeed = 0.5f; // How long it takes to finish the animation

    private Coroutine _updateCoroutine;
    private float _displayedTokenCount = 0f;

    /// <summary>
    /// Call this from your Player script whenever tokens change.
    /// </summary>
    public void UpdateTokenBar(int currentTokens, int maxTokens)
    {
        float targetFill = (float)currentTokens / maxTokens;

        // If a fill is already happening, stop it so we can start the new one
        if (_updateCoroutine != null) 
            StopCoroutine(_updateCoroutine);

        _updateCoroutine = StartCoroutine(AnimateBar(targetFill, currentTokens, maxTokens));
    }

    private IEnumerator AnimateBar(float targetFill, int targetTokens, int maxTokens)
    {
        float startFill = fillImage.fillAmount;
        float startTokens = _displayedTokenCount;
        float elapsed = 0f;

        while (elapsed < fillSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fillSpeed;

            // 1. Smoothly animate the Bar Fill
            // SmoothStep makes it start fast and slow down at the end
            fillImage.fillAmount = Mathf.SmoothStep(startFill, targetFill, t);

            // 2. Smoothly animate the Numbers
            _displayedTokenCount = Mathf.Lerp(startTokens, targetTokens, t);
            
            // Round the float to an int so the text doesn't show decimals
            tokenText.text = $"{(int)_displayedTokenCount} / {maxTokens}";

            yield return null;
        }

        // Ensure we land exactly on the target values
        fillImage.fillAmount = targetFill;
        _displayedTokenCount = targetTokens;
        tokenText.text = $"{targetTokens} / {maxTokens}";
    }
}