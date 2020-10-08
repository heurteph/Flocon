using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("Foreground")]

    [SerializeField]
    [Tooltip("Foreground layer")]
    private GameObject foreground;

    [SerializeField]
    [Tooltip("Foreground speed factor relative to the walk layer")]
    [Range(1f, 10f)]
    private float foregroundSpeedFactor = 1.2f;

    [Space]
    [Header("Walkable")]

    [SerializeField]
    [Tooltip("Walkable layer")]
    private GameObject walkable;

    [SerializeField]
    [Tooltip("Walkable speed layer is always the unit reference")]
    [Range(1f, 1f)]
    private float walkableSpeedFactor = 1f;

    [Space]
    [Header("Near Background")]

    [SerializeField]
    [Tooltip("Near background layer")]
    private GameObject nearBackground;

    [SerializeField]
    [Tooltip("Near background speed factor relative to the walk layer")]
    [Range(0f, 1f)]
    private float nearBackgroundSpeedFactor = 0.9f;

    [Space]
    [Header("Background")]

    [SerializeField]
    [Tooltip("Background layer")]
    private GameObject background;

    [SerializeField]
    [Tooltip("Background speed factor relative to the walk layer")]
    [Range(0f, 1f)]
    private float backgroundSpeedFactor = 0.8f;

    [Space]
    [Header("Sky")]

    [SerializeField]
    [Tooltip("Sky layer")]
    private GameObject sky;

    [SerializeField]
    [Tooltip("Sky speed factor relative to the walk layer")]
    [Range(0f, 1f)]
    private float skySpeedFactor = 0.4f;

    private GameObject camera;
    private Vector2 lastCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(foreground != null, "Missing foreground layer");
        Debug.Assert(walkable != null, "Missing walkable layer");
        Debug.Assert(nearBackground != null, "Missing nearbackground layer");
        Debug.Assert(background != null, "Missing background layer");
        Debug.Assert(sky != null, "Missing sky layer");

        camera = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Assert(camera != null, "Missing reference to main camera");
        lastCameraPosition = camera.transform.position;

        // TO DO : Register MoveAllLayers to a camera move event !
    }

    // LateUpdate is called once per frame
    void LateUpdate()
    {
        MoveAllLayers((Vector2)camera.transform.position - lastCameraPosition);
        lastCameraPosition = camera.transform.position;
    }

    public void MoveAllLayers(Vector2 deltaMove)
    {
        foreground.transform.Translate((-1 + foregroundSpeedFactor) * deltaMove, Space.World);
        walkable.transform.Translate((-1 + walkableSpeedFactor) * deltaMove, Space.World);
        nearBackground.transform.Translate((-1 + nearBackgroundSpeedFactor) * deltaMove, Space.World);
        background.transform.Translate((-1 + backgroundSpeedFactor) * deltaMove, Space.World);
        sky.transform.Translate((-1 + skySpeedFactor) * deltaMove, Space.World);
    }
}
