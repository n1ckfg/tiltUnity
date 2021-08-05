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
    public int numStrokes;
    public List<TiltStroke> strokes;
	public JSONNode json;

	[HideInInspector] public byte[] bytes;

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

        parseTilt();
    }

    private void getEntriesFromZip(byte[] bytes) {
        // https://gist.github.com/r2d2rigo/2bd3a1cafcee8995374f

        MemoryStream fileStream = new MemoryStream(bytes, 0, bytes.Length);
        zipFile = new ZipFile(fileStream);

        foreach (ZipEntry entry in zipFile) {
			switch(entry.Name.ToLower()) {
                case "metadata.json":
                    json = JSON.Parse(readEntryAsString(entry));
                    break;
                case "data.sketch":
                    bytes = readEntryAsBytes(entry);
                    break;
			}
        }
    }

	private void parseTilt() {
        strokes = new List<TiltStroke>();

        numStrokes = getInt(bytes, 33);

        /*
		int offset = 20;

        for (int i = 0; i < numStrokes; i++) {
            int brushIndex = getInt(bytes, offset);

            float r = getFloat(bytes, offset + 4) * 255;
            float g = getFloat(bytes, offset + 8) * 255;
            float b = getFloat(bytes, offset + 12) * 255;
            float a = getFloat(bytes, offset + 16) * 255;
            Color brushColor = new Color(r, g, b, a);

            float brushSize = getFloat(bytes, offset + 20);
            UInt32 strokeMask = getUInt(bytes, offset + 24);
            UInt32 controlPointMask = getUInt(bytes, offset + 28);

            int offsetStrokeMask = 0;
            int offsetControlPointMask = 0;

            for (int j = 0; j < 4; j++) {
                byte bb = (byte)(1 << j);
                if ((strokeMask & bb) > 0) offsetStrokeMask += 4;
                if ((controlPointMask & bb) > 0) offsetControlPointMask += 4;
            }

            //parent.println("1. " + brushIndex + ", [" + brushColorArray[0] + ", " + brushColorArray[1] + ", " + brushColorArray[2] + ", " + brushColorArray[3] + "]," + brushSize);
            //parent.println("2. " + offsetStrokeMask + "," + offsetControlPointMask + "," + strokeMask + "," + controlPointMask);

            offset += 28 + offsetStrokeMask + 4;

            int numControlPoints = getInt(bytes, offset);

			//parent.println("3. " + numControlPoints);

			List<Vector3> positions = new List<Vector3>();

            offset += 4;

            for (int j = 0; j < numControlPoints; j++) {
                float x = getFloat(bytes, offset + 0);
                float y = getFloat(bytes, offset + 4);
                float z = getFloat(bytes, offset + 8);
                positions.Add(new Vector3(x, y, z));

                //float qw = getFloat(bytes, offset + 12);
                //float qx = getFloat(bytes, offset + 16);
                //float qy = getFloat(bytes, offset + 20);
                //float qz = getFloat(bytes, offset + 24);

                offset += 28 + offsetControlPointMask;
            }

            //parent.println("4. " + positions.get(0).x + ", " + positions.get(0).y + ", " + positions.get(0).z);

            strokes.Add(new TiltStroke(positions, brushSize, brushColor));
        }
		*/
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
        return (_bytes[0] & 0xFF)
               | ((_bytes[1] & 0xFF) << 8)
               | ((_bytes[2] & 0xFF) << 16)
               | ((_bytes[3] & 0xFF) << 24);
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
