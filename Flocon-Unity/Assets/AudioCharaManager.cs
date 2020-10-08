using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCharaManager : MonoBehaviour
{
    public void Breath()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Respiration_Character");
    }

    public void Footstep()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Footsteps_Character");
    }
}
