using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest Data /new Quest")]
public class QuestSO : ScriptableObject {

    [SerializeField] private string questID = Guid.NewGuid().ToString();
    [SerializeField] private string displayName;
    [SerializeField] private string displayDescription;
    [SerializeField] private List<string> objectiveList = new List<string>();


    private void OnEnable() {
        
    }

    public string QuestID() {
        return questID;
    }

    public IEnumerable<string> GetObjectiveList() {
        return objectiveList;
    }

    public string GetDisplayName() {
        return displayName;
    }
}