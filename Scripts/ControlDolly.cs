using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class ControlDolly : MonoBehaviour
{
    public float sensitivity;

    [SerializeField]
    [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
    private bool enableMovement = true;

    [SerializeField]
    [Tooltip("Camera movement speed")]
    private float movementSpeed = 0.005f;

    [SerializeField]
    [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    private float boostedSpeed = 0.01f;

    [SerializeField]
    [Tooltip("Boost speed")]
    private KeyCode boostSpeed = KeyCode.LeftShift;

    [SerializeField]
    [Tooltip("Toggle pov")]
    private KeyCode togglePov = KeyCode.LeftAlt;

    [SerializeField]
    [Tooltip("This keypress will move the camera to initialization position")]
    private KeyCode initPositonButton = KeyCode.R;

    private Camera mainCam;
    private CinemachineVirtualCamera cam;
    private CinemachineTrackedDolly dolly;
    private CinemachinePOV pov;
    private float maxValue;
    private void Awake()
    {
        mainCam = Camera.main;
        cam = GetComponent<CinemachineVirtualCamera>();
        dolly = cam.GetCinemachineComponent<CinemachineTrackedDolly>();
        maxValue = (float)((CinemachineSmoothPath)dolly.m_Path).m_Waypoints.Length - 1f;
        pov = cam.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if(Input.GetKeyDown(togglePov))
        {
            if(pov == null)
            {
                pov = cam.AddCinemachineComponent<CinemachinePOV>();
                pov.m_VerticalAxis.m_MaxSpeed = sensitivity;
                pov.m_HorizontalAxis.m_MaxSpeed = sensitivity;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                cam.DestroyCinemachineComponent<CinemachinePOV>();
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (enableMovement)
        {
            float mov = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(boostSpeed))
                mov *= boostedSpeed;
            else
                mov *= movementSpeed;

            if (Input.GetKeyDown(initPositonButton))
                dolly.m_PathPosition = 0f;
            else if (mov != 0f)
            {
                Vector3 dir = dolly.m_Path.EvaluateTangent(dolly.m_PathPosition).normalized;
                mov *= Vector3.Dot(dir, mainCam.transform.forward);

                dolly.m_PathPosition = Mathf.Clamp(dolly.m_PathPosition + mov * Time.deltaTime, 0f, maxValue);
            }
        }
    }
}
