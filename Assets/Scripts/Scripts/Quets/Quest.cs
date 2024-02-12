using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set; }
    
    public Quest(QuestBase _base)
    {
        Base = _base;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;

       yield return DialogManager.Instance.ShowDialog(Base.StartDialogue);
    }

    /*public IEnumerator CompleteQuest()
    {
        Status = QuestStatus.Completed;

        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialogue);

        var intventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            Inventory.RemoveItem(Base.RequiredItem);
        }
    }*/
    
    
    
}


public enum QuestStatus {None, Started,Completed}