using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public bool state = false;

    private DynamicElemHandler ghost;
    private Camera player;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(state);
        player = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void    ChangeState()
    {
        gameObject.SetActive(!state);
        state = !state;
    }

    public void SetGhostRef(DynamicElemHandler ghostRef)
    {
        ghost = ghostRef;
    }

    public DynamicElemHandler GetGhost()
    {
        return ghost;
    }

    public void DestroyMenu()
    {
        Destroy(this.gameObject);
    }

    public void MoveGhost()
    {
        ConstructionController constructionController = player.GetComponent<ConstructionController>();
        constructionController.SetGhost(ghost);
    }
}
