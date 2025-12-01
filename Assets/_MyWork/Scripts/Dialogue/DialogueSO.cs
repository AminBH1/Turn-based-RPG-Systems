using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue Data/New DialogueSO")]
public class DialogueSO : ScriptableObject, ISerializationCallbackReceiver {

    [SerializeField] private List<DialogueNodeSO> nodeList = new List<DialogueNodeSO>();
    [SerializeField] private Vector2 newNodeOffsetPosition = new Vector2(0, 100);

    private Dictionary<string, DialogueNodeSO> lookupNodeByID = new Dictionary<string, DialogueNodeSO>();

    private void OnValidate() {
        lookupNodeByID.Clear();
        foreach (DialogueNodeSO node in nodeList) {
            lookupNodeByID[node.name] = node;
        }
    }

    public IEnumerable<DialogueNodeSO> GetNodeList() {
        return nodeList;
    }


#if UNITY_EDITOR

    public void AddNodeToParent(DialogueNodeSO parentNode) {
        DialogueNodeSO newNode = CreateNode(parentNode);
        Undo.RegisterCreatedObjectUndo(newNode, "Created a Node");
        Undo.RecordObject(this, "Added a node");
        nodeList.Add(newNode);
        OnValidate();
    }

    public void DeleteNode(DialogueNodeSO toDeleteNode) {
        foreach (DialogueNodeSO node in nodeList) {
            node.RemoveChild(toDeleteNode.name);
        }

        nodeList.Remove(toDeleteNode);
        OnValidate();
        Undo.DestroyObjectImmediate(toDeleteNode);
    }

    private DialogueNodeSO CreateNode(DialogueNodeSO parentNode) {
        DialogueNodeSO newNode = CreateInstance<DialogueNodeSO>();
        newNode.name = Guid.NewGuid().ToString();
        if (parentNode != null) {
            parentNode.AddChild(newNode.name);
            newNode.SetIsPlayerSpeaking(!parentNode.IsPlayerSpeaking());
            newNode.SetNodePosition(newNodeOffsetPosition + parentNode.GetNodeRect().position);
        }
        return newNode;
    }

#endif

    public void OnBeforeSerialize() {
#if UNITY_EDITOR
        if (nodeList.Count == 0) {
            DialogueNodeSO newNode = CreateNode(null);
            nodeList.Add(newNode);
            OnValidate();
        }

        if (AssetDatabase.GetAssetPath(this) != "") {
            foreach (DialogueNodeSO node in nodeList) {
                if (AssetDatabase.GetAssetPath(node) == "") {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
        }
#endif
    }

    public void OnAfterDeserialize() {

    }

    public IEnumerable<DialogueNodeSO> GetChildNodeList(DialogueNodeSO parentNode) {
        foreach (string childID in parentNode.GetChildIDList()) {
            if (lookupNodeByID.ContainsKey(childID)) {
                yield return lookupNodeByID[childID];
            }
        }
    }
}
