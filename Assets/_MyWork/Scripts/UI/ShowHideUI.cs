using UnityEngine;

public class ShowHideUI : MonoBehaviour {

    [SerializeField] private GameObject gameObjectToHide;
    [SerializeField] private KeyCode KeyCode;

    private bool isActive;

    private void Update() {
        if (Input.GetKeyDown(KeyCode)) {
            gameObjectToHide.SetActive(!isActive);
            isActive = !isActive;
        }
    }
}