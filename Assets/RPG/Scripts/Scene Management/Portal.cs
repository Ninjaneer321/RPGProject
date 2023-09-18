using System.Collections;
using GameDevTV.Saving;
using RPG.Control;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Portal : MonoBehaviour, IRaycastable
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] AudioSource portalSound;
        [SerializeField] GameObject portalLookAt = null;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {

            if (other.tag == "Player")
            {
                StartCoroutine(PortalTransition());
            }
        }


        public bool HandleRaycast(PlayerManager playerManager)
        {
            if (Input.GetMouseButtonDown(1))
            {
                playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
                if (Vector3.Distance(this.transform.position, playerManager.transform.position) <= 2.5f)
                {
                    playerManager.GetComponent<Animator>().SetTrigger("openDoor");
                    if (portalLookAt != null) playerManager.transform.LookAt(this.transform, Vector3.up);
                    playerManager.transform.eulerAngles = new Vector3(0, playerManager.transform.eulerAngles.y, playerManager.transform.eulerAngles.z);
                    StartCoroutine(PortalTransition());
                }
            }
            return true;
        }


        private IEnumerator PortalTransition()
        {
            //STOP MUSIC WHEN TRANSITIONING AND/OR MAKE DIFFERENT SOUND

            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            PlayerManager playerController = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
            playerController.GetComponent<InputManager>().enabled = false;

            if (portalSound != null) portalSound.Play();

            yield return new WaitForSeconds(.5f);

            yield return fader.FadeOut(fadeOutTime);
            Camera.main.GetComponent<AudioListener>().enabled = false;
            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerManager newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
            newPlayerController.enabled = false;

            savingWrapper.Load();
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;
            Destroy(gameObject);
        }
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {

                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }
            return null;
        }

        public CursorType GetCursorType()
        {
            PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            if (Vector3.Distance(this.transform.position, playerManager.transform.position) <= 2.5f)
            {
                return CursorType.Door;
            }

            return CursorType.None;
        }

    }
}