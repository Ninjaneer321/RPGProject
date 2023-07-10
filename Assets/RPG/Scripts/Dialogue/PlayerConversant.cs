using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.SceneManagement;
using RPG.UI;
using TMPro;
using UnityEngine;

namespace RPG.Dialogue
{

    public class PlayerConversant : MonoBehaviour, ISaveable
    {
        [SerializeField] string playerName;
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        [SerializeField] AIConversant currentConversant = null;
        bool isChoosing = false;

        [SerializeField] DialogueUI dialogueUI;

        public event Action onConversationUpdated;
        public void Update()
        {
            CheckForDistance();
        }


        private void CheckForDistance()
        {
            if (currentDialogue != null)
            {
                if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, currentConversant.gameObject.transform.position) >= 6f)
                {
                    currentDialogue = null;
                    currentNode = null;
                    isChoosing = false;
                    currentConversant.isActive = false;
                    currentConversant = null;
                    if (onConversationUpdated != null)
                    {
                        onConversationUpdated();
                    }

                }
            }
        }


        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentConversant.isActive = true;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            if (onConversationUpdated != null)
            {
                onConversationUpdated();
            }
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversant.isActive = false;
            currentConversant = null;
            if (onConversationUpdated != null)
            {
                onConversationUpdated();
            }
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }
            return currentNode.GetText();

        }
        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
            }
        }


        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                if (onConversationUpdated != null)
                {
                    onConversationUpdated();
                }
                return;
            }

            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();

            if (children.Length < 1)
            {
                Quit();
            }
            else if (children.Length >= 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, children.Count());
                TriggerExitAction();
                currentNode = children[randomIndex];
                TriggerEnterAction();
                if (onConversationUpdated != null)
                {
                    onConversationUpdated();
                }
            }

        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNodes)
        {
            foreach (var node in inputNodes)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }


        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        public object CaptureState()
        {
            //call to SavingWrapper's currentPlayerName to use as player name (GetCurrentSave method? It returns a string.

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            playerName = savingWrapper.GetCurrentSaveNameForPlayer();

            return playerName;
        }

        public void RestoreState(object state)
        {
            playerName = (string)state;
        }
    }
}
