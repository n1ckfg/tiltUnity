using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLibUnityPort.Core;
using ICSharpCode.SharpZipLibUnityPort.Zip;
using SimpleJSON;

public class TiltLoader : MonoBehaviour {

    public string readFileName;
    public byte[] bytes;
    public JSONNode json;
    public int numStrokes;
    public List<TiltStroke> strokes;

    private ZipFile zipFile;

    IEnumerator Start() {
        string url = "";

#if UNITY_ANDROID
		url = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", readFileName);
#endif

#if UNITY_IOS
		url = Path.Combine("file://" + Application.dataPath + "/Raw", readFileName);
#endif

#if UNITY_EDITOR
        url = Path.Combine("file://" + Application.dataPath, readFileName);
#endif

#if UNITY_STANDALONE_WIN
        url = Path.Combine("file://" + Application.dataPath, readFileName);
#endif

#if UNITY_STANDALONE_OSX
        url = Path.Combine("file://" + Application.dataPath, readFileName);
#endif

#if UNITY_WSA
		url = Path.Combine("file://" + Application.dataPath, readFileName);		
#endif

        WWW www = new WWW(url);
        yield return www;

        Debug.Log("+++ File reading finished. Begin parsing...");
        yield return new WaitForSeconds(0);
    }

    void Update() {
        
    }

    JSONNode getJsonFromZip(byte[] bytes) {
        // https://gist.github.com/r2d2rigo/2bd3a1cafcee8995374f

        MemoryStream fileStream = new MemoryStream(bytes, 0, bytes.Length);
        ZipFile zipFile = new ZipFile(fileStream);

        foreach (ZipEntry entry in zipFile) {
            if (Path.GetExtension(entry.Name).ToLower() == ".json") {
                Stream zippedStream = zipFile.GetInputStream(entry);
                StreamReader read = new StreamReader(zippedStream, true);
                string json = read.ReadToEnd();
                Debug.Log(json);
                return JSON.Parse(json);
            }
        }

        return null;
    }

}
