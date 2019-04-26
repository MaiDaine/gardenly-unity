using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraMoveSpeed = 20f;
    public float cameraRotateSpeed = 100f;
    public float cameraZoomSpeed = 250f;
    public bool canMoveCameraWithMouse = true;
    public bool inputEnabled = true;
    public float moveBorderThickness = 10f;
    public float mousePitchDirection = -1f; //Inverse Pitch
    public float minAltitude = 0.5f;
    public float maxAltitude = 100f;

    void Update()
    {
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        if (!inputEnabled)
            return;

        if ((!Input.GetKey(KeyCode.LeftControl) && Input.GetKey("z")) || (canMoveCameraWithMouse && Input.mousePosition.y >= Screen.height - moveBorderThickness))
            currentPos = MoveForward(currentPos, 1f);
        if (Input.GetKey("s") || (canMoveCameraWithMouse && Input.mousePosition.y <= moveBorderThickness))
            currentPos = MoveForward(currentPos, -1f);
        if (Input.GetKey("d") || (canMoveCameraWithMouse && Input.mousePosition.x >= Screen.width - moveBorderThickness))
            currentPos = MoveRight(currentPos, 1f);
        if (Input.GetKey("q") || (canMoveCameraWithMouse && Input.mousePosition.x <= moveBorderThickness))
            currentPos = MoveRight(currentPos, -1f);

        if (Input.GetKey("e"))
            currentRot = RotateYaw(currentRot, 1f);
        if (Input.GetKey("a"))
            currentRot = RotateYaw(currentRot, -1f);
        if (Input.GetKey("f"))
            currentRot = RotatePitch(currentRot, 1f);
        if (Input.GetKey("r"))
            currentRot = RotatePitch(currentRot, -1f);
        if (Input.GetMouseButton(1))
        {
            currentRot = RotateYaw(currentRot, Input.GetAxis("Mouse X"));
            currentRot = RotatePitch(currentRot, mousePitchDirection * Input.GetAxis("Mouse Y"));
        }

        currentPos = Zoom(currentPos, -1f * Input.GetAxis("Mouse ScrollWheel"));

        transform.position = currentPos;
        transform.rotation = currentRot;
    }

    Vector3 MoveForward(Vector3 currentPos, float axisInput)
    {
        Vector3 newPos = (transform.forward + transform.up) * axisInput * cameraMoveSpeed * Time.deltaTime;
        currentPos.x += newPos.x;
        currentPos.z += newPos.z;
        return currentPos;
    }
    
    Vector3 MoveRight(Vector3 currentPos, float axisInput)
    {
        Vector3 newPos = transform.right * axisInput * cameraMoveSpeed * Time.deltaTime;
        currentPos.x += newPos.x;
        currentPos.z += newPos.z;
        return currentPos;
    }

    Vector3 Zoom(Vector3 currentPos, float axisInput)
    {
        float tmp = currentPos.y + (axisInput * cameraZoomSpeed * Time.deltaTime);
        if ((axisInput <= 0 && tmp > minAltitude) || (axisInput > 0 && tmp < maxAltitude))
            currentPos.y = tmp;
        return currentPos;
    }

    Quaternion RotateYaw(Quaternion currentRotation, float axisInput)
    {
        Vector3 rotateValue = new Vector3(0, (axisInput * cameraRotateSpeed * Time.deltaTime), 0);
        currentRotation.eulerAngles += rotateValue;
        return currentRotation;
    }

    Quaternion RotatePitch(Quaternion currentRotation, float axisInput)
    {
        Quaternion tmp = currentRotation;

        Vector3 rotateValue = new Vector3((axisInput * cameraRotateSpeed * Time.deltaTime), 0, 0);
        tmp.eulerAngles += rotateValue;
        if (tmp.x > -0.7f) //90°
            return tmp;
        return currentRotation;
    }
}
