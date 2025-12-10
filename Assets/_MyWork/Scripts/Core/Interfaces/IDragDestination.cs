using UnityEngine;

public interface IDragDestination<T> where T : class {

    public void SetItem(T item);

    public void SetAmount(int amount);
}