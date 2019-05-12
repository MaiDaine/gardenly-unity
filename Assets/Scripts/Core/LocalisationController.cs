using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalisationController : MonoBehaviour
{
    public static LocalisationController instance = null;

    private SimpleJSON.JSONNode locals;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public void Init(string local = "FR")//FIXME : change to EN ?
    {
        string json = ((TextAsset )Resources.Load("Locals_" + local, typeof(TextAsset))).ToString();
        locals = JSONObject.Parse(json);
    }

    public string GetText(string category, string name)
    {
        return locals[category][name];
    }

    public string GetText(string category, string subCategory, string name)
    {
        return locals[category][subCategory][name];
    }
}
