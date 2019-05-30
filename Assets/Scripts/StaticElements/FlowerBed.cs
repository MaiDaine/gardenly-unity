using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FlowerBed : MonoBehaviour, ISelectable, ISerializable
{
    public Material material;
    public string flowerBedName = "";//TODO FBDATA(waiting db schema update)
    public string soilType = "";
    public Vector2[] vertices;

    private ShapeCreator shapeCreator;
    private List<FlowerBedElement> flowerBedElements = new List<FlowerBedElement>();

    public void Init(ShapeCreator shapeCreator)
    {
        this.shapeCreator = shapeCreator;
        this.shapeCreator.flowerBed = this;
        GetComponent<MeshRenderer>().enabled = false;
        shapeCreator.eventShapeConstructionFinished.AddListener(FinalActivation);
    }

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
        foreach (FlowerBedElement elem in flowerBedElements)
            elem.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        ConstructionController.instance.flowerBeds.Remove(this);
        SerializationController.instance.RemoveFromList(this);
        foreach (FlowerBedElement elem in flowerBedElements)
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

    public void AddElement(FlowerBedElement element) { flowerBedElements.Add(element); }

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
    public struct SerializedFlowerBed
    {   
        public string name;
        public string soilType;
        public Vector2[] points;
        public PlantElement.SerializedPlantElement[] elements;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;
        SerializedFlowerBed data;
        int i = 0;
        
        data.name = flowerBedName;
        data.soilType = soilType;
        data.points = vertices;
        data.elements = new PlantElement.SerializedPlantElement[flowerBedElements.Count];

        foreach (FlowerBedElement elem in flowerBedElements)
        {
            if (elem == null)
                Debug.Log(i);
            data.elements[i] = elem.InnerSerialize();
            i++;
        }
        
        tmp.type = SerializationController.ItemType.FlowerBed;
        tmp.data = JsonUtility.ToJson(data);
        return tmp;
    }

    public void DeSerialize(string json)
    {
        SerializedFlowerBed tmp = JsonUtility.FromJson<SerializedFlowerBed>(json);
        flowerBedName = tmp.name;
        soilType = tmp.soilType;
        vertices = tmp.points;
        foreach (PlantElement.SerializedPlantElement elem in tmp.elements)
            flowerBedElements.Add(SpawnController.instance.SpawnFlowerBedElement(elem));
        CreateMesh();
        Setup();
    }
}
