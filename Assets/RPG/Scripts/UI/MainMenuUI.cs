using System;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Dialogue;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using SceneManagement;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        LazyValue<SavingWrapper> savingWrapper;

        [SerializeField] public TMP_InputField newGameNameField;


        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);

        }
        public void FindAndEnablePortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                portal.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
        }

        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }

        public void NewGame()
        {
            savingWrapper.value.NewGame(newGameNameField.text);
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}
