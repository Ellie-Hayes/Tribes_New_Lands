using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private bool StartedDialogue;
   
    private void Update()
    {
        if (!StartedDialogue)
        {
            StartedDialogue = true;
            TriggerDialogue();
        }
    }
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Debug.Log("triggering");
    }
}
