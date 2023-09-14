using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] AIConversant aiConversant;
        [SerializeField] AudioSource audioSource;

        [SerializeField] TextMeshProUGUI AIText;
        //[SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] GameObject AIResponse;
        [SerializeField] TextMeshProUGUI currentSpeaker;

        //Scrolling Text Information


        [SerializeField] private TextMeshProUGUI itemInfoText;
        [SerializeField] private float textSpeed = 0.01f;


        public IEnumerator AnimateText()
        {
            audioSource.mute = false;
            for (int i = 0; i < playerConversant.GetText().Length + 1; i++)
            {
                //nextButton.gameObject.SetActive(false);
                itemInfoText.text = playerConversant.GetText().Substring(0, i);
                if (itemInfoText.text != "") //Trying to remove the sound from playing if the letter is essentially blank (a space)
                {
                    audioSource.Play();
                }
                yield return new WaitForSeconds(textSpeed);
            }
            //Right now this goes immediately to the Next() method which replaces the NPC textbox with the Player textbox. Remove the player
            //textbox and replicate the player responses at the end of the NPC textbox within the NPC textbox.
            Next();
            //nextButton.gameObject.SetActive(true);


            //Enable the nextButton or quitButton only when finished.
            //Sound effects for the letters displaying.
        }


        // Start is called before the first frame update
        void Start()
        {
           playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();

 
            //  USE THIS WHEN WE BEGIN TO TALK TO SOMEONE????
           //aiConversant = GameObject.FindGameObjectWithTag("NPC").GetComponent<AIConversant>();


           audioSource = GetComponent<AudioSource>();
           playerConversant.onConversationUpdated += UpdateUI;
           //nextButton.onClick.AddListener(Next);
           quitButton.onClick.AddListener(Quit);
           UpdateUI();
        }


        void Next()
        {
            playerConversant.Next();
            audioSource.mute = true;
        }

        void Quit()
        {
            playerConversant.Quit();
        }

        // Update is called once per frame
        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            {
                return;
            }

            currentSpeaker.text = playerConversant.GetCurrentConversantName();
            //AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if(playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                StartCoroutine(AnimateText());
                //AIText.text = playerConversant.GetText();
                //nextButton.gameObject.SetActive(playerConversant.HasNext());    
            }
        
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }
            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComponent = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
