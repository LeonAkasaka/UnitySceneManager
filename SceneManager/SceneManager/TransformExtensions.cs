using UnityEngine;
using System.Collections;
using System.Linq;

public static class TransformExtensions
{
    /// <summary>
    /// Destroys all children.
    /// </summary>
    /// <param name="t"></param>
    public static void DestroyChildren(this Transform t)
    {
        var children = t.Cast<Transform>().Select(x => x.gameObject);
        foreach (var c in children) { GameObject.Destroy(c); }
    }

    /// <summary>
    /// Gets a Transform object of root GameObject.
    /// </summary>
    /// <param name="t">Transform object.</param>
    /// <returns>Transform object of root GameObject.</returns>
    public static Transform GetRoot(this Transform t)
    {
        if (t.parent == null) { return t; }
        return t.parent.GetRoot();
    }
}
