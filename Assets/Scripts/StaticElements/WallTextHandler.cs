using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(TextMesh))]
public class WallTextHandler : MonoBehaviour
{
    public float textSize = 0.5f;

    private TextMesh text;
    
    void Awake()
    {
        this.text = GetComponent<TextMesh>();
    }

    private void OnDestroy()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void LateUpdate()
    {
        this.transform.rotation = Quaternion.LookRotation(this.transform.position - Camera.main.transform.position, Vector3.up);
        this.text.characterSize = this.textSize * (transform.position - Camera.main.transform.position).magnitude / 100f;
    }

    public void SetText(string inText)
    {
        this.text.text = inText;
    }
}
