using UnityEngine;

public abstract class Scene<T> : MonoBehaviour
{
    public abstract void Initialized(T parameter);
}