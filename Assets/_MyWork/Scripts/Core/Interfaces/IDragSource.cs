using UnityEngine;

public interface IDragSource<T> where T : class {

    public T GetItem();

    public int GetAmount();

    public int GetMaxStackAmount();
}