using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PadType
{
    None,
    Speed,
    Jump,
    Hover,
    Checkpoint,
    NoJump,
    Timewarp
}

public class PowerPad : MonoBehaviour
{
    public PadType padType = PadType.None;
}
