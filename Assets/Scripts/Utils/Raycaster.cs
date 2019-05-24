using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    private Camera currentCamera;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private Vector3 lastCastPos = new Vector3(0, 0, 0);

    private void Start()
    {
        currentCamera = this.GetComponent<Camera>();
    }

    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, int layer = PlayerController.layerMaskStatic)
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer) && hit.collider.tag != "Invalid")
        {
            pos = ray.GetPoint(rayDistance);
            lastCastPos = pos;
            return true;
        }
        pos = lastCastPos;
        hit = new RaycastHit();
        return false;
    }

    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, out ISnapable snapable, int layer = PlayerController.layerMaskStatic)
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer, QueryTriggerInteraction.Ignore)
           && hit.collider.tag != "Invalid")
        {
            pos = ray.GetPoint(rayDistance);
            lastCastPos = pos;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            return true;
        }
        pos = lastCastPos;
        hit = new RaycastHit();
        snapable = null;
        return false;
    }
}
