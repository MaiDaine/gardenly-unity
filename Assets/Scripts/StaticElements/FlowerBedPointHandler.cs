using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class FlowerBedPointHandler : MonoBehaviour, IInteractible
{
    private int index;
    private FlowerBedMesh owner;
    private Tuple<bool, bool>[] limitRule = new Tuple<bool, bool>[2];
    private FlowerBedPointHandler[] limites = new FlowerBedPointHandler[2];

    public void Init(Vector2 offset, int idx, FlowerBedMesh inOwner)
    {
        this.transform.position = new Vector3(this.transform.position.x + offset.x, 0f, this.transform.position.z + offset.y);
        index = idx;
        owner = inOwner;
    }

    public void SetLimits(FlowerBedPointHandler first, Tuple<bool, bool> firstRule, FlowerBedPointHandler second, Tuple<bool, bool> secondRule)
    {
        limites[0] = first;
        limitRule[0] = firstRule;
        limites[1] = second;
        limitRule[1] = secondRule;
    }

    // IInteractible
    public GameObject GetGameObject() { return this.gameObject; }
    public void Activate()
    {
        this.gameObject.SetActive(true);
    }
    public void DeActivate()
    {
        this.gameObject.SetActive(false);
    }

    public void EndDrag()
    {
        owner.UpdateStraight();
    }

    private void CheckStraight(ref Vector3 mousePosition, int[] indexs, int[] checks)
    {
        return;
        //Vector2 updatedPos = new Vector2(0f, 0f);
        //for (int i = 0; i < 2; i++)
        //{
        //    if (((limitRule[i].Item1 == false && this.transform.position.x <= limites[i].transform.position.x)
        //        || (limitRule[i].Item1 == true && this.transform.position.x >= limites[i].transform.position.x))
        //        && ((limitRule[i].Item2 == false && this.transform.position.z <= limites[i].transform.position.z)
        //        || (limitRule[i].Item2 == true && this.transform.position.z >= limites[i].transform.position.z)))
        //    {
        //        if ((checks[i] > 0 && owner.straights[indexs[i]].CheckPointPosition(new Vector2(mousePosition.x, mousePosition.z), out updatedPos) > 0f)
        //            || (checks[i] < 0 && owner.straights[indexs[i]].CheckPointPosition(new Vector2(mousePosition.x, mousePosition.z), out updatedPos) < 0f))
        //            mousePosition = new Vector3(updatedPos.x, mousePosition.y, updatedPos.y);
        //    }
        //}
        //
        //if ((checks[2] > 0 && owner.straights[indexs[2]].CheckPointPosition(new Vector2(mousePosition.x, mousePosition.z), out updatedPos) >= 0f)
        //   || (checks[2] < 0 && owner.straights[indexs[2]].CheckPointPosition(new Vector2(mousePosition.x, mousePosition.z), out updatedPos) <= 0f))
        //{
        //    Vector2 tmp = new Vector2 (mousePosition.x, mousePosition.z);
        //    Vector2 min = new Vector2(0, 0);
        //    Vector2 max = new Vector2(0, 0);
        //    bool xLine = false;
        //    if (limites[0].transform.position.x < limites[1].transform.position.x)
        //    {
        //        min.x = limites[0].transform.position.x;
        //        max.x = limites[1].transform.position.x;
        //    }
        //    else
        //    {
        //        min.x = limites[1].transform.position.x;
        //        max.x = limites[0].transform.position.x;
        //    }
        //    if (limites[0].transform.position.z < limites[1].transform.position.z)
        //    {
        //        min.y = limites[0].transform.position.z;
        //        max.y = limites[1].transform.position.z;
        //    }
        //    else
        //    {
        //        min.y = limites[1].transform.position.z;
        //        max.y = limites[0].transform.position.z;
        //    }
        //    if ((mousePosition.x < min.x || mousePosition.x > max.x)
        //        && (mousePosition.z < min.y || mousePosition.z > max.y))
        //    {
        //        mousePosition = this.transform.position;
        //        return;
        //    }
        //    else
        //    {
        //        if (mousePosition.x < min.x)
        //            tmp.x = min.x;
        //        else if (mousePosition.x > max.x)
        //            tmp.x = max.x;
        //        if (mousePosition.y < min.y)
        //        {
        //            tmp.y = min.y;
        //            xLine = true;
        //        }
        //        else if (mousePosition.y > max.y)
        //        {
        //            tmp.y = max.y;
        //            xLine = true;
        //        }
        //    }
        //    
        //    if ((checks[2] > 0 && owner.straights[indexs[2]].CheckPointPosition(tmp, out updatedPos, xLine) >= 0f)
        //        || (checks[2] < 0 && owner.straights[indexs[2]].CheckPointPosition(tmp, out updatedPos, xLine) <= 0f))
        //        mousePosition = new Vector3(updatedPos.x, mousePosition.y, updatedPos.y);
        //}
        //if (mousePosition == limites[0].transform.position || mousePosition == limites[1].transform.position)
        //    mousePosition = this.transform.position;
    }

    public void DragClick(Vector3 mousePosition)
    {
        switch (index)
        {
            case 0:
                CheckStraight(ref mousePosition, new int[3] { 3, 2, 5 }, new int[3] { 1, -1, 1 });
                break;
        
            case 1:
                CheckStraight(ref mousePosition, new int[3] { 0, 3, 4 }, new int[3] { 1, 1, -1 });
                break;
        
            case 2:
                CheckStraight(ref mousePosition, new int[3] { 0, 1, 5 }, new int[3] { 1, -1, -1 });
                break;
        
            case 3:
                CheckStraight(ref mousePosition, new int[3] { 1, 2, 4 }, new int[3] { -1, -1, 1 });
                break;
        }
        this.transform.position = new Vector3(mousePosition.x, 0f, mousePosition.z);
        this.GetComponentInParent<FlowerBedMesh>().UpdatePointPosition(this.transform.position, index);
    }
}
*/