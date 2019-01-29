using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBedMesh : MonoBehaviour, ISelectable
{
    public Material material;
    public FlowerBedPointHandler point;
    public int qualitySettings = 0;
    public Straight[] straights = new Straight[6];
    public FlowerBedPointHandler[] points = new FlowerBedPointHandler[4];

    private MeshHandler meshHandler;
    private MeshRenderer mRenderer;
    private MeshFilter mFilter;
    private MeshCollider mCollider; 
    private Vector2[] vertices2D = new Vector2[4];
    private const float maxDistance = 10f;

    public void CustomStart()
    {
        Init(new Vector2(-1f, 1f), new Vector2(1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f));
    }

    public void Init(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Vector2 pos = new Vector2(this.transform.position.x, this.transform.position.z);
        CreatePoint(p1, 0);
        CreatePoint(p2, 1);
        CreatePoint(p3, 2);
        CreatePoint(p4, 3);
        points[0].SetLimits(points[3], new System.Tuple<bool, bool>(false, false), points[1], new System.Tuple<bool, bool>(true, true));
        points[1].SetLimits(points[0], new System.Tuple<bool, bool>(false, true), points[2], new System.Tuple<bool, bool>(true, false));
        points[2].SetLimits(points[3], new System.Tuple<bool, bool>(false, false), points[1], new System.Tuple<bool, bool>(true, true));
        points[3].SetLimits(points[0], new System.Tuple<bool, bool>(false, true), points[2], new System.Tuple<bool, bool>(true, false));
        for (int i = 0; i < 6; i++)
            straights[i] = gameObject.AddComponent<Straight>();
        straights[0].UpdateEquation(GetRealPosition(3), GetRealPosition(0));
        straights[1].UpdateEquation(GetRealPosition(1), GetRealPosition(0));
        straights[2].UpdateEquation(GetRealPosition(2), GetRealPosition(1));
        straights[3].UpdateEquation(GetRealPosition(3), GetRealPosition(2));
        straights[4].UpdateEquation(GetRealPosition(0), GetRealPosition(2));
        straights[5].UpdateEquation(GetRealPosition(1), GetRealPosition(3));
        meshHandler = gameObject.AddComponent<MeshHandler>();
        mRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        mFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        mRenderer.material = material;
        mCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        UpdateMeshVisual();
    }

    public void UpdateStraight()
    {
        straights[0].UpdateEquation(GetRealPosition(3), GetRealPosition(0));
        straights[1].UpdateEquation(GetRealPosition(1), GetRealPosition(0));
        straights[2].UpdateEquation(GetRealPosition(2), GetRealPosition(1));
        straights[3].UpdateEquation(GetRealPosition(3), GetRealPosition(2));
        straights[4].UpdateEquation(GetRealPosition(0), GetRealPosition(2));
        straights[5].UpdateEquation(GetRealPosition(1), GetRealPosition(3));
    }

    public Vector2 GetPoint(int index)
    {
        return (new Vector2(this.transform.position.x - points[index].transform.position.x, this.transform.position.z - points[index].transform.position.z));
    }

    private Vector2 GetRealPosition(int index)
    {
        return (new Vector2(points[index].transform.position.x, points[index].transform.position.z));
    }

    private void CreatePoint(Vector2 position, int index)
    {
        vertices2D[index] = position;
        points[index] = Instantiate(point, this.transform);
        points[index].Init(position, index, this);
    }

    public void UpdatePointPosition(Vector3 pos, int index)
    {
        Vector2 tmp = new Vector2(pos.x - this.transform.position.x, pos.z - this.transform.position.z);
        vertices2D[index].x = pos.x - this.transform.position.x;
        vertices2D[index].y = pos.z - this.transform.position.z;
        UpdateMeshVisual();
    }

    public void UpdateMeshVisual()
    {
        mFilter.mesh = meshHandler.Init(vertices2D, qualitySettings);
        mCollider.sharedMesh = mFilter.mesh;
    }

    //ISelectable
    public GameObject GetGameObject() { return this.gameObject; }
    public void Select(ConstructionController.ConstructionState state)
    {
        if (state == ConstructionController.ConstructionState.Editing)
            for (int i = 0; i < 4; i++)
                points[i].Activate();
        //TODO else overlay
    }
    public List<ISelectable> SelectWithNeighbor() { return new List<ISelectable>(); }
    public void DeSelect()
    {
        for (int i = 0; i < 4; i++)
            points[i].DeActivate();
    }
    public void AddNeighbor(ISelectable item) { }
    public void RemoveFromNeighbor(ISelectable item) { }
}
