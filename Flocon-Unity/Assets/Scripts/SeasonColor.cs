using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonColor : MonoBehaviour
{
    private GameObject hair;

    private GameObject playerModel;
    private Animator playerAnimator;

    [SerializeField]
    [Tooltip("The speed of the rotating character")]
    private float spinSpeed = 2500;

    private bool isRotating = false;

    private bool firstZone = true;

    private Color[] hairColors = { new Color(0x00 / 255f, 0x4C / 255f, 0xEE), new Color(0xFF / 255f, 0x1E / 255f, 0x39 / 255f), new Color(0x70 / 255f, 0xB7 / 255f, 0x00 / 255f), new Color(1, 1, 1) };

    // Start is called before the first frame update
    void Start()
    {
        hair = GameObject.FindGameObjectWithTag("Hair");
        Debug.Assert(hair != null, "Missing hair reference");

        playerModel = transform.GetChild(1).gameObject;
        Debug.Assert(playerModel != null, "No 3D model found for the player");

        playerAnimator = playerModel.GetComponent<Animator>();
        Debug.Assert(playerModel != null, "No Animator found for the player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(firstZone)
        {
            firstZone = false;
            return;
        }

        if(collision.CompareTag("Winter"))
        {
            StartCoroutine(HairTransitionColor(0));
            //hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", hairColors[0]);
            Debug.Log("inside winter");
        }
        else if (collision.CompareTag("Fall"))
        {
            StartCoroutine(HairTransitionColor(1));
            //hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", hairColors[1]);
            Debug.Log("inside fall");
        }
        else if (collision.CompareTag("Summer"))
        {
            StartCoroutine(HairTransitionColor(2));
            //hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", hairColors[2]);
            Debug.Log("inside summer");
        }
        else if (collision.CompareTag("Spring"))
        {
            StartCoroutine(HairTransitionColor(3));
            //hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", hairColors[3]);
            Debug.Log("inside spring");
        }
    }

    private IEnumerator HairTransitionBrightness(int index)
    {
        hair.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_DetailTex", 0);

        float timer = 0;
        float timeToTransition = 1;
        while(timer < timeToTransition)
        {
            hair.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Brightness", 6 + 6000 * (timer / timeToTransition));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        hair.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Brightness", 6000);

        hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", hairColors[index]);

        timer = 0;
        while (timer < timeToTransition)
        {
            hair.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Brightness", 6 + 6000 * ((timeToTransition - timer) / timeToTransition));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        hair.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Brightness", 6);

        hair.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_DetailTex", 1);
    }

    private IEnumerator HairTransitionColor(int index)
    {
        // Animation
        playerAnimator.SetBool("IsFinish", true);

        float timer = 0;
        float timeToTransition = 1;

        Color lastColor = hair.GetComponent<SkinnedMeshRenderer>().material.GetColor("_Color");

        //Color diffColor = new Color(hairColors[index].r - lastColor.r, hairColors[index].g - lastColor.g, hairColors[index].b - lastColor.b);
        Color diffColor = hairColors[index] - lastColor;

        isRotating = true;

        while (timer < timeToTransition)
        {
            playerModel.transform.Rotate(Vector2.up, spinSpeed * Time.deltaTime);
            hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", lastColor + diffColor * (timer / timeToTransition));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        isRotating = false;

        // Finish animation
        playerAnimator.SetBool("IsFinish", false);
    }

    public bool IsRotating()
    {
        return isRotating;
    }
}
