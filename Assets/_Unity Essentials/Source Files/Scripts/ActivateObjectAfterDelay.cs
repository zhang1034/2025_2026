using UnityEngine;
using System.Collections;

public class ActivateObjectAfterDelay : MonoBehaviour
{
    // Time delay in seconds
    public float delay = 5.0f;

    // Use this for initialization
    void Start()
    {
        // Start the coroutine
        StartCoroutine(ActivationRoutine());
    }

    // Coroutine to activate child objects after a delay
    private IEnumerator ActivationRoutine()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Activate all child objects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
