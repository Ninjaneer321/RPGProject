using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using UnityEngine;

public class QuestIconDisplay : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] QuestList playerQuestList = null;
    [SerializeField] QuestGiver npcQuestGiver = null;

    [SerializeField] List<QuestGiver> questGiverList = new List<QuestGiver>();
    [SerializeField] GameObject exclaimationPointGold = null;
    [SerializeField] GameObject questionMarkGrey = null;
    [SerializeField] GameObject questionMarkGold = null;

    [SerializeField] List<Quest> quests = new List<Quest>();


    void Start()
    {
        playerQuestList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
        npcQuestGiver = gameObject.GetComponentInParent<QuestGiver>();



        questGiverList.AddRange(npcQuestGiver.GetComponents<QuestGiver>());
        GetQuestsByNPC();
        ShowExclaimationPoint();
        ShowGoldQuestionMark();

        //ADDED THIS THINKING THAT THIS WILL FIX THE GOLD ! AND GREY ? RACE CONDITION PROBLEM
        ShowGreyQuestionMark();

        playerQuestList.onQuestGained += HideExclaimationPoint;

        playerQuestList.onQuestListUpdated += ShowExclaimationPoint;
        playerQuestList.onQuestListUpdated += ShowGoldQuestionMark;
        playerQuestList.onQuestListUpdated += ShowGreyQuestionMark;

        //playerQuestList.onQuestListUpdated += ShowGoldQuestionMark;
        //playerQuestList.onQuestListUpdated += ShowExclaimationPoint;


        playerQuestList.onQuestRemoved += HideGoldQuestionMark;



        //HideExclaimationPoint();
        //DISABLE ALL GAMEOBJECTS HERE? OR DISABLE THEM INSIDE THE INSPECTOR.

    }

    //THIS RETURNS A LIST OF QUESTS ON AN NPC THAT COULD POTENTIALLY BE USED TO DRIVE THE VISIBILITY OF THE QUEST ICONS
    private List<Quest> GetQuestsByNPC()
    {
        foreach (QuestGiver questGiverComponent in questGiverList)
        {
            Quest quest = questGiverComponent.GetQuestForQuestIconDisplay();
            quests.Add(quest);
        }
        return quests;
    }

    private void ShowExclaimationPoint()
    {
        //If the player DOES NOT have the quest from the NPC giver
        //Maybe somehow try to figure out a way to do a foreach-style loop for every Quest Giver component on the NPC (this is because our Quests are found on Quest Giver).
        //This is a problem when we have an NPC with more than one quest to give out.

        foreach (Quest quest in quests)
        {
            if (!playerQuestList.HasQuest(quest))
            {
                exclaimationPointGold.SetActive(true);
            }
        }
    }

    private void ShowGreyQuestionMark()
    {
        //Show Grey Question Mark to quest completion NPC when the player has a quest that does not have objectives met.
        //Probably -2 from the objective count, ALA the gold question mark method.
        //LESS THAN THE QUESTION MARK METHOD.


        foreach (QuestStatus status in playerQuestList.GetStatuses())
        {
            if (gameObject.GetComponentInParent<QuestCompletion>())
            {
                if (status.GetCompletedCount() < status.GetQuest().GetObjectiveCount() - 1)
                {
                    questionMarkGrey.SetActive(true);

                    //If the NPC has a quest available AND is an NPC that completes a quest, disable the exclaimation point.

                    if (exclaimationPointGold.activeSelf)
                    {
                        exclaimationPointGold.SetActive(false);
                    }
                    if (questionMarkGold.activeSelf)
                    {
                        questionMarkGold.SetActive(false);
                    }
                }
            }
        }

    }
    private void HideExclaimationPoint()
    {
        foreach (Quest quest in quests)
        {
            if (playerQuestList.HasQuest(quest))
            {
                exclaimationPointGold.SetActive(false);

                //IF NPC IS A QUEST GIVER AND STILL HAS A QUEST PLAYER DOES NOT HAVE, SHOW THE EXCLAIMATION POINT AGAIN
            }
        }
    }

    public void HideGoldQuestionMark()
    {
        questionMarkGold.SetActive(false);
        //IF PLAYER HAS COMPLETED THE QUEST(S) THAT THE QUESTGIVER CAN GIVE, REMOVE THE GOLD QUESTION MARK ICON
    }

    private void ShowGoldQuestionMark()
    {

        foreach (QuestStatus status in playerQuestList.GetStatuses())
        {
            if (gameObject.GetComponentInParent<QuestCompletion>())
            {
                if (status.GetCompletedCount() == status.GetQuest().GetObjectiveCount() - 1)
                {
                    questionMarkGold.SetActive(true);

                    //If the NPC has a quest available AND is an NPC that completes a quest, disable the exclaimation point.

                    if (exclaimationPointGold.activeSelf)
                    {
                        exclaimationPointGold.SetActive(false);
                    }

                }
            }
        }
    }



}
