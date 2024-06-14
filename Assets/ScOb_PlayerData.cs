using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create PlayerData")]
public class ScOb_PlayerData : ScriptableObject
{
    public string playerName;
    public float moveSpeed = 7f;
    public float speedChangeRate = 10f;

    public float rotationSmoothTime = 0.12f;

    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;
    public float groundedOffset = 0.0f;
    public float groundedRadius = 0.1f;
    public LayerMask groundLayers;

    public float shortJumpHeight = 1.2f;
}