using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueHandler : MonoBehaviour {

    public event Action OnDialogueUpdated;

    private DialogueSO dialogueStarted;
    private NPCSpeaker NPCSpeaker;
    private DialogueNodeSO currentNode;

    private bool isPlayerSpeaking;
    private bool isActiveDialogue;

    public void StartDialogue(DialogueSO dialogueStarted, NPCSpeaker NPCSpeaker) {
        this.dialogueStarted = dialogueStarted; 
        this.NPCSpeaker = NPCSpeaker;
        currentNode = dialogueStarted.GetRootNode();
        isActiveDialogue = true;
        OnDialogueUpdated?.Invoke();
    }

    public void Quit() {
        NPCSpeaker = null;
        currentNode = null;
        dialogueStarted = null;
        isPlayerSpeaking = false;
        isActiveDialogue = false;
        OnDialogueUpdated?.Invoke();
    }

    public bool IsActiveDialogue() {
        return isActiveDialogue;
    }

    public bool IsPlayerSpeaking() {
        return isPlayerSpeaking;
    }

    public IEnumerable<DialogueNodeSO> GetChoiceNodeList() {
        return FilterNodeListOnCondition(dialogueStarted.GetChoiceNodeList(currentNode));
    }

    public DialogueNodeSO GetCurrentNode() {
        return currentNode;
    }

    public string GetNPCName() {
        return NPCSpeaker.GetNPCName();
    }

    public bool HasNext() {
        if (FilterNodeListOnCondition(dialogueStarted.GetChildNodeList(currentNode)).Count() == 0) {
            return false;
        }
        return true;
    }
    public void SelectChoice(DialogueNodeSO choiceNode) {
        currentNode = choiceNode;
        TriggerAction();
        isPlayerSpeaking = false;
        if (HasNext()) {
            Next();
        } else {
            Quit();
        }
    }

    public void Next() {
        if (HasChoiceNode(currentNode)) return;
      
        DialogueNodeSO nextNode = FilterNodeListOnCondition(dialogueStarted.GetNPCNodeList(currentNode)).ToArray()[0];
        currentNode = nextNode;
        TriggerAction();

        if (HasChoiceNode(nextNode)) return;

        OnDialogueUpdated?.Invoke();
    }

    private bool HasChoiceNode(DialogueNodeSO parentNode) {
        if (FilterNodeListOnCondition(dialogueStarted.GetChoiceNodeList(parentNode)).Count() > 0) {
            isPlayerSpeaking = true;
            OnDialogueUpdated?.Invoke();
            return true;
        }
        return false;
    }

    private void TriggerAction() {
        if (currentNode == null) {
            return;
        }

        if (currentNode.GetToTriggerAction() == "") {
            return;
        }

        if (NPCSpeaker.GetComponents<NPCSpeaker>() == null) {
            return;
        }

        foreach (DialogueActionTrigger dialogueActionTrigger in NPCSpeaker.GetComponents<DialogueActionTrigger>()) {
            dialogueActionTrigger.TriggerAction(currentNode.GetToTriggerAction());
        }
    }

    private IEnumerable<DialogueNodeSO> FilterNodeListOnCondition(IEnumerable<DialogueNodeSO> nodeList) {
        IEnumerable<IPredicateEvaluator> predicateEvaluatorList = GetComponents<IPredicateEvaluator>();
        foreach (DialogueNodeSO node in nodeList) {
            if (node.CheckCondition(predicateEvaluatorList)) {
                yield return node;
            }
        }
    }
}