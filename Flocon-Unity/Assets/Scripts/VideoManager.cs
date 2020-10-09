using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    FMOD.Studio.Bus masterBus;

    // Start is called before the first frame update
    void Start()
    {
        // Reset all values
        Destroy(GameObject.FindGameObjectWithTag("GameManager"));

        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(CheckEndVideo());

        masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Saisons", 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator CheckEndVideo()
    {
        videoPlayer.Prepare();

        while(!videoPlayer.isPrepared)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Video ready");

        videoPlayer.Play();

        while (videoPlayer.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Animatic finished");
        GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>().RestartAnimator();

        SceneManager.LoadScene(0);
    }
}
