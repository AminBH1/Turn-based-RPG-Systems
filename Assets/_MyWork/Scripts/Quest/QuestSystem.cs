using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestSystem : MonoBehaviour, IPredicateEvaluator {

    public event Action OnQuestSystemUpdated;

    private List<QuestStatus> questStatusList = new List<QuestStatus>();

    public void AddQuest(QuestSO questToAdd) {
        if (HasQuest(questToAdd)) {
            return;
        }

        QuestStatus newQuestStatus = new QuestStatus(questToAdd);
        questStatusList.Add(newQuestStatus);
        OnQuestSystemUpdated?.Invoke();
    }

    public void CompleteObjective(QuestSO questSO, string objectiveToComplete) {
        if (!HasQuest(questSO)) {
            return;
        }

        GetQuestStatus(questSO).CompleteObjective(objectiveToComplete);
        OnQuestSystemUpdated?.Invoke();
    }

    public bool HasQuest(QuestSO questToCheck) {
        foreach (QuestStatus questStatus in questStatusList) {
            if (questStatus.GetQuestSO() == questToCheck) return true;
        }
        return false;
    }

    public QuestStatus GetQuestStatus(QuestSO questToCheck) {
        foreach (QuestStatus questStatus in questStatusList) {
            if (questStatus.GetQuestSO() == questToCheck) return questStatus;
        }
        return null;
    }

    public IEnumerable<QuestStatus> GetQuestStatusList() {
        return questStatusList;
    }

    public bool IsQuestCompleted(QuestStatus questStatus) {
        if (questStatus.IsQuestCompleted()) {
            return true;
        }
        return false;
    }

    private QuestStatus GetQuestStatusByQuestID(string questID) {
        foreach (QuestStatus questStatus in questStatusList) {
            if (questStatus.GetQuestSO().QuestID() == questID) {
                return questStatus;
            }
        }
        return null;
    }

    public bool? Evaluate(string predicate, string[] parametres) {
        switch (predicate) {
            case "FinishedQuest":
                if (GetQuestStatusByQuestID(parametres[0]) != null) {
                    if (IsQuestCompleted(GetQuestStatusByQuestID(parametres[0]))) {
                        return true;
                    }
                }
                return false;              
        }
        return null;
    }
}