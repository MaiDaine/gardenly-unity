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
    public FlowerBedHandler FlowerBedHandlerRef;
    public DefaultStaticElement[] DSElements = new DefaultStaticElement[4];
    public FlowerBedElement[] FBElements = new FlowerBedElement[1];

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public void SpawnScene(SerializationData[] data)
    {
        WallHandler wallHandler;
        FlowerBedHandler flowerBedHandler;

        for (int i = 0; i < data.Length; i++)
        {
            switch (data[i].type)
            {
                case SerializationController.ItemType.WallHandler:
                    wallHandler = Instantiate(WallHandlerRef, Vector3.zero, Quaternion.identity);
                    wallHandler.DeSerialize(data[i].serializedData);
                    break;

                case SerializationController.ItemType.FlowerBed:
                    flowerBedHandler = Instantiate(FlowerBedHandlerRef, Vector3.zero, Quaternion.identity);
                    flowerBedHandler.DeSerialize(data[i].serializedData);
                    break;

                case SerializationController.ItemType.DefaultStaticElement:
                    DefaultStaticElement staticElement;
                    DefaultStaticElement.SerializableItem DSESubType;
                    DSESubType = JsonUtility.FromJson<DefaultStaticElement.SerializableItem>(data[i].serializedData);
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
                    staticElement.DeSerialize(data[i].serializedData);
                    break;

                default:
                    break;
            }
        }
    }

    public FlowerBedElement SpawnFlowerBedElement(SerializationData data)
    {
        FlowerBedElement elem = null;
        FlowerBedElement.SerializableItem tmp = JsonUtility.FromJson<FlowerBedElement.SerializableItem>(data.serializedData);
        switch (tmp.subType)
        {
            case FlowerBedElement.FlowerBedElementType.Flower01:
                elem = Instantiate(FBElements[0], Vector3.zero, Quaternion.identity);
                elem.DeSerialize(data.serializedData);
                break;
            //default :
                //ErrorHandler.instance.ErrorMessage("Error while loading, please reload the page");
                //return null;
        }
        return elem;
    }
}