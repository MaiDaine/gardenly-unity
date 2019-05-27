using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance = null;

    public int staticElementsCount;
    public int flowerBedElementsCount;

    //need one of each for serialization
    public WallHandler WallHandlerRef;
    public FlowerBed flowerBedRef;
    //public FlowerBedHandler FlowerBedHandlerRef;
    public ShapeCreator ShapeCreator;
    public DefaultStaticElement[] DSElements = new DefaultStaticElement[2];
    public PlantElement[] plantElements = new PlantElement[2];
    public FlowerBedElement[] flowerBedElements = new FlowerBedElement[2];
    public LineTextHandler lineText;
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
        shapeCreator = Instantiate(SpawnController.instance.ShapeCreator);
        shapeCreator.gameObject.SetActive(false);
    }

    public GhostHandler GetPlantGhost(string type, string name)
    {
        //TODO #74
        // Instantiate(ghostRef, Vector3.zero, Quaternion.identity);
        PlantData tmp = ReactProxy.instance.GetPlantsData(type, name);
        if (tmp == null)
            return null;
        if (tmp.model != -1 && tmp.model < plantModels.datas.Count)
        {
            FlowerBedElement elem = plantModels.datas[tmp.model].CreateElement(flowerBedElements[1], tmp.plantColor);
            elem.subID = tmp.plantID;
            elem.OnPlantDataLoad(tmp);
            return elem;
        }
        GhostHandler ghost;
        switch (type)
        {
            case "Arbre":
                ghost = plantElements[0];
                ((PlantElement)ghost).OnPlantDataLoad(tmp);
                break;
            case "Arbuste":
                ghost = plantElements[1];
                ((PlantElement)ghost).OnPlantDataLoad(tmp);
                break;
            case "Legume":
                ghost = flowerBedElements[0];
                ((FlowerBedElement)ghost).OnPlantDataLoad(tmp);
                break;
            case "Fleur":
                ghost = flowerBedElements[0];
                ((FlowerBedElement)ghost).OnPlantDataLoad(tmp);
                break;
            default:
                return null;
        }
        return ghost;
    }

    public void SpawnScene(SerializationData[] data)
    {
        WallHandler wallHandler;
        FlowerBed flowerBed;

        for (int i = 0; i < data.Length; i++)
        {
            switch (data[i].type)
            {
                case SerializationController.ItemType.WallHandler:
                    wallHandler = Instantiate(WallHandlerRef, Vector3.zero, Quaternion.identity);
                    wallHandler.DeSerialize(data[i].data);
                    break;

                case SerializationController.ItemType.FlowerBed:
                    flowerBed = Instantiate(flowerBedRef, Vector3.zero, Quaternion.identity);
                    flowerBed.DeSerialize(data[i].data);
                    break;

                case SerializationController.ItemType.DefaultStaticElement:
                    DefaultStaticElement staticElement;
                    DefaultStaticElement.SerializableItem DSESubType;
                    DSESubType = JsonUtility.FromJson<DefaultStaticElement.SerializableItem>(data[i].data);
                    switch (DSESubType.subType)
                    {
                        case DefaultStaticElement.StaticElementType.Chair:
                            staticElement = Instantiate(DSElements[0], Vector3.zero, Quaternion.identity);
                            break;
                        case DefaultStaticElement.StaticElementType.Table:
                            staticElement = Instantiate(DSElements[1], Vector3.zero, Quaternion.identity);
                            break;
                        default:
                            MessageHandler.instance.ErrorMessage("loading_error");
                            return;
                    }
                    staticElement.DeSerialize(data[i].data);
                    break;

                default:
                    break;
            }
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

    public FlowerBedElement SpawnFlowerBedElement(FlowerBedElement.SerializedFBE elem)
    {
        //TODO MODEL
        FlowerBedElement tmp = Instantiate(flowerBedElements[1], Vector3.zero, Quaternion.identity);
        tmp.InnerDeSerialize(elem);
        return tmp;
    }
}
