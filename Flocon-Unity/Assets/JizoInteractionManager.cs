using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JizoInteractionManager : MonoBehaviour
{
    private TextMeshPro textMesh;

    public GameObject bulle;

    private string msg = "CECI EST UN MESSAGE DE LA PLUS HAUTE IMPORTANCE";

    [Space]
    [Header("Message Animation Options")]

    [SerializeField]
    [Tooltip("The delay between two letters")]
    [Range(1, 5)]
    private float messageSpeed = 1;

    [SerializeField]
    [Tooltip("How long it takes for a message to start appearing")]
    [Range(0f, 5)]
    private float preMessageDuration = 0.5f;

    [SerializeField]
    [Tooltip("How long a message remains in seconds once it's been displayed")]
    [Range(0f, 5)]
    private float postMessageDuration = 1.25f;

    [SerializeField]
    [Tooltip("How long a pause last (dot, comma)")]
    [Range(0f, 2f)]
    private float shortPauseDuration = 0.25f;

    [SerializeField]
    [Tooltip("How long a pause last (dot, comma)")]
    [Range(0f, 2f)]
    private float longPauseDuration = 0.35f;

    public delegate void MessageShownHandler();
    public event MessageShownHandler MessageShownEvent;

    void Start()
    {
        textMesh = GetComponentInChildren<TextMeshPro>(true);
        Debug.Assert(textMesh != null, "No TextMeshPro found");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool IsShortPause(char c)
    {
        return c == ',' || c == ':';
    }

    public static bool IsLongPause(char c)
    {
        return c == '.' || c == '!';
    }

    IEnumerator RevealLetterByLetter(string text)
    {
        //inputs.Player.SkipDialog.performed += SkipDialog;

        textMesh.text = text;
        textMesh.maxVisibleCharacters = 0;

        float delay;
        textMesh.ForceMeshUpdate(); // update textInfo to get actual characterCount

        yield return new WaitForSeconds(preMessageDuration);

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            textMesh.maxVisibleCharacters = i + 1;

            if (IsShortPause(textMesh.text[i]))
            {
                delay = shortPauseDuration;
            }
            else if (IsLongPause(textMesh.text[i]))
            {
                delay = longPauseDuration;
            }
            else
            {
                // No sound on space characters
                if (textMesh.text[i] != ' ')
                {
                    /* Guerric : Yo, pose ton trigger là */

                    /*
                    if (textName.text == "Zeous")
                        AkSoundEngine.PostEvent("Play_Ecriture_Animation_Ange", gameObject);
                    else
                        AkSoundEngine.PostEvent("Play_Ecriture_Animation", gameObject);
                    */
                }

                delay = 0.1f / (2 * messageSpeed);
            }

            yield return new WaitForSeconds(delay);
        }

        // Erase message before next event, maybe in some case it could stay
        //textMesh.text = "";

        //MessageShownEvent();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            Debug.Log("Jizo Triggered"); 
            //bulle.SetActive(true);
            bulle.GetComponent<Animation>().Play("PopOut");
            StartCoroutine(RevealLetterByLetter(msg));
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            textMesh.text = "";
            bulle.GetComponent<Animation>().Play("Dissapear");
            //bulle.SetActive(false);
        }
    }
}
