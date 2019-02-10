using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FlowerBedHandler : GhostHandler, ISerializable
{
    public Material material;
    public FlowerBedMesh meshRef;
    public SerializableItem serializableItem;

    public static FlowerBedHandler instance = null;
    
    private List<FlowerBedMesh> meshes = new List<FlowerBedMesh>();
    private FlowerBedMesh currentMesh = null;
    private int meshCount = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SerializationController.instance.AddToList(this);
        SpawnMesh();
        ConstructionController.instance.SetConstructionState(ConstructionController.ConstructionState.Building);
    }

    void OnDestroy()
    {
        SerializationController.instance.RemoveFromList(this);
    }

    private void LateUpdate()
    {
        if (ConstructionController.instance.GetConstructionState() == ConstructionController.ConstructionState.Editing
            && Input.GetKeyDown(KeyCode.Keypad5))
        {
            SpawnMesh();
            ConstructionController.instance.SetConstructionState(ConstructionController.ConstructionState.Building);
        }
    }

    public void CombineMesh()
    {
        int i = 0;
        MeshFilter[] meshFilters = new MeshFilter[meshCount];
        foreach (FlowerBedMesh mesh in meshes)
        {
            meshFilters[i] = mesh.GetComponent<MeshFilter>();
            i++;
        }
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        this.GetComponent<MeshFilter>().mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        this.gameObject.SetActive(true);
        this.GetComponent<MeshRenderer>().material = material;
        this.transform.position = new Vector3(0, 0, 0);
        ConstructionController.instance.SetConstructionState(ConstructionController.ConstructionState.Off);
    }

    public void SpawnMesh()
    {
        if (currentMesh != null)
        {
            currentMesh.DeSelect();
            PlayerController.instance.currentSelection.Clear();
        }
        currentMesh = Instantiate<FlowerBedMesh>(meshRef);
        currentMesh.CustomStart();
        meshes.Add(currentMesh);
        meshCount++;
    }

    public override void StartPreview(Vector3 position)
    {
    }

    public override void Preview(Vector3 position)
    {
        currentMesh.transform.position = position;
    }

    public override void EndPreview()
    {
        currentMesh.gameObject.layer = 10;
        ConstructionController.instance.SetConstructionState(ConstructionController.ConstructionState.Editing);
        currentMesh.Select(ConstructionController.ConstructionState.Editing);
        PlayerController.instance.ForcedSelection(currentMesh.GetComponent<ISelectable>());
    }

    //Serialization
    [Serializable]
    public struct SerializableList
    {
        public Vector3[] positions;
        public Vector2[] points;
    }
    [Serializable]
    public struct SerializableItem
    {
        public Vector3 meshPosition;
        public int meshNumber;
        public SerializableList pointsList;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;
        tmp.type = SerializationController.ItemType.FlowerBed;

        SerializableList tmpList;
        tmpList.points = new Vector2[meshCount * 4];
        tmpList.positions = new Vector3[meshCount];
        int i = 0;
        foreach(FlowerBedMesh mesh in meshes)
        {
            tmpList.points[i] = mesh.GetPoint(0);
            tmpList.points[i + 1] = mesh.GetPoint(1);
            tmpList.points[i + 2] = mesh.GetPoint(2);
            tmpList.points[i + 3] = mesh.GetPoint(3);
            tmpList.positions[i / 4] = mesh.transform.position;
            i += 4;
        }
        serializableItem.meshPosition = this.transform.position;
        serializableItem.meshNumber = meshCount;
        serializableItem.pointsList = tmpList;
        tmp.serializedData = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);

        this.transform.position = serializableItem.meshPosition;
        serializableItem.pointsList.points = new Vector2[serializableItem.meshNumber];
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);
        for (int i = 0; i < serializableItem.meshNumber; i = i + 4)
        {
            currentMesh = Instantiate<FlowerBedMesh>(meshRef);
            currentMesh.transform.position = serializableItem.pointsList.positions[i / 4];
            currentMesh.Init(serializableItem.pointsList.points[i], serializableItem.pointsList.points[i + 1], serializableItem.pointsList.points[i + 2], serializableItem.pointsList.points[i + 3]);
            meshes.Add(currentMesh);
            meshCount++;
        }
        CombineMesh();
    }

}
