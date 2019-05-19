using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        if (Application.isEditor)
        {
            string json = ((TextAsset)Resources.Load("Locals_" + local, typeof(TextAsset))).ToString();
            locals = JSONObject.Parse(json);
        }
        else
            StartCoroutine(GetLocal(local));
    }

    public string GetText(string category, string name)
    {
        return locals[category][name];
    }

    public string GetText(string category, string subCategory, string name)
    {
        return locals[category][subCategory][name];
    }

    private IEnumerator GetLocal(string local)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://s3.greefine.ovh/unity/Locals/Locals_" + local + ".json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.Log(www.error);
        else
            locals = JSONObject.Parse(www.downloadHandler.text);
    }
}
