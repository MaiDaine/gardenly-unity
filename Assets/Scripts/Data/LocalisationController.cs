using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LocalisationController : MonoBehaviour
{
    public static LocalisationController instance = null;

    private SimpleJSON.JSONNode locales;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public void Init(string locale = "FR")
    {
        if (Application.isEditor)
        {
            string json = ((TextAsset)Resources.Load("Locales_" + locale, typeof(TextAsset))).ToString();
            locales = JSONObject.Parse(json);
        }
        else
            StartCoroutine(GetLocal(locale));
    }

    public string GetText(string category, string name)
    {
        if (locales == null)
            return category + ":" + name;
        return locales[category][name];
    }

    public string GetText(string category, string subCategory, string name)
    {
        if (locales == null)
            return category + ":" + name;
        return locales[category][subCategory][name];
    }

    private IEnumerator GetLocal(string locale)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://s3.gardenly.app/unity/Locales/Locales_" + locale + ".json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.Log(www.error);
        else
            locales = JSONObject.Parse(www.downloadHandler.text);
    }
}