using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        QuestList questList;
        [SerializeField] QuestItemUI questPrefab;
        // Start is called before the first frame update
        void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onQuestListUpdated += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            transform.DetachChildren();

            foreach (QuestStatus status in questList.GetStatuses())
            {
                if (status.IsComplete()) continue;
                QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
                uiInstance.Setup(status);
            }
        }
    }
}
