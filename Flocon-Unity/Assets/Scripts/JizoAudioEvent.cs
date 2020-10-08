using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JizoAudioEvent : MonoBehaviour
{
    public void LeafSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Jizo_Mouvementfeuille");
    }
}
