using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject plane;
    public bool inputEnabled = true;
    public bool zoomEnabled = true;

    public const float To2D = ((maxAltitude2D - minAltitude2D) / (maxAltitude - minAltitude));
    public const float From2D = ((maxAltitude - minAltitude) / (maxAltitude2D - minAltitude2D));

    private const float cameraBaseMoveSpeed = 10f;
    private const float cameraRotateSpeed = 100f;
    private const float cameraZoomSpeed = 250f;
    private const float mousePitchDirection = -1f; //Inverse Pitch
    private const float minAltitude = 0.5f;
    private const float maxAltitude = 100f;
    private const float minAltitude2D = 0.5f;
    private const float maxAltitude2D = 58f;

    private Camera currentCamera;
    private float cameraMoveSpeed = cameraBaseMoveSpeed;
    private Vector2 lowerPlaneBound;
    private Vector2 upperPlaneBound;
    private bool changeMod = false;

    private Matrix4x4 perspective;
    private float aspect;
    private float far = 1000f;
    private float near = .3f;
    private const float fov = 60f;

    private void Start()
    {
        currentCamera = GetComponent<Camera>();
        lowerPlaneBound = new Vector2(
            plane.transform.position.x - (5f * plane.transform.localScale.x),
            plane.transform.position.z - (5f * plane.transform.localScale.z)
            );
        upperPlaneBound = new Vector2(
            plane.transform.position.x + (5f * plane.transform.localScale.x),
            plane.transform.position.z + (5f * plane.transform.localScale.z)
            );
        if (Screen.width == 0f || Screen.height == 0f)
            aspect = 1.7f;
        else
            aspect = (float)Screen.width / (float)Screen.height;
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        cameraMoveSpeed = cameraBaseMoveSpeed + cameraBaseMoveSpeed * transform.position.y * 10f / maxAltitude;
    }

    public void ChangeViewMod()
    {
        changeMod = false;
        if (!currentCamera.orthographic)
        {
            transform.eulerAngles = new Vector3(90f, 0f, -transform.eulerAngles.y);
            float tmp = transform.position.y * To2D;
            currentCamera.orthographic = true;
            currentCamera.projectionMatrix = Matrix4x4.Ortho(-tmp * aspect, tmp * aspect, -tmp, tmp, near, far);
            currentCamera.orthographicSize = tmp;
            transform.position = new Vector3(
                transform.position.x,
                maxAltitude,
                transform.position.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(89f, transform.eulerAngles.y, 0f);
            currentCamera.orthographic = false;
            currentCamera.projectionMatrix = perspective;
            transform.position = new Vector3(
                transform.position.x,
                currentCamera.orthographicSize * From2D,
                transform.position.z);
        }
    }

    public void SetChangeMod(bool state) { changeMod = state; }

    public void ChangeZoom(int amount)
    {
        if (currentCamera.orthographic)
            Zoom2D(amount);
        else
            transform.position = Zoom3D(transform.position, amount);
    }


    private void Update()
    {
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        if (!inputEnabled)
            return;

        if (Input.GetAxis("Vertical") != 0f)
            currentPos = MoveForward(currentPos, Input.GetAxis("Vertical"));

        if (Input.GetAxis("Horizontal") != 0f)
            currentPos = MoveRight(currentPos, Input.GetAxis("Horizontal"));

        if (!currentCamera.orthographic)
        {
            if (Input.GetMouseButton(1))
            {
                currentRot = RotateYaw(currentRot, Input.GetAxis("Mouse X"));
                currentRot = RotatePitch(currentRot, mousePitchDirection * Input.GetAxis("Mouse Y"));
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0f && zoomEnabled)
                currentPos = Zoom3D(currentPos, -Input.GetAxis("Mouse ScrollWheel"));
        }
        else
        {
            if (Input.GetMouseButton(1))
                currentRot = RotateYaw(currentRot, Input.GetAxis("Mouse X"));
            if (Input.GetAxis("Mouse ScrollWheel") != 0f && zoomEnabled)
                Zoom2D(Input.GetAxis("Mouse ScrollWheel"));
        }

        transform.position = currentPos;
        transform.rotation = currentRot;

        if (changeMod)
            ChangeViewMod();
    }

    //Move
    private Vector3 MoveForward(Vector3 currentPos, float axisInput)
    {
        Vector3 newPos = currentPos;
        Vector3 step = ((transform.forward + transform.up) / 2f) * axisInput * cameraMoveSpeed * Time.deltaTime;

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

    //Zoom
    private Vector3 Zoom3D(Vector3 currentPos, float axisInput)
    {
        float tmp = currentPos.y + (axisInput * cameraZoomSpeed * Time.deltaTime);
        if ((axisInput <= 0 && tmp > minAltitude) || (axisInput > 0 && tmp < maxAltitude))
            currentPos.y = tmp;
        cameraMoveSpeed = cameraBaseMoveSpeed + cameraBaseMoveSpeed * currentPos.y * 10f / maxAltitude;
        return currentPos;
    }

    private void Zoom2D(float axisInput)
    {
        float tmp = currentCamera.orthographicSize - axisInput * To2D;
        currentCamera.projectionMatrix = Matrix4x4.Ortho(-tmp * aspect, tmp * aspect, -tmp, tmp, near, far);
        currentCamera.orthographicSize = tmp;
    }

    //Rotation
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

    public void ToogleInput()
    {
        inputEnabled = !inputEnabled;
    }
}
