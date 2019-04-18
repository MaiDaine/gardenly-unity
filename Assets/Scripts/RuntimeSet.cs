using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> items = new List<T>();

    public virtual void Add(T elem)
    {
        if (!items.Contains(elem))
            items.Add(elem);
    }

    public virtual void Remove(T elem)
    {
        if (items.Contains(elem))
            items.Remove(elem);
    }

    public virtual void ClearSet()
    {
        items.Clear();
    }
}