using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signals
{
    public static Action<int> OnActiveBlockTypeChanged;
    public static Action<Vector3Int, Chunk> OnPlayerRightClick;

}
