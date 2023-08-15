using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new List<QuestStatus>();
        [SerializeField] QuestPopupUI questPopupUI = null;
        [SerializeField] AudioSource questCompleteSound = null;

        public event Action onQuestListUpdated;
        public event Action onQuestGained;
        public event Action onQuestRemoved;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;

            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);


            string newQuestString = "<br>Quest accepted: " + quest.GetTitle();
            ChatBox chatBox = GameObject.FindGameObjectWithTag("Player").GetComponent<ChatBox>();
            chatBox.UpdateText(newQuestString);

            questPopupUI.gameObject.SetActive(true);
            questPopupUI.QuestPopupUIAccept(quest.GetTitle());

            if (onQuestGained != null)
            {
                onQuestGained();
            }
            if (onQuestListUpdated != null)
            {
                onQuestListUpdated();
            }

            
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest); //Get hold of our status using GetQuestStatus, and save it as a variable to use
            status.CompleteObjective(objective);

            if (status.IsComplete())
            {
                questPopupUI.gameObject.SetActive(true);
                questPopupUI.QuestPopupUIComplete(quest.GetTitle());
                if (questCompleteSound != null) questCompleteSound.Play();
                GiveReward(quest);
            }
            if (onQuestRemoved != null)
            {
                onQuestRemoved();
            }
            if (onQuestListUpdated != null)
            {
                Debug.Log("onQuestListUpdated");
                onQuestListUpdated();
            }


            string questCompleteString = "<br>Quest completed: " + quest.GetTitle() + ".";

            ChatBox chatBox = GameObject.FindGameObjectWithTag("Player").GetComponent<ChatBox>();
            chatBox.UpdateText(questCompleteString);
        }

        private void Update()
        {
            CompleteObjectivesByPredicates();
        }

        private void CompleteObjectivesByPredicates()
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.IsComplete()) continue;
                Quest quest = status.GetQuest();
                foreach (var objective in quest.GetObjectives())
                {
                    if (status.IsObjectiveComplete(objective.reference)) continue;
                    if (!objective.usesCondition) continue;
                    if (objective.completionCondition.Check(GetComponents<IPredicateEvaluator>()))
                    {
                        questPopupUI.gameObject.SetActive(true);
                        questPopupUI.QuestObjectivePopupUI(objective.description);
                        CompleteObjective(quest, objective.reference);
                    }
                }
            }
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }


        public bool HasInventoryItem(InventoryItem item)
        {
            return InventoryItem.GetFromID(item.itemID) != null;
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public QuestStatus GetQuestStatus(Quest quest) //We pass in a quest in order to get the QuestStatus of that quest.
        {
            foreach (QuestStatus status in statuses) //We loop through all statuses we have
            {
                if (status.GetQuest() == quest) //If we have the correct status
                {
                    return status; //Return status
                }
            }
            return null;
        }
        private void GiveReward(Quest quest)
        {
            foreach (var experienceReward in quest.GetExperienceReward())
            {
                Experience experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
                experience.GainExperience(experienceReward.experienceRewardAmount);
            }
            foreach (var reward in quest.GetRewards())
            {
               bool success = GetComponent<Inventory>().AddToFirstEmptySlotInventory(reward.item, reward.number);
               if (!success)
               {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
               }

            }
        }
        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) return;

            statuses.Clear();
            foreach (object objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));               
            }
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            switch (predicate)
            {
                case EPredicate.HasQuest:
                    return HasQuest(Quest.GetByName(parameters[0]));

                case EPredicate.CompletedQuest:
                    QuestStatus status = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (status == null) return false;
                    //return status.IsComplete();         
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();

                case EPredicate.HasNotCompletedQuest:
                    QuestStatus status2 = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (status2 == null) return false;
                    //return status.IsComplete();         
                    return !GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();

                case EPredicate.CompletedObjective:
                    QuestStatus teststatus = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (teststatus == null) return false;
                    return teststatus.IsObjectiveComplete(parameters[1]);

            }

            return null;
        }


    }
}
