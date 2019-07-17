﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class ShadowMap : MonoBehaviour
{
    public static ShadowMap instance = null;
    public DayNightController dayNightController;
    public GameObject shadowFilter;
    public RawImage shadowMapTexture;

    private Light sun;
    private Camera shadowCamera;
    private RenderTexture shadowmapCopy;
    private Texture2D pixelArray;
    private Vector2 startPoint;
    private float toTextureRatio;
    private CommandBuffer cb;

    private const float vHour = 1f / 24f;
    private const int planeSize = 1000;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        sun = dayNightController.GetComponent<Light>();
        shadowCamera = GetComponent<Camera>();
        shadowmapCopy = new RenderTexture(2048, 1024, 0);
        cb = new CommandBuffer();
        Calibrate();
    }

    public float GetSunExposure(float xPos, float yPos)
    {
        return (pixelArray.GetPixel((int)(startPoint.x + (xPos * toTextureRatio)), (int)(startPoint.y + (yPos * toTextureRatio))).r);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            UpdateShadowMap();
    }

    private void Calibrate()
    {
        float currentTime = dayNightController.targetTime;
        StartCapture();
        shadowFilter.SetActive(false);

        Texture2D tmp = CaptureShadowMap(4);//capture at (8 + 4) 12H00 to minimize shadows
        int offset = tmp.width / 2;
        for (int y = 0; y < tmp.height; y++)
            for (int x = 0; x < offset; x++)
                if (tmp.GetPixel(x + offset, y).r != 0)
                {
                    startPoint.x = x;
                    startPoint.y = y;
                    while (x < tmp.width && tmp.GetPixel(x + offset, y).r != 0)
                        x++;
                    toTextureRatio = planeSize / (x - startPoint.x);
                    goto EndCapture;
                }

        EndCapture:
        EndCapture(currentTime);
    }

    private void StartCapture()
    {
        shadowCamera.enabled = true;
        shadowFilter.SetActive(true);

        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cb.Blit(shadowmap, new RenderTargetIdentifier(shadowmapCopy));
        sun.AddCommandBuffer(LightEvent.AfterShadowMap, cb);
    }

    private void UpdateShadowMap(float max = 8)
    {
        float currentTime = dayNightController.targetTime;

        Debug.Log("=> 10%");
        pixelArray = null;
        pixelArray = new Texture2D(shadowmapCopy.width / 2, shadowmapCopy.height);
        for (int y = 0; y < pixelArray.height; y++)
            for (int x = 0; x < pixelArray.width; x++)
                pixelArray.SetPixel(x, y, new Color(1, 1, 1));

        float step;
        Texture2D tmp = null;
        for (int i = 0; i < max; i++)
        {
            step = 0;
            while (step < 1)
            {
                tmp = CaptureShadowMap((float)i + step);
                FilterDefaultValue(tmp, ref pixelArray, max);
                step += 0.1f;
            }
            Debug.Log("=> " + (i + 2) * 10 + "%");
            //Camera.main.Render();
        }

        pixelArray.Apply();
        Debug.Log("=> 100%");
    }

    private void EndCapture(float currentTime)
    {
        shadowMapTexture.texture = pixelArray;
        sun.RemoveCommandBuffer(LightEvent.AfterShadowMap, cb);
        shadowCamera.enabled = false;
        shadowFilter.SetActive(false);
        dayNightController.InstantSetTimeofDay(currentTime);
    }

    private Texture2D CaptureShadowMap(float hourOffset)
    {
        dayNightController.InstantSetTimeofDay(vHour * (8f + hourOffset));
        shadowCamera.Render();
        RenderTexture.active = shadowmapCopy;
        Texture2D tmp = new Texture2D(shadowmapCopy.width, shadowmapCopy.height);
        tmp.ReadPixels(new Rect(0, 0, shadowmapCopy.width, shadowmapCopy.height), 0, 0);
        tmp.Apply();
        return tmp;
    }

    private void FilterDefaultValue(Texture2D tmp, ref Texture2D pixelArray, float stepsNumber)
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
                {
                    float updatedValue = pixelArray.GetPixel(x, y).r - (pixelValue / stepsNumber);
                    pixelArray.SetPixel(x, y, new Color(updatedValue, updatedValue, updatedValue, 1));
                }
            }
        }
    }
}