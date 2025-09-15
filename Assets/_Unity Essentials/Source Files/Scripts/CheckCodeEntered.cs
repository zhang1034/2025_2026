using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CheckCodeEntered : MonoBehaviour
{
    public InputField codeInputField; // Reference to the Input Field
    public ParticleSystem correctEffect; // Reference to the GameObject to activate
    public AudioSource correctSound; // Reference to the Audio Source
    public AudioSource incorrectSound; // Reference to the Audio Source


    void Start()
    {
        // Add listener for when the value of the text in the input field changes
        codeInputField.onValueChanged.AddListener(OnInputFieldChanged);
    }

    private void OnInputFieldChanged(string text)
    {
        // Check if the input field has exactly 4 characters
        if (text.Length == 4)
        {
            ValidateCode();
            // Start the coroutine to clear the input field after a delay
            StartCoroutine(ClearInputFieldAfterDelay(0.5f));
        }
        else if (text.Length > 4)
        {
            // If more than 4 characters are entered, truncate the text
            codeInputField.text = text.Substring(0, 4);
        }
    }

    private IEnumerator ClearInputFieldAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Clear the input field and reset it for new input
        codeInputField.text = "";
        codeInputField.ActivateInputField();
    }

    public void ValidateCode()
    {
        if (codeInputField != null)
        {
            string code = codeInputField.text;
            if (code == "2004")
            {
                // Activate the GameObject
                correctEffect.Play();
                // Play correct sound
                correctSound.Play();
            }
            else
            {
                // Play incorrect sound
                incorrectSound.Play();

                // Start the shake animation
                StartCoroutine(ShakeInputField(0.5f, 0.1f));
            }
        }
    }

        // Coroutine for shaking the input field
    private IEnumerator ShakeInputField(float duration, float magnitude)
    {
        Vector3 originalPosition = codeInputField.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-50f, 50f) * magnitude;
            codeInputField.transform.localPosition = new Vector3(x, originalPosition.y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        codeInputField.transform.localPosition = originalPosition;
    }
}
