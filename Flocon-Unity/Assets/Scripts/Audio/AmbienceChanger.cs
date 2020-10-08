using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceChanger : MonoBehaviour
{
    [Header("Parameter")]
    public bool m_changeParameter = true;
    public string m_parameterName;
    public float m_parameterValue;

    [Header("Event Trigger")]
    public bool m_triggerEvent = false;
    public string m_eventName = "event:/Example";

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
            if(m_changeParameter)
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName(m_parameterName, m_parameterValue);
            }

            if(m_triggerEvent)
            {
                FMODUnity.RuntimeManager.PlayOneShot(m_eventName);
            }
        }

        Debug.Log("Triggered");
    }
}
