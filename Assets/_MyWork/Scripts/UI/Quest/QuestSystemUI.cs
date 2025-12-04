using UnityEngine;

public class QuestSystemUI : MonoBehaviour {

    [SerializeField] private QuestSystem questSystem;
    [SerializeField] private Transform questsRootUI;
    [SerializeField] private Transform questPrefabUI;


    private void Start() {
        questSystem.OnQuestSystemUpdated += QuestSystem_OnQuestSystemUpdated;

        QuestSystem_OnQuestSystemUpdated();
    }

    private void QuestSystem_OnQuestSystemUpdated() {
        foreach (Transform child in questsRootUI) {
            Destroy(child.gameObject);
        }

        foreach (QuestStatus questStatus in questSystem.GetQuestStatusList()) {
            Transform questTransform = Instantiate(questPrefabUI, questsRootUI);
            questTransform.TryGetComponent<QuestUI>(out QuestUI questUI);
            questUI.Setup(questStatus);
        }
    }
}