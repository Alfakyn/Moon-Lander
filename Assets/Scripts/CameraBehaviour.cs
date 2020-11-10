using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform lander_transform;
    private Vector3 offset = new Vector3(0.0f, 0.0f, -1.0f);

    private Vector3 overview = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 zoom = new Vector3(0.5f, 0.5f, 0.5f);

    private Vector3 original_camera_position = new Vector3(0.0f, 0.0f, -10.0f);

    private float screen_width = 1920.0f;
    private float screen_heigth = 1080.0f;

    private enum CameraState {overview, zoom};
    private CameraState camera_state;

    public LanderBehaviour lander_behaviour;
    private const float ALTITUDE_ZOOM_THRESHOLD = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        camera_state = CameraState.overview;
    }

    // Update is called once per frame
    void Update()
    {
        checkZoom();

        switch (camera_state)
        {
            case CameraState.overview:
                break;
            case CameraState.zoom:
                cameraFollow();
                break;
        }
    }
    
    private void cameraFollow()
    {
        if (lander_transform.position.x < -(screen_width/4.0f))
        {
            transform.position = new Vector3(-(screen_width / 4.0f), lander_transform.position.y, transform.position.z);
            if (lander_transform.position.y < -(screen_heigth / 4))
            {
                transform.position = new Vector3(-(screen_width / 4.0f), -(screen_heigth / 4), transform.position.z);
            }
        }
        else if(lander_transform.position.x > (screen_width / 4.0f))
        {
            transform.position = new Vector3((screen_width / 4.0f), lander_transform.position.y, transform.position.z);
            if (lander_transform.position.y < -(screen_heigth / 4))
            {
                transform.position = new Vector3((screen_width / 4.0f), -(screen_heigth / 4), transform.position.z);
            }
        }
        else if(lander_transform.position.y < -(screen_heigth/4))
        {
            transform.position = new Vector3(lander_transform.position.x, -(screen_heigth / 4), transform.position.z);
        }
        else
        {
            transform.position = lander_transform.position + offset;
        }
    }

    private void changeCameraState(CameraState next_camera_state)
    {
        camera_state = next_camera_state;
        switch (camera_state)
        {
            case CameraState.overview:
                transform.localScale = overview;
                transform.position = original_camera_position;
                break;
            case CameraState.zoom:
                transform.localScale = zoom;
                break;
        }
    }

    private void checkZoom()
    {
        float altitude = lander_behaviour.getAltitude();
        if (altitude <= ALTITUDE_ZOOM_THRESHOLD && camera_state == CameraState.overview)
        {
            changeCameraState(CameraState.zoom);
        }
        else if (altitude > ALTITUDE_ZOOM_THRESHOLD && camera_state == CameraState.zoom)
        {
            changeCameraState(CameraState.overview);
        }
    }
}
