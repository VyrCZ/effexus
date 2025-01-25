using UnityEngine;

public class Collectible : MonoBehaviour {
    public enum Type{
        tp,
        activatorUp,
        activatorDown
    }
    public Type type;
}