using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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

        if (mouseDX != 0)
        {
            Vector3 initialDir = transform.forward;
            Vector3 camToTargetDir = (targetPosNormY - transform.position).normalized;

            Vector3 newDirection = Quaternion.AngleAxis(cameraSpeed * mouseDX * Time.deltaTime, Vector3.up) * camToTargetDir;
            transform.position = targetPosNormY - (newDirection * targetDistance);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            Vector3 TargetToCamDir = (transform.position - targetPosNormY).normalized;
            transform.position = targetPosNormY + TargetToCamDir * targetDistance;
            transform.rotation = Quaternion.LookRotation(TargetToCamDir * -1);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }





}
