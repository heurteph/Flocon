using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuFading : MonoBehaviour
{
    public Image m_blackCanvas;
    int m_time = 255;
    float m_timer = 0.0f;

    void Start()
    {
        m_blackCanvas.color = new Color32(0,0,0,255);
        m_time = 255;
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= 1)
        {
            m_time -= 1;
            m_blackCanvas.color = new Color(0, 0, 0, m_time);
            m_timer = 0;
        }
    }
}
