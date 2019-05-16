using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FlowerBed : MonoBehaviour, ISelectable, ISerializable
{
    public Material material;
    public new string name = "PLACEHOLDER";
    public string soilType = "PLACEHOLDER";
    public Vector2[] vertices;

    private ShapeCreator shapeCreator;
    private List<FlowerBedElement> flowerBedElements = new List<FlowerBedElement>();

    public void Init(ShapeCreator shapeCreator)
    {
        this.shapeCreator = shapeCreator;
        this.shapeCreator.flowerBed = this;
        this.GetComponent<MeshRenderer>().enabled = false;
        shapeCreator.eventShapeConstructionFinished.AddListener(FinalActivation);
    }

    public void ActivationCancel()
    {
        this.GetComponent<MeshCollider>().sharedMesh = null;
        this.GetComponent<MeshCollider>().enabled = false;
        Destroy(this.GetComponent<MeshCollider>());
        this.GetComponent<MeshFilter>().mesh = null;
        Destroy(this.GetComponent<MeshFilter>().mesh);
        Destroy(this.GetComponent<MeshHandler>());
    }

    public void FinalActivation()
    {
        this.shapeCreator.SelfClear();
        this.shapeCreator.gameObject.SetActive(false);
        this.shapeCreator.eventShapeConstructionFinished.RemoveListener(FinalActivation);
        Setup();
    }

    public void OnShapeFinished()
    {
        this.vertices = new Vector2[this.shapeCreator.points.Count];
        int i = 0;

        foreach (ShapePoint point in this.shapeCreator.points)
        {
            this.vertices[i] = new Vector2(point.transform.position.x, point.transform.position.z);
            i++;
        }
        CreateMesh();
    }

    private void CreateMesh(bool isFixed = false)
    {
        MeshHandler meshHandler = this.gameObject.AddComponent<MeshHandler>();
        this.GetComponent<MeshFilter>().mesh = meshHandler.Init(this.vertices);
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        this.gameObject.AddComponent<MeshCollider>();
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
        this.GetComponent<MeshCollider>().enabled = true;
        if (!isFixed && mesh.triangles.Length + 1 < 3 * (vertices.Length - 2))
        {
            System.Array.Reverse(vertices);
            Destroy(meshHandler);
            Destroy(mesh);
            CreateMesh(true);
            Debug.Log("CREATE MESH");
            Camera.main.GetComponentInChildren<UIController>().ResetButton();
        }
    }

    private void Setup()
    {
        this.GetComponent<MeshRenderer>().material = this.material;
        this.GetComponent<MeshRenderer>().enabled = true;
        Destroy(this.GetComponent<MeshHandler>());
        this.gameObject.layer = 10;
        ConstructionController.instance.currentState = ConstructionController.ConstructionState.Editing;
        ConstructionController.instance.currentState = ConstructionController.ConstructionState.Off;//TODO UI with UI button
        PlayerController.instance.currentSelection = this.gameObject.GetComponent<ISelectable>();
        ConstructionController.instance.flowerBeds.Add(this);
    }

    public void AddElement(FlowerBedElement element) { this.flowerBedElements.Add(element); }

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


    public void SetTutorial()
    {
        if (TutoBoxScript.isOn)
        {
            UIController controller = Camera.main.GetComponent<UIController>();
            controller.tutoView.GetComponentInChildren<TutoBoxScript>().SetTutorial("");
        }
    }
    //ISelectable
    public GameObject GetGameObject() { return this.gameObject; }
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

    public Vector2[] GetVertices() { return this.vertices; }

    public void DeSelect()
    {
        UIController uIController = Camera.main.GetComponent<UIController>();
        if (uIController.GetFlowerBedMenuScript() != null)
        {
            uIController.flowerBedDataPanel.Hide();
            uIController.tutoView.Hide();
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
        public FlowerBedElement.SerializedFBE[] elements;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;
        SerializedFlowerBed data;
        int i = 0;
        
        data.name = this.name;
        data.soilType = this.soilType;
        data.points = vertices;
        data.elements = new FlowerBedElement.SerializedFBE[flowerBedElements.Count];

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
        this.name = tmp.name;
        this.soilType = tmp.soilType;
        this.vertices = tmp.points;
        foreach (FlowerBedElement.SerializedFBE elem in tmp.elements)
            this.flowerBedElements.Add(SpawnController.instance.SpawnFlowerBedElement(elem));
        CreateMesh();
        Setup();
    }
}
