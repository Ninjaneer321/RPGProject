using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] AudioSource questAcceptedSound = null;

        public void GiveQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            if (questAcceptedSound != null)
            {
                questAcceptedSound.Play();
            }
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
