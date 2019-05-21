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
    public DefaultStaticElement[] DSElements = new DefaultStaticElement[4];
    public FlowerBedElement[] FBElements = new FlowerBedElement[2];
    public LineTextHandler lineText;
    public ModelList plantModels;

    private ShapeCreator shapeCreator;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        this.shapeCreator = Instantiate(SpawnController.instance.ShapeCreator);
        this.shapeCreator.gameObject.SetActive(false);
    }

    public GhostHandler GetPlantGhost(string type, string name)
    {
        PlantData tmp = ReactProxy.instance.GetPlantsData(type, name);
        if (tmp == null)
            return null;
        if (tmp.model != -1)
        {
            FlowerBedElement elem = plantModels.datas[tmp.model].CreateElement(FBElements[0], tmp.plantColor);
            elem.SetData(tmp);
            elem.subID = tmp.plantID;
            return elem;
        }
        switch (type)
        {
            case "Arbre":
                DSElements[2].SetData(tmp);
                return DSElements[2];
            case "Arbuste":
                DSElements[3].SetData(tmp);
                return DSElements[3];
            case "Legume":
                FBElements[1].SetData(tmp);
                return FBElements[1];
            case "Fleur":
                FBElements[1].SetData(tmp);
                return FBElements[1];
            default:
                return null;
        }
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
                        case DefaultStaticElement.StaticElementType.Tree:
                            staticElement = Instantiate(DSElements[2], Vector3.zero, Quaternion.identity);
                            break;
                        case DefaultStaticElement.StaticElementType.Tree2:
                            staticElement = Instantiate(DSElements[3], Vector3.zero, Quaternion.identity);
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

    public FlowerBed SpawnFlowerBed()
    {
        FlowerBed tmp;

        this.shapeCreator.gameObject.SetActive(true);
        this.shapeCreator.Init();
        tmp = Instantiate(this.flowerBedRef);
        tmp.Init(this.shapeCreator);
        ConstructionController.instance.SetGhost(this.shapeCreator);
        return tmp;
    }

    public FlowerBedElement SpawnFlowerBedElement(FlowerBedElement.SerializedFBE elem)
    {
        //TODO MODEL
        FlowerBedElement tmp = Instantiate(FBElements[0], Vector3.zero, Quaternion.identity);
        tmp.InnerDeSerialize(elem);
        return tmp;
    }
}