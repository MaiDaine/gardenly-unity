using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBedHandler : GhostHandler
{
    public FlowerBedMesh meshRef;

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
        meshes.Add(currentMesh);
        meshCount++;
    }
}
