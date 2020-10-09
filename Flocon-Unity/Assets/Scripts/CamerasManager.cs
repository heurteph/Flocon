using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamerasManager : MonoBehaviour
{
    private GameObject inputsManager;

    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;
    public GameObject cam4;


    public GameObject t1;
    public GameObject t2;
    public GameObject t3;
    public GameObject t4;
    public GameObject t5;
    public GameObject t6;

    // Start is called before the first frame update
    void Start()
    {
        t1.GetComponent<CamerasTrigger>().TriggerEvent += WinterToFall;
        t2.GetComponent<CamerasTrigger>().TriggerEvent += FallToWinter;
        t3.GetComponent<CamerasTrigger>().TriggerEvent += FallToSummer;
        t4.GetComponent<CamerasTrigger>().TriggerEvent += SummerToFall;
        t5.GetComponent<CamerasTrigger>().TriggerEvent += SummerToSpring;
        t6.GetComponent<CamerasTrigger>().TriggerEvent += SpringToSummer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WinterToFall()
    {
        cam1.SetActive(false);
        cam2.SetActive(true);
        Debug.Log("Winter to Fall");
    }

    public void FallToWinter()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
        Debug.Log("Fall to Winter");
    }

    public void FallToSummer()
    {
        cam2.SetActive(false);
        cam3.SetActive(true);
    }
    public void SummerToFall()
    {
        cam2.SetActive(true);
        cam3.SetActive(false);
    }

    public void SummerToSpring()
    {
        cam3.SetActive(false);
        cam4.SetActive(true);
    }
    public void SpringToSummer()
    {
        cam3.SetActive(true);
        cam4.SetActive(false);
    }
}
