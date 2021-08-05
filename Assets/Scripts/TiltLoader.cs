using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
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

    void Start() {
        read(readFileName);
    }

	public void read(string readFileName) {
        StartCoroutine(reader(readFileName));
	}

	private IEnumerator reader(string readFileName) {
        // A tilt zipfile should contain three items: thumbnail.png, data.sketch, metadata.json
        string url = formPath(readFileName);

        WWW www = new WWW(url);
        yield return www;

        getEntriesFromZip(www.bytes);
    }

    private void getEntriesFromZip(byte[] bytes) {
        // https://gist.github.com/r2d2rigo/2bd3a1cafcee8995374f

        MemoryStream fileStream = new MemoryStream(bytes, 0, bytes.Length);
        zipFile = new ZipFile(fileStream);

        foreach (ZipEntry entry in zipFile) {
			switch(entry.Name.ToLower()) {
                case "metadata.json":
                    json = JSON.Parse(readEntryAsString(entry));
                    Debug.Log(json);
                    break;
                case "data.sketch":
                    bytes = readEntryAsBytes(entry);
                    break;
			}
        }
    }

	private void parseTilt() {

	}

	private byte[] readEntryAsBytes(ZipEntry entry) {
        Stream zippedStream = zipFile.GetInputStream(entry);
        MemoryStream ms = new MemoryStream();
        zippedStream.CopyTo(ms);
        return ms.ToArray();
    }

    private string readEntryAsString(ZipEntry entry) {
        Stream zippedStream = zipFile.GetInputStream(entry);
        StreamReader read = new StreamReader(zippedStream, true);
        return read.ReadToEnd();
    }

    private int getUInt(byte[] _bytes, int _offset) {
        byte[] uintBytes = { _bytes[_offset], _bytes[_offset + 1], _bytes[_offset + 2], _bytes[_offset + 3] };
        return asUInt(uintBytes);
    }

    private int getInt(byte[] _bytes, int _offset) {
        byte[] intBytes = { _bytes[_offset], _bytes[_offset + 1], _bytes[_offset + 2], _bytes[_offset + 3] };
        return asInt(intBytes);
    }

    private float getFloat(byte[] _bytes, int _offset) {
        byte[] floatBytes = { _bytes[_offset], _bytes[_offset + 1], _bytes[_offset + 2], _bytes[_offset + 3] };
        return asFloat(floatBytes);
    }

    private int asUInt(byte[] _bytes) {
        int i = asInt(_bytes);
        long unsigned = i & 0xffffffffL;
        return (int)unsigned;
    }

    private int asInt(byte[] _bytes) {
        return BitConverter.ToInt32(_bytes, 0);
    }

    private float asFloat(byte[] _bytes) {
        return BitConverter.ToSingle(_bytes, 0);
    }
	
    private string formPath(string readFileName) {
        string url = "";
#if UNITY_ANDROID
		url = Path.Combine("jar:file://" + Application.streamingAssetsPath + "!/assets/", readFileName);
#endif

#if UNITY_IOS
		url = Path.Combine("file://" + Application.streamingAssetsPath + "/Raw", readFileName);
#endif

#if UNITY_EDITOR
        url = Path.Combine("file://" + Application.streamingAssetsPath, readFileName);
#endif

#if UNITY_STANDALONE_WIN
        url = Path.Combine("file://" + Application.streamingAssetsPath, readFileName);
#endif

#if UNITY_STANDALONE_OSX
        url = Path.Combine("file://" + Application.streamingAssetsPath, readFileName);
#endif

#if UNITY_WSA
		url = Path.Combine("file://" + Application.streamingAssetsPath, readFileName);		
#endif
		return url;
    }

}
