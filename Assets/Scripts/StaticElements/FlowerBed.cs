using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class FlowerBed : MonoBehaviour ,ISelectable, ISerializable
{
    public Material material;
    public string name = "PLACEHOLDER";
    public string soilType = "PLACEHOLDER";

    private ShapeCreator shapeCreator;
    private Vector2[] vertices;
    private List<FlowerBedElement> flowerBedElements = new List<FlowerBedElement>();

    public void Init(ShapeCreator shapeCreator)
    {
        this.shapeCreator = shapeCreator;
        shapeCreator.eventShapeConstructionFinished.AddListener(OnShapeFinished);
    }

    private void OnShapeFinished()
    {
        this.shapeCreator.eventShapeConstructionFinished.RemoveListener(OnShapeFinished);

        this.vertices = new Vector2[this.shapeCreator.points.Count];
        int i = 0;

        foreach (ShapePoint point in this.shapeCreator.points)
        {
            this.vertices[i] = new Vector2(point.transform.position.x, point.transform.position.z);
            i++;
        }
        this.shapeCreator.SelfClear();
        this.shapeCreator.gameObject.SetActive(false);
        CreateMesh();
    }

    private void CreateMesh(bool isFixed = false)
    {
        MeshHandler meshHandler = this.gameObject.AddComponent<MeshHandler>();
        this.GetComponent<MeshFilter>().mesh = meshHandler.Init(this.vertices);
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        this.GetComponent<MeshRenderer>().material = this.material;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
        if (!isFixed && mesh.triangles.Length + 1 < 3 * (vertices.Length - 2))
        {
            System.Array.Reverse(vertices);
            Destroy(meshHandler);
            Destroy(mesh);
            CreateMesh(true);
            return;
        }
        Destroy(meshHandler);
        this.gameObject.layer = 10;
        ConstructionController.instance.currentState = ConstructionController.ConstructionState.Editing;
        ConstructionController.instance.flowerbedCount++;
        ConstructionController.instance.currentState = ConstructionController.ConstructionState.Off;//TODO UI with UI button
    }

    public void AddElement(FlowerBedElement element) { this.flowerBedElements.Add(element); }

    private void OnEnable()
    {
        SerializationController.instance.AddToList(this);
    }

    private void OnDisable()
    {
        ConstructionController.instance.flowerbedCount--;
        SerializationController.instance.RemoveFromList(this);
    }

    //ISelectable
    public GameObject GetGameObject() { return this.gameObject; }
    public void Select(ConstructionController.ConstructionState state)
    {
        if (state == ConstructionController.ConstructionState.Off || state == ConstructionController.ConstructionState.Editing)
        {
            UIController controller = Camera.main.GetComponent<UIController>();
            controller.SpawnFlowerBedMenu(this);
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
      /*  UIController uIController = Camera.main.GetComponent<UIController>();
        if (uIController.GetFlowerBedMenuScript() != null)
            uIController.GetFlowerBedMenuScript().DestroyMenu();*/
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
    }
}
