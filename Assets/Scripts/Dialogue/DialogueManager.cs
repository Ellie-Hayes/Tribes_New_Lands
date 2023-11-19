using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    //public Animator anim;
    public TextMeshProUGUI DialogueText;
    public GameObject promptPlayerText;
    public float textSpeed;
    private float promptPlayerCountdown;
    private bool promptPlayer;

    private Queue<string> sentances;
    PressKeyToContinue pressKey;
    private Menus menus;
    // Start is called before the first frame update
    void Start()
    {
        sentances = new Queue<string>();
        pressKey = promptPlayerText.GetComponent<PressKeyToContinue>();
        menus = GameObject.FindGameObjectWithTag("FadeOverlay").GetComponent<Menus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
            {
                return;
            }
            else
            {
                DisplayNextSentance();
            }
            
        }

        if (promptPlayer)
        {
            promptPlayerCountdown -= Time.deltaTime;

            if (promptPlayerCountdown <= 0)
            {
                pressKey.PromptPlayer = true;
                
            }
        }

    }
    public void StartDialogue(Dialogue dialogue)
    {

        //anim.SetBool("IsOpen", true);
        sentances.Clear();

        foreach (string sentance in dialogue.sentances)
        {
            sentances.Enqueue(sentance);
        }
        DisplayNextSentance();
    }
    public void DisplayNextSentance()
    {
        promptPlayer = false;
        promptPlayerCountdown = 10f;
        pressKey.PromptPlayer = false;

        if (sentances.Count == 0)
        {
            EndDialogue();
            return;

        }
        string sentance = sentances.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentance));
    }
    IEnumerator TypeSentence(string sentance)
    {
        DialogueText.text = "";
        foreach (char letter in sentance.ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
            yield return null;
        }

        promptPlayer = true;
    }
    void EndDialogue()
    {
        menus.Fadeout = true;
    }
}
