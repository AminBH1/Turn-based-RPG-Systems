using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {

    [SerializeField] private DialogueHandler dialogueHandler;
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Transform choicesRootUI;
    [SerializeField] private Transform choicePrefabUI;


    private void Start() {
        dialogueHandler.OnDialogueUpdated += DialogueHandler_OnDialogueUpdated;

        nextButton.onClick.AddListener(() => {
            dialogueHandler.Next();
        });

        quitButton.onClick.AddListener(() => {
            dialogueHandler.Quit();
        });

        DialogueHandler_OnDialogueUpdated();
    }

    private void DialogueHandler_OnDialogueUpdated() {
        gameObject.SetActive(dialogueHandler.IsActiveDialogue());
        if (!dialogueHandler.IsActiveDialogue()) {
            return;
        }
        NPCNameText.text = dialogueHandler.GetNPCName();
        NPCText.text = dialogueHandler.GetCurrentNode().GetNodeText();
        choicesRootUI.gameObject.SetActive(dialogueHandler.IsPlayerSpeaking());
        nextButton.gameObject.SetActive(true);
        if (dialogueHandler.IsPlayerSpeaking()) {
            BuildChoiceList();
            nextButton.gameObject.SetActive(false);
            return;
        }

        nextButton.gameObject.SetActive(dialogueHandler.HasNext());
    }

    private void BuildChoiceList() {
        foreach (Transform child in choicesRootUI) {
            Destroy(child.gameObject);
        }

        foreach (DialogueNodeSO choiceNode in dialogueHandler.GetChoiceNodeList()) {
            Transform choiceTransform = Instantiate(choicePrefabUI, choicesRootUI);
            if (choiceTransform.TryGetComponent<ChoiceUI>(out ChoiceUI choiceUI)) {
                choiceUI.Setup(choiceNode, dialogueHandler);
                
            }
        }
    }
}