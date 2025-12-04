using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class QuestStatus {

    private QuestSO questSO;
    private List<string> completedObjectiveList = new List<string>();


    public QuestStatus(QuestSO questSO) {
        this.questSO = questSO;
    }

    public QuestSO GetQuestSO() {
        return questSO;
    }

    public IEnumerable<string> GetCompletedObjectiveList() {
        return completedObjectiveList;
    }

    public void CompleteObjective(string objectiveToComplete) {
        if (!completedObjectiveList.Contains(objectiveToComplete)) {
            completedObjectiveList.Add(objectiveToComplete);
        }
    }

    public bool IsObjectiveCompleted(string objective) {
        if (completedObjectiveList.Contains(objective)) {
            return true;
        }
        return false;
    }

    public bool IsQuestCompleted() {
        if (completedObjectiveList.Count == questSO.GetObjectiveList().Count()) {
            return true;
        }
        return false;
    }
}