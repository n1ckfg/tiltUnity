using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltLoader : MonoBehaviour {

    public string url;
    public byte[] bytes;
    public JSONObject json;
    public int numStrokes;
    public List<TiltStroke> strokes;

    private ZipFile zipFile;
    private ArrayList<String> fileNames;

    void Start() {
        
    }

    void Update() {
        
    }

}
