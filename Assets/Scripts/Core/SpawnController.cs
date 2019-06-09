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

    public void SpawnScene(SerializedElement[] data)
    {
        loadingData = true;
        WallHandler wallHandler;
        FlowerBed flowerBed;

        for (int i = 0; i < data.Length; i++)
        {
            switch (data[i].type)
            {
                case SerializationController.ItemType.Wall:
                    wallHandler = Instantiate(wallHandlerRef, Vector3.zero, Quaternion.identity);
                    wallHandler.DeSerialize(data[i].data);
                    break;

                case SerializationController.ItemType.FlowerBed:
                    flowerBed = Instantiate(flowerBedRef, Vector3.zero, Quaternion.identity);
                    flowerBed.DeSerialize(data[i].data);
                    break;

                case SerializationController.ItemType.StaticElement:
                    //DefaultStaticElement staticElement;
                    //DefaultStaticElement.SerializableItem DSEData;
                    //DSEData = JsonUtility.FromJson<DefaultStaticElement.SerializableItem>(data[i].data);
                    //
                    //switch (DSEData.type)
                    //{
                    //    case DefaultStaticElement.StaticElementType.Chair:
                    //        staticElement = Instantiate(DSElements[0], Vector3.zero, Quaternion.identity);
                    //        break;
                    //    case DefaultStaticElement.StaticElementType.Table:
                    //        staticElement = Instantiate(DSElements[1], Vector3.zero, Quaternion.identity);
                    //        break;
                    //    default:
                    //        MessageHandler.instance.ErrorMessage("loading_error");
                    //        return;
                    //}
                    //staticElement.DeSerialize(data[i].data);
                    break;

                case SerializationController.ItemType.Plant:
                    PlantElement element;
                    element = Instantiate(plantElement, Vector3.zero, Quaternion.identity);
                    element.DeSerialize(data[i].data);//TODO LOAD MODEL
                    break;

                default:
                    break;
            }
        }
        loadingData = false;
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