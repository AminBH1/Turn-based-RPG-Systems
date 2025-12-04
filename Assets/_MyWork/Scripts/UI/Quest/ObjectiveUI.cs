using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI objectiveNameText;
    [SerializeField] private TextMeshProUGUI objectiveStatusText;

    private string objective;
    private QuestStatus questStatus;

    public void Setup(string objective, QuestStatus questStatus) {
        this.objective = objective;
        this.questStatus = questStatus;

        SetVisual();
    }

    public void SetVisual() {
        objectiveNameText.text = objective;
        objectiveStatusText.text = questStatus.IsObjectiveCompleted(objective).ToString();
    }
}