using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DialogueActionTrigger : MonoBehaviour {


    [SerializeField] private string action;
    [SerializeField] private UnityEvent OnTriggerAction;

    public void TriggerAction(string action) {
        if (this.action == action) {
            OnTriggerAction?.Invoke();
        }
    }

}