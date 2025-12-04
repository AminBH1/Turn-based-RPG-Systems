using System;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private Transform objectivesRootUI;
    [SerializeField] private Transform objectivePrefabUI;

    private QuestStatus questStatus;

    public void Setup(QuestStatus questStatus) {
        this.questStatus = questStatus;
      
        SetVisual();
        SetupObjectiveUI();
    }

    private void SetupObjectiveUI() {
        foreach (Transform child in objectivesRootUI) {
            Destroy(child.gameObject);
        }

        foreach (string objective in questStatus.GetQuestSO().GetObjectiveList()) {
            Transform objectiveTransform = Instantiate(objectivePrefabUI, objectivesRootUI);
            objectiveTransform.TryGetComponent<ObjectiveUI>(out ObjectiveUI objectiveUI);
            objectiveUI.Setup(objective, questStatus);
        }
    }

    private void SetVisual() {
        questNameText.text = questStatus.GetQuestSO().GetDisplayName();
        questStatusText.text = questStatus.IsQuestCompleted().ToString();
    }

}