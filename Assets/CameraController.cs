using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public bool limitCamera = false;
    public Transform minPosition;
    public Transform maxPosition;

    private Transform target;
    
    public float cameraSpeed = 25f;
    public float targetDistance = 10f;

    private void Update()
    {
        if (!target)
            return;

        float mouseDX = Input.GetAxis("Mouse X");
        float mouseDY = Input.GetAxis("Mouse Y");

        Vector3 targetPosNormY = new Vector3(target.position.x, transform.position.y, target.position.z);

        if (mouseDX != 0) //Rotate the camera
        {
            Vector3 initialDir = transform.forward;
            Vector3 camToTargetDir = (targetPosNormY - transform.position).normalized;

            Vector3 newDirection = Quaternion.AngleAxis(cameraSpeed * mouseDX * Time.deltaTime, Vector3.up) * camToTargetDir;
            transform.position = targetPosNormY - (newDirection * targetDistance);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else  //Keep following the player
        {
            Vector3 TargetToCamDir = (transform.position - targetPosNormY).normalized;
            transform.position = targetPosNormY + TargetToCamDir * targetDistance;
            transform.rotation = Quaternion.LookRotation(TargetToCamDir * -1);
        }

        //Do not exit the scenario
        if(limitCamera)
        {
            if (transform.position.x < minPosition.position.x)
            {
                transform.position = new Vector3(minPosition.position.x, transform.position.y, transform.position.z);
            }
            if (transform.position.x > maxPosition.position.x)
            {
                transform.position = new Vector3(maxPosition.position.x, transform.position.y, transform.position.z);
            }
            if (transform.position.z < minPosition.position.z)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, minPosition.position.z);
            }
            if (transform.position.z > maxPosition.position.z)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, maxPosition.position.z);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
