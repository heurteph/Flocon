using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CamerasTrigger : MonoBehaviour
{
    public UnityAction action;

    private GameObject camerasManager;

    public delegate void TriggerHandler();
    public event TriggerHandler TriggerEvent;

    // Start is called before the first frame update
    void Start()
    {
        camerasManager = GameObject.FindGameObjectWithTag("CamerasManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger camera");
        TriggerEvent();
    }
}
