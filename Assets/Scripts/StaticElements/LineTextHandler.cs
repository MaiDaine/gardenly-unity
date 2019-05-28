using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class LineTextHandler : MonoBehaviour
{
    public float textSize = 0.5f;

    private Camera mainCamera;
    private TextMesh text;

    private void Awake()
    {
        text = GetComponent<TextMesh>();
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        GameObject.Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (mainCamera.orthographic)
        {
            transform.eulerAngles = new Vector3(90f, 180 + mainCamera.transform.eulerAngles.y / 90 * 90, 0f);
            text.characterSize = textSize * (transform.position.y - (mainCamera.orthographicSize * CameraController.From2D)) / 100f;
        }
        else
        {

            transform.eulerAngles = new Vector3(90f, mainCamera.transform.eulerAngles.y / 90 * 90, 0f);
            text.characterSize = textSize * (transform.position - mainCamera.transform.position).magnitude / 100f;
        }
    }

    public void SetColor(Color color)
    {
        text.color = color;
    }

    public void SetText(string inText)
    {
        text.text = inText;
    }
}
