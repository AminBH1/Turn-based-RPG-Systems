using UnityEngine;

public class InteractionSystem : MonoBehaviour {

    private Player player;

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue)) {
            if (raycastHit.transform.TryGetComponent<IInteractable>(out IInteractable interactable)) {
                if (Input.GetMouseButtonDown(0)) {
                    interactable.Interact(player);
                }
            }
        }
    }

}