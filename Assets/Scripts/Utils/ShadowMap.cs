using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class ShadowMap : MonoBehaviour
{
    public DayNightController dayNightController;
    public GameObject shadowFilter;

    public RawImage debugTexture;
    public List<RawImage> debugTextures;

    private Light sun;
    private Camera shadowCamera;
    private RenderTexture shadowmapCopy;
    private CommandBuffer cb;
    private const float vHour = 1f / 24f;

    private void Start()
    {
        sun = dayNightController.GetComponent<Light>();
        shadowCamera = GetComponent<Camera>();
        shadowmapCopy = new RenderTexture(2048, 1024, 0);
        cb = new CommandBuffer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            UpdateShadowMap();
        if (Input.GetKeyDown(KeyCode.N))
            dayNightController.InstantSetTimeofDay((dayNightController.targetTime + vHour) % 1f);
    }

    private void UpdateShadowMap()
    {
        shadowCamera.enabled = true;
        shadowFilter.SetActive(true);

        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cb.Blit(shadowmap, new RenderTargetIdentifier(shadowmapCopy));
        sun.AddCommandBuffer(LightEvent.AfterShadowMap, cb);

        float currentTime = dayNightController.targetTime;
        float max = 4;

        Texture2D pixelArray = new Texture2D(shadowmapCopy.width / 2, shadowmapCopy.height);
        for (int y = 0; y < pixelArray.height; y++)
            for (int x = 0; x < pixelArray.width; x++)
                pixelArray.SetPixel(x, y, new Color(0, 0, 0));

        float step;
        for (int i = 0; i < max; i++)
        {
            step = 0;
            Texture2D tmp = null;
            while (step < 1)
            {
                tmp = CaptureShadowMap((((float)i + step) * vHour));
                FilterDefaultValue(tmp, ref pixelArray);
                step += 0.25f;
            }
            pixelArray.Apply();
            Texture2D debug = new Texture2D(pixelArray.width, pixelArray.height);
            debug.SetPixels(tmp.GetPixels(tmp.width / 2, 0, tmp.width / 2, tmp.height));
            debug.Apply();
            debugTextures[i].texture = debug;
        }

        pixelArray.Apply();
        debugTexture.texture = pixelArray;
        sun.RemoveCommandBuffer(LightEvent.AfterShadowMap, cb);
        shadowCamera.enabled = false;
        shadowFilter.SetActive(false);
        dayNightController.InstantSetTimeofDay(currentTime);
    }

    private Texture2D CaptureShadowMap(float hourOffset)
    {
        dayNightController.InstantSetTimeofDay(vHour * 12f + (vHour * hourOffset));
        shadowCamera.Render();
        RenderTexture.active = shadowmapCopy;
        Texture2D tmp = new Texture2D(shadowmapCopy.width, shadowmapCopy.height);
        tmp.ReadPixels(new Rect(0, 0, shadowmapCopy.width, shadowmapCopy.height), 0, 0);
        tmp.Apply();
        return tmp;
    }

    private void FilterDefaultValue(Texture2D tmp, ref Texture2D pixelArray)
    {
        float pixelValue;
        float filterValue;
        for (int y = 0; y < tmp.height; y++)
        {
            filterValue = tmp.GetPixel(pixelArray.width, y).r;
            for (int x = 1; x < pixelArray.width; x++)
            {
                pixelValue = tmp.GetPixel(x + pixelArray.width, y).r;
                if (pixelValue != filterValue)
                    pixelArray.SetPixel(x, y, new Color(pixelArray.GetPixel(x, y).r + pixelValue, 0, 0, 1));
            }
        }
    }
}