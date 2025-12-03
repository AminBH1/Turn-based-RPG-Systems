using UnityEngine;

public interface IDragContainer<T> : IDragSource<T>, IDragDestination<T> where T : class {


}
