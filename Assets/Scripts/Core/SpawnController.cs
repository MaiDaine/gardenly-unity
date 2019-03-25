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
    public FlowerBedHandler FlowerBedHandlerRef;
    public ShapeCreator ShapeCreator;
    public DefaultStaticElement[] DSElements = new DefaultStaticElement[4];
    public FlowerBedElement[] FBElements = new FlowerBedElement[1];

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
        shapeCreator = Instantiate(SpawnController.instance.ShapeCreator);
        shapeCreator.gameObject.SetActive(false);
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
                            ErrorHandler.instance.ErrorMessage("Error while loading, please reload the page");
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

        shapeCreator.gameObject.SetActive(true);
        shapeCreator.Init();
        tmp = Instantiate(flowerBedRef);
        tmp.Init(shapeCreator);
        ConstructionController.instance.SetGhost(shapeCreator);
        return tmp;
    }

    public FlowerBedElement SpawnFlowerBedElement(FlowerBedElement.SerializedFBE elem)
    {
        FlowerBedElement tmp = null;
        switch (elem.subID)
        {
            case FlowerBedElement.FlowerBedElementType.Flower01:
                tmp = Instantiate(FBElements[0], Vector3.zero, Quaternion.identity);
                tmp.InnerDeSerialize(elem);
                break;
            default :
                ErrorHandler.instance.ErrorMessage("Error while loading, please reload the page");
                return null;
        }
        return tmp;
    }
}