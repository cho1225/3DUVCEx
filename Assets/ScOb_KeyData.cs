using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create KeyData")]
public class ScOb_KeyData : ScriptableObject
{
    public string playerName;

    public float groundedOffset = 0.0f;
    public float groundedRadius = 4.0f;
    public bool is_inArea;
    public int maxScaleCoun = 1000;

    public float scalePerFra = 0.005f;
}