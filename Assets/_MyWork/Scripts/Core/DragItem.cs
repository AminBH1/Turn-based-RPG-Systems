using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class {

    private Canvas parentCanvas;
    private Transform originalParent;
    private Vector3 startPosition;
    private IDragSource<T> dragSource;

    private void Awake() {
        parentCanvas = GetComponentInParent<Canvas>();
        dragSource = GetComponentInParent<IDragSource<T>>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        originalParent = transform.parent;
        startPosition = transform.position;

        GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.SetParent(parentCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.position = startPosition;
        transform.SetParent(originalParent, true);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        IDragDestination<T> destination;
        if (!EventSystem.current.IsPointerOverGameObject()) {
            destination = null;
        } else {
            destination = GetDestinationInPoint(eventData);
        }

        if (destination != null) {
            HandleExchange(destination);
        }
    }

    private IDragDestination<T> GetDestinationInPoint(PointerEventData eventData) {
        if (eventData.pointerEnter) {
            IDragDestination<T> destination = eventData.pointerEnter.transform.GetComponentInParent<IDragDestination<T>>();
            return destination;
        }
        return null;
    }

    private void HandleExchange(IDragDestination<T> destination) {

        if (object.ReferenceEquals(destination, dragSource)) {
            return;
        }

        IDragContainer<T> sourceContainer = dragSource as IDragContainer<T>;
        IDragContainer<T> destinationContainer = destination as IDragContainer<T>;

        if (sourceContainer == null || destinationContainer.GetItem() == null
            || sourceContainer.GetItem() == null || object.ReferenceEquals(sourceContainer.GetItem(), destinationContainer.GetItem())) {
            TransferItem(sourceContainer, destinationContainer);
            return;
        }

        SwapItems(sourceContainer, destinationContainer);   
    }

    private void TransferItem(IDragContainer<T> sourceContainer, IDragContainer<T> destinationContainer) {
        var itemToTransfer = sourceContainer.GetItem();
        int amountToTransfer = sourceContainer.GetAmount();
        int maxStackAmount  = sourceContainer.GetMaxStackAmount();
        int existingAmount = destinationContainer.GetAmount();

        int allowedTransferAount = Mathf.Min(maxStackAmount - existingAmount, amountToTransfer);
        int restAmount = amountToTransfer - allowedTransferAount;

        destinationContainer.SetItem(itemToTransfer);
        destinationContainer.SetAmount(allowedTransferAount + existingAmount);

        if (restAmount > 0) {
            sourceContainer.SetAmount(restAmount);
            return;
        }
        sourceContainer.SetAmount(0);
        sourceContainer.SetItem(null);
    }

    private void SwapItems(IDragContainer<T> sourceContainer, IDragContainer<T> destinationContainer) {
        var sourceItem = sourceContainer.GetItem();
        int sourceAmount = sourceContainer.GetAmount();  
        var destinationItem = destinationContainer.GetItem();
        int destinationAmount = destinationContainer.GetAmount();

        sourceContainer.SetItem(destinationItem);
        sourceContainer.SetAmount(destinationAmount);

        destinationContainer.SetItem(sourceItem);
        destinationContainer.SetAmount(sourceAmount);
    }
}