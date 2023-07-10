using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using UnityEngine.SceneManagement;
using RPG.Dialogue;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string currentSaveKey = "currentSaveName";
        [SerializeField] float fadeInTime = 0.2f;
        [SerializeField] float fadeOutTime = 0.2f;
        [SerializeField] int firstLevelBuildIndex = 1;
        [SerializeField] int menuLevelBuildIndex = 0;

        public string currentPlayerName = "name";

        public void ContinueGame()
        {
            //if (!PlayerPrefs.HasKey(currentSaveKey)) return;
            //if (!GetComponent<SavingSystem>().SaveFileExists(GetCurrentSave())) return;
            StartCoroutine(LoadLastScene());
        }
        public void NewGame(string saveFile)
        {
            //if (!String.IsNullOrEmpty(saveFile)) return;
            SetCurrentSave(saveFile);
            //StartCoroutine(LoadFirstScene());
        }
        public void LoadGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuScene());
        }

        private void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(currentSaveKey, saveFile);
        }
        public string GetCurrentSaveNameForPlayer()
        {

            return PlayerPrefs.GetString(currentSaveKey);
        }
        private string GetCurrentSave()
        {

            return PlayerPrefs.GetString(currentSaveKey);
        }

        private IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());
            Load();
            yield return fader.FadeIn(fadeInTime);
        }
        //private IEnumerator LoadFirstScene()
        //{
        //    Fader fader = FindObjectOfType<Fader>();
        //    yield return fader.FadeOut(fadeOutTime);
        //    yield return SceneManager.LoadSceneAsync(firstLevelBuildIndex);
        //    yield return fader.FadeIn(fadeInTime);
        //}
        private IEnumerator LoadMenuScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(menuLevelBuildIndex);
            yield return fader.FadeIn(fadeInTime);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());
        }

        public void Load()
        {
           GetComponent<SavingSystem>().Load(GetCurrentSave());
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
        }
        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }
    }
}
