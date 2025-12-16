using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button nextButton;

    private string[] sentences;
    private int index;
    private bool isTalking;

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.E))
        {
            NextSentence();
        }
    }

    public void StartDialogue(string[] newSentences)
    {
        sentences = newSentences;
        index = 0;
        isTalking = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = sentences[index];

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void NextSentence()
    {
        index++;

        if (index >= sentences.Length)
            EndDialogue();
        else
            dialogueText.text = sentences[index];
    }


    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isTalking = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsTalking()
    {
        return isTalking;
    }
}
