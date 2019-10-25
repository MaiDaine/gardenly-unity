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
    private float toTextureRatio;
    private CommandBuffer cb;
    private float frameUpdatePending = 0;

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
        Calibrate();
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

                    toTextureRatio = (x - startPoint.x) / planeSize;
                    goto EndCapture;
                }

            EndCapture:
        EndCapture(currentTime, currentShadowSetting);
    }

    private void StartCapture()
    {
        QualitySettings.shadowDistance = 100f;
        shadowCamera.enabled = true;
        shadowFilter.SetActive(true);

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
        Vector2 borders = new Vector2(pixelArray.width - startPoint.x, tmp.height - startPoint.y);

        for (int y = (int)startPoint.y; y < borders.y; y++)
        {
            filterValue = tmp.GetPixel(tmp.width - 1, tmp.height - 1 - y).r;
            for (int x = (int)startPoint.x; x < borders.x; x++)
            {
                pixelValue = tmp.GetPixel(tmp.width - 1 - x, tmp.height - 1 - y).r;
                if (pixelValue != filterValue)
                {
                    float updatedValue = pixelArray.GetPixel(x, y).r - (pixelValue / stepsNumber);

                    //pixelArray.SetPixel(x, y, new Color(updatedValue, updatedValue, updatedValue, 1));
                    pixelArray.SetPixel(x, y, gradientScript.GetColor(GetSunExposure(x, y)));                    
                }
                else
                    pixelArray.SetPixel(x, y, gradientScript.gradient.Evaluate(0));
            }
        }
    }

    private void ClearPixelArray()
    {
        pixelArray = null;
        pixelArray = new Texture2D(shadowmapCopy.width / 2, shadowmapCopy.height);
        for (int y = (int)startPoint.y; y < pixelArray.height - (int)startPoint.y; y++)
            for (int x = (int)startPoint.x; x < pixelArray.width - (int)startPoint.x; x++)
                pixelArray.SetPixel(x, y, new Color(1, 1, 1, 1));
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
