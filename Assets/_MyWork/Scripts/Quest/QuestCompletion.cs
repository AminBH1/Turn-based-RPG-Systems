using UnityEngine;

public class QuestCompletion : MonoBehaviour {

    [SerializeField] private QuestSystem questSystem;
    [SerializeField] private QuestSO questSO;
    [SerializeField] private string objective;


    public void CompleteObjective() {
        questSystem.CompleteObjective(questSO, objective);
    }
}