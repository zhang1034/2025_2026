using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogueLines;

    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void Interact()
    {
        dialogueManager.StartDialogue(dialogueLines);
    }
}
