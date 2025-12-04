using UnityEngine;

public class QuestGiver : MonoBehaviour {

    [SerializeField] private QuestSystem questSystem;
    [SerializeField] private QuestSO questSO;

    public void GiveQuest() {
        questSystem.AddQuest(questSO);
    }

}