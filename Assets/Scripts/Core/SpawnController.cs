using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance = null;

    public bool loadingData = false;

    //Need one of each for serialization
    public ShapeCreator shapeCreatorRef;
    public FlowerBed flowerBedRef;
    public LineTextHandler lineTextRef;
    public WallHandler wallHandlerRef;

    //Models
    public DefaultStaticElement[] DSElements = new DefaultStaticElement[2];
    public PlantElement plantElement;
    public ModelList plantModels;

    private ShapeCreator shapeCreator;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        shapeCreator = Instantiate(SpawnController.instance.shapeCreatorRef);
        shapeCreator.gameObject.SetActive(false);
    }

    public GhostHandler GetPlantGhost(string type, string name)
    {
        //TODO #74
        PlantData tmp = ReactProxy.instance.GetPlantsData(type, name);
        if (tmp == null)
            return null;

        PlantElement ghost = Instantiate(plantElement);
        ghost.OnPlantDataLoad(tmp);
        return ghost;
    }

    public void SpawnFlowerBed(string json)
    {
        FlowerBed flowerBed;
        flowerBed = Instantiate(flowerBedRef, Vector3.zero, Quaternion.identity);
        flowerBed.DeSerialize(json);
    }

    public void SpawnPlantElement(string json)
    {
        Instantiate(plantElement).DeSerialize(json);
    }

    public void SpawnStaticElement(string json, DefaultStaticElement.StaticElementType type)
    {
        Debug.Log(json);

        switch (type)
        {
            case DefaultStaticElement.StaticElementType.Chair:
                Instantiate(DSElements[0]).DeSerialize(json);
                break;
            case DefaultStaticElement.StaticElementType.Table:
                Instantiate(DSElements[1]).DeSerialize(json);
                break;
            case DefaultStaticElement.StaticElementType.Wall:
                Instantiate(wallHandlerRef).DeSerialize(json);
                break;
            default:
                return;
        }
    }

    public FlowerBed StartNewShape(/*final object*/)
    {
        FlowerBed tmp;

        shapeCreator.gameObject.SetActive(true);
        shapeCreator.Init();
        tmp = Instantiate(flowerBedRef);
        tmp.Init(shapeCreator);
        ConstructionController.instance.SetGhost(shapeCreator);
        ConstructionController.instance.currentState = ConstructionController.ConstructionState.Positioning;
        return tmp;
    }
}