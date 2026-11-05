using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class PlayerGroundedData
{
    [field: SerializeField, Range(0f, 25f)] public float baseSpeed { get; private set; } = 5f;
    [field: SerializeField] public PlayerRotationData playerRotationData { get; private set; } = new PlayerRotationData();
    [field: SerializeField] public PlayerRunData playerRunnData { get; private set; } = new PlayerRunData();

}
