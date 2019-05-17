using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject plane;
    public bool inputEnabled = true;

    public const float cameraMoveSpeed = 20f;
    public const float cameraRotateSpeed = 100f;
    public const float cameraZoomSpeed = 250f;
    public const bool canMoveCameraWithMouse = false;
    public const float moveBorderThickness = 10f;
    public const float mousePitchDirection = -1f; //Inverse Pitch
    public const float minAltitude = 0.5f;
    public const float maxAltitude = 100f;
    public const float minAltitude2D = 0.5f;
    public const float maxAltitude2D = 58f;

    private Camera camera;
    private Vector2 lowerPlaneBound;
    private Vector2 upperPlaneBound;
    private bool changeMod = false;

    private Matrix4x4 perspective;
    private float aspect;
    private float far = 1000f;
    private float near = .3f;
    private const float fov = 60f;
    private const float To2D = ((maxAltitude2D - minAltitude2D) / (maxAltitude - minAltitude));
    private const float From2D = ((maxAltitude - minAltitude) / (maxAltitude2D - minAltitude2D));

    private void Start()
    {
        camera = GetComponent<Camera>();
        this.lowerPlaneBound = new Vector2(
            this.plane.transform.position.x - (5f * this.plane.transform.localScale.x),
            this.plane.transform.position.z - (5f * this.plane.transform.localScale.z)
            );
        this.upperPlaneBound = new Vector2(
            this.plane.transform.position.x + (5f * this.plane.transform.localScale.x),
            this.plane.transform.position.z + (5f * this.plane.transform.localScale.z)
            );
        if (Screen.width == 0f || Screen.height == 0f)
            aspect = 1.7f;
        else
            aspect = (float)Screen.width / (float)Screen.height;
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
    }

    public void ChangeViewMod()
    {
        changeMod = false;
        if (!this.camera.orthographic)
        {
            this.transform.eulerAngles = new Vector3(90f, 0f, -this.transform.eulerAngles.y);
            float tmp = this.transform.position.y * To2D;
            this.camera.orthographic = true;
            this.camera.projectionMatrix = Matrix4x4.Ortho(-tmp * this.aspect, tmp * this.aspect, -tmp, tmp, this.near, this.far);
            this.camera.orthographicSize = tmp;
            this.transform.position = new Vector3(
                this.transform.position.x,
                maxAltitude,
                this.transform.position.z);
        }
        else
        {
            this.transform.eulerAngles = new Vector3(89f, this.transform.eulerAngles.y, 0f);
            this.camera.orthographic = false;
            this.camera.projectionMatrix = this.perspective;
            this.transform.position = new Vector3(
                this.transform.position.x,
                this.camera.orthographicSize * From2D,
                this.transform.position.z);
        }
    }

    private void Update()
    {
        Vector3 currentPos = this.transform.position;
        Quaternion currentRot = this.transform.rotation;

        if (!inputEnabled)
            return;
        if (Input.GetKeyDown(KeyCode.Keypad5))//TODO UI BUTTON
            changeMod = true;


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

        if (!this.camera.orthographic)
        {
            if (Input.GetKey("r"))
                currentRot = RotatePitch(currentRot, -1f);
            if (Input.GetKey("f"))
                currentRot = RotatePitch(currentRot, 1f);
            if (Input.GetMouseButton(1))
            {
                currentRot = RotateYaw(currentRot, Input.GetAxis("Mouse X"));
                currentRot = RotatePitch(currentRot, mousePitchDirection * Input.GetAxis("Mouse Y"));
            }

            currentPos = Zoom3D(currentPos, -Input.GetAxis("Mouse ScrollWheel"));
        }
        else
        {
            if (Input.GetMouseButton(1))
                currentRot = RotateYaw(currentRot, Input.GetAxis("Mouse X"));
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                Zoom2D(Input.GetAxis("Mouse ScrollWheel"));
        }

        this.transform.position = currentPos;
        this.transform.rotation = currentRot;

        if (changeMod)
            ChangeViewMod();
    }

    private Vector3 MoveForward(Vector3 currentPos, float axisInput)
    {
        Vector3 newPos = currentPos;
        Vector3 step = (transform.forward + transform.up) * axisInput * cameraMoveSpeed * Time.deltaTime;

        newPos.x += step.x;
        if (newPos.x < lowerPlaneBound.x || newPos.x > upperPlaneBound.x)
            return currentPos;

        newPos.z += step.z;
        if (newPos.z < lowerPlaneBound.y || newPos.z > upperPlaneBound.y)
            return currentPos;
        return newPos;
    }

    private Vector3 MoveRight(Vector3 currentPos, float axisInput)
    {
        Vector3 newPos = currentPos;
        Vector3 step = transform.right * axisInput * cameraMoveSpeed * Time.deltaTime;

        newPos.x += step.x;
        if (newPos.x < lowerPlaneBound.x || newPos.x > upperPlaneBound.x)
            return currentPos;

        newPos.z += step.z;
        if (newPos.z < lowerPlaneBound.y || newPos.z > upperPlaneBound.y)
            return currentPos;
        return newPos;
    }

    private Vector3 Zoom3D(Vector3 currentPos, float axisInput)
    {
        float tmp = currentPos.y + (axisInput * cameraZoomSpeed * Time.deltaTime);
        if ((axisInput <= 0 && tmp > minAltitude) || (axisInput > 0 && tmp < maxAltitude))
            currentPos.y = tmp;
        return currentPos;
    }

    private void Zoom2D(float axisInput)
    {
        float tmp = this.camera.orthographicSize - axisInput * To2D;
        this.camera.projectionMatrix = Matrix4x4.Ortho(-tmp * this.aspect, tmp * this.aspect, -tmp, tmp, this.near, this.far);
        this.camera.orthographicSize = tmp;
    }

    private Quaternion RotateYaw(Quaternion currentRotation, float axisInput)
    {
        Quaternion tmp = currentRotation;

        Vector3 rotateValue = new Vector3(0, (axisInput * cameraRotateSpeed * Time.deltaTime), 0);
        tmp.eulerAngles += rotateValue;
        return tmp;
    }

    private Quaternion RotatePitch(Quaternion currentRotation, float axisInput)
    {
        Quaternion tmp = currentRotation;

        Vector3 rotateValue = new Vector3((axisInput * cameraRotateSpeed * Time.deltaTime), 0, 0);
        tmp.eulerAngles += rotateValue;
        if ((rotateValue.x + tmp.eulerAngles.x) > 90f || (rotateValue.x + tmp.eulerAngles.x) < 0f)
            return currentRotation;
        return tmp;
    }
}
