using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class ShadowMap : MonoBehaviour
{
    public static ShadowMap instance = null;

    public DayNightController dayNightController;
    public GradientScript gradientScript;
    public LoadingShadowScript loadingBar;
    public GameObject shadowFilter;
    public RawImage shadowMapTexture;
    public float capturedFramesNumber = 12f;
    public int startShadowCalc = 0;

    private Light sun;
    private Camera shadowCamera;
    private RenderTexture shadowmapCopy;
    private Texture2D pixelArray = null;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private float toTextureRatio;
    private CommandBuffer cb;
    private float frameUpdatePending = 0;
    private float max = 0;

    private const float planeSize = 100f;

    private void Awake()
    {
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = ShadowResolution.High;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        QualitySettings.shadowNearPlaneOffset = 3f;
        QualitySettings.shadowmaskMode = ShadowmaskMode.Shadowmask;
        QualitySettings.shadowDistance = 100f;
        QualitySettings.shadowCascades = 2;
        QualitySettings.shadowCascade4Split = new Vector3(0.1f, 0.2f, 0.5f);
        QualitySettings.shadowCascade2Split = 0.3333333f;

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
        startPoint = new Vector2(1207, 182);
        endPoint = new Vector2(1866, 841);
    }

    public float GetSunExposure(float xPos, float yPos)
    {
        if (pixelArray == null)
            return 0;
        int x = (int)(startPoint.x + (xPos * toTextureRatio));
        int y = (int)(startPoint.y + (-yPos * toTextureRatio));
        x = pixelArray.width - 1 - x;

        return (pixelArray.GetPixel(x, y).r);
    }

    private void Update()
    {
        if (startShadowCalc == 1 && frameUpdatePending <= 0)
        {
            ClearPixelArray();
            MessageHandler.instance.Message("shadow_map", "loading");
            frameUpdatePending = capturedFramesNumber;
        }
        if (frameUpdatePending > 0 && startShadowCalc == 1)
            UpdateShadowMap();
    }

    private void Calibrate()
    {
        float currentTime = dayNightController.targetTime;
        float currentShadowSetting = QualitySettings.shadowDistance;

        StartCapture(false);
        Texture2D tmp = CaptureShadowMap(6);//capture at (6 + 4) 12H00 to minimize shadows
        int offset = tmp.width / 2;
        startPoint.x = offset + offset / 2;
        startPoint.y = 0;

        while (startPoint.y < tmp.height && tmp.GetPixel((int)startPoint.x, (int)startPoint.y).r == 0f)
            startPoint.y++;
        startPoint.x = offset;
        while (startPoint.x < tmp.width && tmp.GetPixel((int)startPoint.x, (int)startPoint.y).r == 0f)
            startPoint.x++;

        endPoint.x = startPoint.x;
        endPoint.y = startPoint.y;

        while (endPoint.x < tmp.width && tmp.GetPixel((int)endPoint.x, (int)endPoint.y).r != 0f)
            endPoint.x++;
        while (endPoint.y < tmp.height && tmp.GetPixel((int)startPoint.x, (int)endPoint.y).r != 0f)
            endPoint.y++;

        toTextureRatio = (endPoint.x - startPoint.x) / planeSize;
        EndCapture(currentTime, currentShadowSetting);
    }

    private void StartCapture(bool setActive = true)
    {
        QualitySettings.shadowDistance = 100f;
        shadowCamera.enabled = true;
        shadowFilter.SetActive(setActive);
        max = 0f;

        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cb.Blit(shadowmap, new RenderTargetIdentifier(shadowmapCopy));
        sun.AddCommandBuffer(LightEvent.AfterShadowMap, cb);
    }

    private void UpdateShadowMap()
    {
        float currentShadowSetting = QualitySettings.shadowDistance;
        float currentTime = dayNightController.targetTime;
        float frame = capturedFramesNumber - frameUpdatePending;
        float percent = 100.0f / capturedFramesNumber;

        if (!loadingBar.gameObject.activeSelf)
            loadingBar.gameObject.SetActive(true);

        StartCapture();
        Texture2D tmp = CaptureShadowMap(frame);
        FilterDefaultValue(ref tmp, ref pixelArray, capturedFramesNumber * 10f);
        pixelArray.Apply();

        loadingBar.UpdateLoadingBar((frame * percent) / 100.0f);
        if ((frame * percent) > 99.0f)
        {
            startShadowCalc = 2;
            loadingBar.gameObject.SetActive(false);
            MessageHandler.instance.SuccesMessage("shadow_successfull");
            //ConvertColors();
        }

        frameUpdatePending -= 0.1f;
        EndCapture(currentTime, currentShadowSetting);
        ConstructionController.instance.UpdatePlantsSunExposure();
    }

    private void EndCapture(float currentTime, float shadowDistance)
    {
        QualitySettings.shadowDistance = shadowDistance;
        if (pixelArray != null)
            shadowMapTexture.texture = pixelArray;
        sun.RemoveCommandBuffer(LightEvent.AfterShadowMap, cb);
        shadowCamera.enabled = false;
        shadowFilter.SetActive(false);
        dayNightController.InstantSetTimeofDay(currentTime * 24f);
    }

    private void ConvertColors()
    {
        int offset = pixelArray.width;
        float correction = 1 ;
        float value;
        for (int y = 0; y < pixelArray.height; y++)
            for (int x = 0; x < pixelArray.width; x++)
            {
                value = pixelArray.GetPixel(x, y).r;
                if (value != 0f)
                    pixelArray.SetPixel(x, y, gradientScript.GetColor(value * correction));
            }
        pixelArray.Apply();
    }

    private Texture2D CaptureShadowMap(float hourOffset)
    {
        dayNightController.InstantSetTimeofDay(6f + hourOffset);
        shadowCamera.Render();
        RenderTexture.active = shadowmapCopy;
        Texture2D tmp = new Texture2D(shadowmapCopy.width, shadowmapCopy.height);
        tmp.ReadPixels(new Rect(0, 0, shadowmapCopy.width, shadowmapCopy.height), 0, 0);
        tmp.Apply();
        return tmp;
    }

    private void FilterDefaultValue(ref Texture2D tmp, ref Texture2D pixelArray, float stepsNumber)
    {
        float pixelValue;
        float filterValue;
        int offset = tmp.width / 2;

        for (int y = (int)startPoint.y; y < endPoint.y; y++)
        {
            filterValue = tmp.GetPixel(tmp.width - 1, y).r;
            for (int x = (int)startPoint.x; x < endPoint.x; x++)
            {
                pixelValue = tmp.GetPixel(x, y).r;
                if (pixelValue != filterValue)
                {
                    float updatedValue = pixelArray.GetPixel(x - offset, y).r - (pixelValue / stepsNumber);
                    if (updatedValue > max)
                        max = updatedValue;
                    pixelArray.SetPixel(x - offset, y, new Color(updatedValue, updatedValue, updatedValue, 1));
                }
            }
        }
    }

    private void ClearPixelArray()
    {
        pixelArray = null;
        pixelArray = new Texture2D(shadowmapCopy.width / 2, shadowmapCopy.height);
        for (int y = (int)startPoint.y; y < pixelArray.height - (int)startPoint.y; y++)
            for (int x = (int)startPoint.x; x < pixelArray.width - (int)startPoint.x; x++)
                pixelArray.SetPixel(x, y, new Color(0, 0, 0, 0));
    }

    public void SetShadowCalc()
    {
        if (startShadowCalc == 0)
            startShadowCalc = 1;
        else if (startShadowCalc == 2)
            startShadowCalc = 0;
        if (Camera.main.GetComponent<UIController>().shadowMap.IsHiding)
            startShadowCalc = 0;
    }
}
