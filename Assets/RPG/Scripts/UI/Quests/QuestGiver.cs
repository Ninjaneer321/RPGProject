using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] Quest quest;

        public void GiveQuest()
        {
            Debug.Log("GiveQuest");
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.AddQuest(quest);
        }

        public Quest GetQuestForQuestIconDisplay()
        {
            return quest;
        }

        public string GetQuestName()
        {
            return quest.name;
        }
    }
}
