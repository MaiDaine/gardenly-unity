using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FlowerBed : MonoBehaviour, ISelectable, ISerializable
{
    public Material material;
    public string flowerBedName = "";//TODO FBDATA(waiting db schema update)
    public string groundType;//TODO FBDATA(waiting db schema update)
    public Vector2[] vertices;
    public SerializableItemData serializableItem;

    private ShapeCreator shapeCreator;
    private List<PlantElement> flowerBedElements = new List<PlantElement>();
    private SerializedElement serializedElement;
    private bool initFromSerialization = false;

    private void Start()
    {
        if (initFromSerialization)
            initFromSerialization = false;
        else
            serializedElement.key = SerializationController.GetCurrentDate();
    }

    public void Init(ShapeCreator shapeCreator)
    {
        this.shapeCreator = shapeCreator;
        this.shapeCreator.flowerBed = this;
        GetComponent<MeshRenderer>().enabled = false;
        shapeCreator.eventShapeConstructionFinished.AddListener(FinalActivation);
    }

    public int GetKey() { return serializedElement.key; }

    //Activation
    public void ActivationCancel()
    {
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().enabled = false;
        Destroy(GetComponent<MeshCollider>());
        GetComponent<MeshFilter>().mesh = null;
        Destroy(GetComponent<MeshFilter>().mesh);
        Destroy(GetComponent<MeshHandler>());
    }

    public void FinalActivation()
    {
        shapeCreator.SelfClear();
        shapeCreator.gameObject.SetActive(false);
        shapeCreator.eventShapeConstructionFinished.RemoveListener(FinalActivation);
        Setup();
        groundType = ReactProxy.instance.externalData.groundTypes[0].Key;
        flowerBedName = LocalisationController.instance.GetText("names", "flowerbed") + " " + ConstructionController.instance.flowerBeds.Count;
    }

    public void OnShapeFinished()
    {
        vertices = new Vector2[shapeCreator.points.Count];
        int i = 0;

        foreach (ShapePoint point in shapeCreator.points)
        {
            vertices[i] = new Vector2(point.transform.position.x, point.transform.position.z);
            i++;
        }
        CreateMesh();
    }

    private void OnEnable()
    {
        SerializationController.instance.AddToList(this);
        foreach (PlantElement elem in flowerBedElements)
            elem.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        ConstructionController.instance.flowerBeds.Remove(this);
        SerializationController.instance.RemoveFromList(this);
        foreach (PlantElement elem in flowerBedElements)
            if (elem != null)
                elem.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        for (int i = flowerBedElements.Count - 1; i > 0; i--)
            Destroy(flowerBedElements[i]);
    }

    private void Setup()
    {
        GetComponent<MeshRenderer>().material = material;
        GetComponent<MeshRenderer>().enabled = true;
        Destroy(GetComponent<MeshHandler>());
        gameObject.layer = 10;
        ConstructionController.instance.currentState = ConstructionController.ConstructionState.Editing;
        ConstructionController.instance.currentState = ConstructionController.ConstructionState.Off;//TODO UI with UI button
        PlayerController.instance.currentSelection = gameObject.GetComponent<ISelectable>();
        ConstructionController.instance.flowerBeds.Add(this);
        flowerBedName = LocalisationController.instance.GetText("names", "flowerbed") + " " + ConstructionController.instance.flowerBeds.Count;
        Camera.main.GetComponentInChildren<UIController>().ResetButton();
    }

    private void CreateMesh(bool isFixed = false)
    {
        MeshHandler meshHandler = gameObject.AddComponent<MeshHandler>();
        GetComponent<MeshFilter>().mesh = meshHandler.Init(vertices);
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        gameObject.AddComponent<MeshCollider>();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshCollider>().enabled = true;
    }

    public void AddElement(PlantElement element) { flowerBedElements.Add(element); }

    //ISelectable
    public GameObject GetGameObject() { return gameObject; }
    public void Select(ConstructionController.ConstructionState state)
    {
        if (state == ConstructionController.ConstructionState.Off)// || state == ConstructionController.ConstructionState.Editing)
        {
            UIController controller = Camera.main.GetComponent<UIController>();
            controller.SetFlowerBedDataPanel(this);
        }
    }
    public List<ISelectable> SelectWithNeighbor()
    {
        List<ISelectable> tmp = new List<ISelectable>();

        tmp.Add(this);
        return tmp;
    }

    public Vector2[] GetVertices() { return vertices; }

    public void DeSelect()
    {
        UIController uIController = Camera.main.GetComponent<UIController>();
        if (uIController.GetFlowerBedMenuScript() != null)
        {
            uIController.flowerBedDataPanel.Hide();
            uIController.plantsViews[5].Hide();
        }
    }

    public void AddNeighbor(ISelectable item) { }
    public void RemoveFromNeighbor(ISelectable item) { }

    //Serialization
    [Serializable]
    public struct SerializedElement
    {
        public SerializationController.ItemType type;
        public int key;
        public string name;
        public string ground_type_id;
        public string data;
    }

    [Serializable]
    public struct SerializableItemData
    {
        public Vector2[] points;
    }

    public string Serialize()
    {

        SerializableItemData serializableItemData;

        serializableItemData.points = vertices;

        serializedElement.type = SerializationController.ItemType.FlowerBed;
        serializedElement.data = JsonUtility.ToJson(serializableItemData);

        SimpleJSON.JSONObject json = new SimpleJSON.JSONObject();

        json["type"] = serializedElement.type.ToString();
        json["key"] = serializedElement.key;
        json["name"] = flowerBedName;
        json["ground_type_id"] = GetGroundTypeFromName(groundType);
        json["data"] = serializedElement.data;

        return (json.ToString());
    }

    public void DeSerialize(string json)
    {
        //SerializableItemData tmp = JsonUtility.FromJson<SerializableItemData>(json);
        //flowerBedName = tmp.name;
        //groundType = GetGroundNameFromType(tmp.groundType);
        //if (groundType == null)
        //{
        //    ReactProxy.instance.externalData.callbackGround.Add(UpdateGroundTypeName);
        //    groundType = tmp.name;
        //}
        //vertices = tmp.points;
        //CreateMesh();
        //Setup();
    }

    private void UpdateGroundTypeName()
    {
        groundType = GetGroundNameFromType(groundType);
    }

    private string GetGroundTypeFromName(string name)
    {
        foreach (KeyValuePair<string, string> elem in ReactProxy.instance.externalData.groundTypes)
            if (elem.Key == name)
                return elem.Value;
        return null;
    }

    private string GetGroundNameFromType(string type)
    {
        foreach (KeyValuePair<string, string> elem in ReactProxy.instance.externalData.groundTypes)
            if (elem.Value == name)
                return elem.Key;
        return null;
    }
}