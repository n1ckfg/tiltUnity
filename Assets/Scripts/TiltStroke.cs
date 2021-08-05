using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltStroke : MonoBehaviour {

    public List<Vector3> positions;
    public float brushSize;
    public Color brushColor;

    public TiltStroke(List<Vector3> _positions, float _brushSize, Color _brushColor) {
        positions = _positions;
        brushSize = _brushSize;
        brushColor = _brushColor;
    }

}
