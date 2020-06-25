using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class Manager2 : MonoBehaviour
{

    private Map2 map2;
    public TextMeshProUGUI title;
    public GameObject Header, Row;
    public GameObject Panel;
    private int inX = 255, inXnow, inY=55, inYnow;
    private List<GameObject> toDelete;

    private void Awake()
    {
        toDelete = new List<GameObject>();
        LoadJson();
    }

    public void LoadAgain()
    {
        
        foreach (GameObject item in toDelete)
        {
            GameObject.DestroyImmediate(item);
        }

        toDelete = new List<GameObject>();
        inXnow = 0;
        inYnow = 0;

        LoadJson();
    }

    private void LoadJson()
    {
        try
        {            
            string file = Path.Combine(Application.streamingAssetsPath, "JsonChallenge.json");
            string json = File.ReadAllText(file);
            map2 = JsonUtility.FromJson<Map2>(json);
            map2.Data = new List<Dictionary<string, string>>();

            ParseData(json);

            ShowData();
        }
        catch (Exception e)
        {
            title.text = e.Message;
        }
    }

    private void ParseData(string json)
    {
        string data = getBetween(json, "Data", "]");
        data = data.Substring(data.IndexOf('[') + 1);
        string[] datos = data.Split('}');

        foreach (string item in datos)
        {
            if (item.Trim().Equals("")) continue;
            string splitear = item.Substring(item.IndexOf('{') + 1).Trim();
            string[] columns = splitear.Split(',');            

            Dictionary<string, string> node = new Dictionary<string, string>();

            foreach (string column in columns)
            {                
                string[] keyValue = column.Trim().Split(':');
                if (keyValue.Length != 2) continue;

                string key = getBetween(keyValue[0].Trim(), "\"", "\"");
                string value = "";
                if (keyValue[1].Contains("\"")) value = getBetween(keyValue[1].Trim(), "\"", "\"");
                else value = keyValue[1];

                node.Add(key, value);
            }

            map2.Data.Add(node);
        }

       
    }

    private string getBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }

    private void ShowData()
    {
        title.text = map2.Title;

        foreach (string header in map2.ColumnHeaders)
        {
            toDelete.Add(CreateHeaders(header));
        }
        
        foreach (Dictionary<string,string> item in map2.Data)
        {            
            toDelete.AddRange(CreateRows(item));
        }

    }

    private GameObject CreateHeaders(string text)
    {
        GameObject clone = GameObject.Instantiate<GameObject>(Header, Panel.transform);
        inXnow += inX;
        clone.transform.position = new Vector3(clone.transform.position.x + inXnow, clone.transform.position.y, clone.transform.position.z);
        TextMeshProUGUI headerText = clone.GetComponent<TextMeshProUGUI>();
        headerText.text = text;

        return clone;
    }

    private List<GameObject> CreateRows(Dictionary<string, string> dic)
    {
        List<GameObject> retornar = new List<GameObject>();
        inXnow = 0;        
        inYnow += inY ;

        foreach (string key in map2.ColumnHeaders)
        {

            GameObject clone = GameObject.Instantiate<GameObject>(Row, Panel.transform);
            inXnow += inX;            
            clone.transform.position = new Vector3(clone.transform.position.x + inXnow, clone.transform.position.y - inYnow, clone.transform.position.z);
            TextMeshProUGUI rowText = clone.GetComponent<TextMeshProUGUI>();
            string value = "";
            dic.TryGetValue(key, out value);
            rowText.text = value;

            retornar.Add(clone);
        }

        return retornar;
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

}
