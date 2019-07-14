using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class ShadowMap : MonoBehaviour
{
    public Light sun;
    public GameObject plane;

    public RawImage debugTexture;
    private RenderTexture shadowmapCopy;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            UpdateShadowMap();
    }

    void UpdateShadowMap()
    {
        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        shadowmapCopy = new RenderTexture(2048, 1024, 0);
        CommandBuffer cb = new CommandBuffer();

        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cb.Blit(shadowmap, new RenderTargetIdentifier(shadowmapCopy));
        sun.AddCommandBuffer(LightEvent.AfterShadowMap, cb);

        Camera.main.Render();
        RenderTexture.active = shadowmapCopy;
        Texture2D tmp = new Texture2D(shadowmapCopy.width, shadowmapCopy.height);
        tmp.ReadPixels(new Rect(0, 0, shadowmapCopy.width, shadowmapCopy.height), 0, 0);
        tmp.Apply();
        Texture2D pixelArray = new Texture2D(shadowmapCopy.width / 2, shadowmapCopy.height);
        for (int y = 0; y < tmp.height; y++)
            for (int x = 0; x < pixelArray.width; x++)
                 pixelArray.SetPixel(x, y, new Color(tmp.GetPixel(x + pixelArray.width, y).r, 0, 0, 1));
        pixelArray.Apply();
        debugTexture.texture = pixelArray;
        //sun.RemoveCommandBuffer(LightEvent.AfterShadowMap, cb);
    }
}