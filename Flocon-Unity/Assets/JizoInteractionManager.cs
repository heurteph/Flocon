using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JizoInteractionManager : MonoBehaviour
{
    public GameObject bulle;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            bulle.SetActive(true);
            Debug.Log("Jizo 01 Triggered");
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            bulle.SetActive(false);
        }
    }
}
