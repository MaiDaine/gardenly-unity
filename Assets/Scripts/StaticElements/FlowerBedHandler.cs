using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBedHandler : GhostHandler, ISerializable
{
    public FlowerBedMesh meshRef;
    public SerializableItem serializableItem;
    
    private List<FlowerBedMesh> meshes = new List<FlowerBedMesh>();
    private FlowerBedMesh currentMesh = null;
    private int meshCount = 0;

    void Start()
    {
        SpawnMesh();
        ConstructionController.instance.SetConstructionState(ConstructionController.ConstructionState.Building);
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
        //TODO;
    }

    public override void StartPreview(Vector3 position)
    {
        currentMesh.transform.position = position;
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

    private void SpawnMesh()
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
    }

}
