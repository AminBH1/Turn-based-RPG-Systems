using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogueEditor : EditorWindow {

    private DialogueSO selectedDialogueSO;

    private GUIStyle NPCNodeStyle;
    private GUIStyle playerNodeStyle;
    private GUIStyle finalNodeStyle;
    private GUIStyle textAreaStyle;

    private Vector2 scrollPosition;

    private const float canvasSize = 8000f;
    private const float backgroundSize = 50f;

    private DialogueNodeSO parentOfToAddNode;
    private DialogueNodeSO toDeleteNode;
    private DialogueNodeSO draggingNode;
    private DialogueNodeSO currentLinkingNode;

    private bool draggingCanvas;

    private Vector2 draggingNodeOffset;
    private Vector2 draggingCanvasOffset;


    [MenuItem("Window/Dialogue Editor")]
    public static void ShowDialogueEditor() {
        GetWindow(typeof(DialogueEditor), false, "DialogueEditor");
    }

    [OnOpenAsset(1)]
    public static bool OnAssetOpen(int instanceID, int line) {
        DialogueSO dialogueSO = EditorUtility.InstanceIDToObject(instanceID) as DialogueSO;
        if (dialogueSO != null) {
            ShowDialogueEditor();
            return true;
        }
        return false;
    }

    private void OnEnable() {
        Selection.selectionChanged += OnSelectionChanged;

        NPCNodeStyle = new GUIStyle();
        NPCNodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
        NPCNodeStyle.padding = new RectOffset(12, 12, 12, 12);
        NPCNodeStyle.border = new RectOffset(20, 20, 20, 20);

        playerNodeStyle = new GUIStyle();
        playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        playerNodeStyle.padding = new RectOffset(12, 12, 12, 12);
        playerNodeStyle.border = new RectOffset(20, 20, 20, 20);
    }

    private void OnSelectionChanged() {
        DialogueSO dialogueSO = Selection.activeObject as DialogueSO;
        if (dialogueSO != null) {
            selectedDialogueSO = dialogueSO;
        }
    }

    private void OnGUI() {
        if (selectedDialogueSO == null) {
            Debug.Log("Please Select a Dialogue");
        } else {
            SetupTextAreaStyle();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
            Texture2D backgroundTexture = Resources.Load("Background") as Texture2D;
            Rect textureCoordination = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoordination);

            ProcessEvents();

            foreach (DialogueNodeSO currentNode in selectedDialogueSO.GetNodeList()) {
                DrawNode(currentNode);
            }

            foreach (DialogueNodeSO currentNode in selectedDialogueSO.GetNodeList()) {
                DrawNodeLinks(currentNode);
            }

            if (parentOfToAddNode != null) {
                selectedDialogueSO.AddNodeToParent(parentOfToAddNode);
                parentOfToAddNode = null;
            }

            if (toDeleteNode != null) {
                selectedDialogueSO.DeleteNode(toDeleteNode);
                toDeleteNode = null;
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawNode(DialogueNodeSO currentNode) {
        finalNodeStyle = NPCNodeStyle;
        if (currentNode.IsPlayerSpeaking()) {
            finalNodeStyle = playerNodeStyle;
        }

        GUILayout.BeginArea(currentNode.GetNodeRect(), finalNodeStyle);
        EditorGUI.BeginChangeCheck();

        currentNode.SetNodeText(EditorGUILayout.TextArea(currentNode.GetNodeText(), textAreaStyle, GUILayout.ExpandHeight(true)));

        float currentTextHeight = textAreaStyle.CalcHeight(new GUIContent(currentNode.GetNodeText()), currentNode.GetNodeRect().width);
        float rectHeightOffset = 80;
        float wantedRectHeight = currentTextHeight + rectHeightOffset;

        if (!Mathf.Approximately(currentNode.GetNodeRect().height, wantedRectHeight)) {
            currentNode.SetNodeRect(new Rect(currentNode.GetNodeRect().x, currentNode.GetNodeRect().y, currentNode.GetNodeRect().width, wantedRectHeight));
            Repaint();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Child")) {
            parentOfToAddNode = currentNode;
        }

        if (GUILayout.Button("Delete")) {
            toDeleteNode = currentNode;
        }

        DrawLinkingButtons(currentNode);

        GUILayout.EndHorizontal();


        if (GUILayout.Button("Change Speaker")) {
            currentNode.SetIsPlayerSpeaking(!currentNode.IsPlayerSpeaking());
        }

        EditorGUI.EndChangeCheck();
        GUILayout.EndArea();
    }

    private void DrawLinkingButtons(DialogueNodeSO currentNode) {
        if (currentLinkingNode == null) {

            if (GUILayout.Button("Link")) {
                currentLinkingNode = currentNode;
            }
        } else if (currentLinkingNode == currentNode) {

            if (GUILayout.Button("Cancel")) {
                currentLinkingNode = null;
            }
        } else if (currentLinkingNode != null) {

            if (currentLinkingNode.GetChildIDList().Contains(currentNode.name)) {

                if (GUILayout.Button("Unlink Child")) {
                    currentLinkingNode.RemoveChild(currentNode.name);
                    currentLinkingNode = null;
                }
            } else if (GUILayout.Button("Link Child")) {
                currentLinkingNode.AddChild(currentNode.name);
                currentLinkingNode = null;
            }
        } 
    }

    private void DrawNodeLinks(DialogueNodeSO currentNode) {
        Vector3 startPosition = new Vector2(currentNode.GetNodeRect().center.x, currentNode.GetNodeRect().yMax);
        foreach (DialogueNodeSO childNode in selectedDialogueSO.GetChildNodeList(currentNode)) {
            Vector3 endPosition = new Vector2(childNode.GetNodeRect().center.x, childNode.GetNodeRect().yMin);
            Vector3 linkOffset = endPosition - startPosition;
            linkOffset.x *= .1f;
            linkOffset.y *= 1.2f;
            Handles.DrawBezier(
                startPosition, endPosition,
                startPosition + linkOffset,
                endPosition - linkOffset,
                Color.white, null, 4f
                );
        }
    }

    private void ProcessEvents() {
        if (Event.current.type == EventType.MouseDown && draggingNode == null) {
            draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
            if (draggingNode != null) {
                draggingNodeOffset = draggingNode.GetNodeRect().position - Event.current.mousePosition; 
                Selection.activeObject = draggingNode;
            } else {
                draggingCanvas = true;
                draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
            }
        } else if (Event.current.type == EventType.MouseDrag && draggingNode != null) {
            draggingNode.SetNodePosition(Event.current.mousePosition + draggingNodeOffset);

            GUI.changed = true;
        } else if (Event.current.type == EventType.MouseDrag && draggingCanvas == true) {
            scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

            GUI.changed = true;
        } else if (Event.current.type == EventType.MouseUp && draggingNode != null) {
            draggingNode = null;
        } else if (Event.current.type == EventType.MouseUp && draggingCanvas == true) {
            draggingCanvas = false;
        }
    }

    private DialogueNodeSO GetNodeAtPoint(Vector2 point) {
        foreach (DialogueNodeSO node in selectedDialogueSO.GetNodeList()) {
            if (node.GetNodeRect().Contains(point)) {
                return node;
            }
        }
        return null;
    }


    private void SetupTextAreaStyle() {
        if (textAreaStyle == null) {
            textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
        }
    }
}