using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> items = new List<T>();

    public virtual void Add(T elem)
    {
        if (!this.items.Contains(elem))
            this.items.Add(elem);
    }

    public virtual void Remove(T elem)
    {
        if (this.items.Contains(elem))
            this.items.Remove(elem);
    }

    public virtual void ClearSet()
    {
        this.items.Clear();
    }
}