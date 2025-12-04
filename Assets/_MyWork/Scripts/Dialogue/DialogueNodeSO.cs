using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class DialogueNodeSO : ScriptableObject {

    [SerializeField] private string nodeText;
    [SerializeField] private List<string> childIDList = new List<string>();
    [SerializeField] private bool isPlayerSpeaking;
    [SerializeField] private Rect nodeRect = new Rect(0, 0, 200, 100);
    [SerializeField] private string toTriggerAction;
    [SerializeField] private Condition condition;


    public string GetNodeText() {
        return nodeText;
    }

    public IEnumerable<string> GetChildIDList() {
        return childIDList;
    }

    public bool IsPlayerSpeaking() {
        return isPlayerSpeaking;
    }

    public Rect GetNodeRect() {
        return nodeRect;
    }

    public string GetToTriggerAction() {
        return toTriggerAction;
    }

    public bool CheckCondition(IEnumerable<IPredicateEvaluator> predicateEvaluatorList) {
        return condition.Check(predicateEvaluatorList);
    }



#if UNITY_EDITOR
    public void SetNodeText(string nodeText) {
        if (this.nodeText != nodeText) {
            Undo.RecordObject(this, "Changed NodeText");
            this.nodeText = nodeText;
            EditorUtility.SetDirty(this);
        }
    }

    public void SetNodeRect(Rect nodeRect) {
        if (this.nodeRect != nodeRect) {
            Undo.RecordObject(this, "Updated NodeHeight");
            this.nodeRect = nodeRect;
            EditorUtility.SetDirty(this);
        }
    }

    public void AddChild(string childID) {
        if (!childIDList.Contains(childID)) {
            Undo.RecordObject(this, "Added a child");
            childIDList.Add(childID);
            EditorUtility.SetDirty(this);
        }
    }

    public void RemoveChild(string childID) {
        if (childIDList.Contains(childID)) {
            Undo.RecordObject(this, "Removed a child");
            childIDList.Remove(childID);
            EditorUtility.SetDirty(this);
        }
    }

    public void SetIsPlayerSpeaking(bool isPlayerSpeaking) {
        if (this.isPlayerSpeaking != isPlayerSpeaking) {
            Undo.RecordObject(this, "Changed Speaker");
            this.isPlayerSpeaking = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
    }

    public void SetNodePosition(Vector2 positionOffset) {
        nodeRect.position = positionOffset;
    }
#endif
}