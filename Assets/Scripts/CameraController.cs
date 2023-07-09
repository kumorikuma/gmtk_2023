using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController> {
    [NonNullField]
    public Transform player;

    public bool AllowGoingDown = false;

    public float transitionY = -30;
    public float lowerY = -50;
    public float upperY = -20;
    public float smoothTime = 0.25f;
    Vector3 target;
    Vector3 currentVelocity;

    private void LateUpdate()
    {
        SetTarget();
        MoveCamera();
    }

    void SetTarget()
    {
        if (AllowGoingDown && player.position.y < transitionY) {
            target = new Vector3(0f, lowerY, 0f);
        } else {
            target = new Vector3(0f, upperY, 0f);
        }
    }
    
    void MoveCamera()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, smoothTime);
    }

}
