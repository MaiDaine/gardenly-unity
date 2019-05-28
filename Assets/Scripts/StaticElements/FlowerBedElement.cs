using UnityEngine;

public class FlowerBedElement : PlantElement
{
    private void Start()
    {
        if (ConstructionController.instance.flowerBeds.Count < 1)
        {
            MessageHandler.instance.ErrorMessage("flower_bed", "no_flowerbed");
            ConstructionController.instance.Cancel();
        }
    }

    //Serialization
    public override SerializationData Serialize()
    {
        SerializationData tmp = new SerializationData();

        tmp.type = SerializationController.ItemType.FlowerBedElement;
        tmp.data = JsonUtility.ToJson(InnerSerialize());
        return tmp;
    }
}
