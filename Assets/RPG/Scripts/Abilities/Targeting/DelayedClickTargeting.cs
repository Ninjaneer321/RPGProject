using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Abilities/Targeting/Delayed Click", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D cursorTexture;
        [SerializeField] float areaAffectRadius;
        [SerializeField] Vector2 cursorHotspot;
        [SerializeField] Transform targetingPrefab;

        Transform targetingPrefabInstance = null;
        [SerializeField] LayerMask layerMask;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            ThirdPersonController thirdPersonController = data.GetUser().GetComponent<ThirdPersonController>();
            thirdPersonController.StartCoroutine(Targeting(data, thirdPersonController, finished));
        }

        private IEnumerator Targeting(AbilityData data, ThirdPersonController thirdPersonController, Action finished)
        {
            if (targetingPrefabInstance == null)
            {
                targetingPrefabInstance = Instantiate(targetingPrefab);
            }
            else
            {
                targetingPrefabInstance.gameObject.SetActive(true);
            }
            targetingPrefabInstance.localScale = new Vector3(areaAffectRadius, areaAffectRadius, areaAffectRadius);

            while (true)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("Targeting stopped.");
                    yield break;
                }

                RaycastHit raycastHit;
                if (Physics.Raycast(ThirdPersonController.GetMouseRay(), out raycastHit, 1000, layerMask))
                {
                    Vector3 offSet = new Vector3(0, .25f, 0);
                    targetingPrefabInstance.position = raycastHit.point + offSet;
                    if (Input.GetMouseButtonDown(0))
                    {
                        targetingPrefabInstance.gameObject.SetActive(false);
                        data.SetTargets(GetEnemiesInRadius(raycastHit.point));
                        finished();
                        yield break;
                    }
                }
                //Run every frame;
                yield return null;
            }
        }

        private IEnumerable<GameObject> GetEnemiesInRadius(Vector3 point)
        {
            Collider[] colliders = Physics.OverlapSphere(point, areaAffectRadius / 2);
            
            foreach (var hit in colliders)
            {
                yield return hit.GetComponent<Collider>().gameObject;
            }
        }

        public override void EnemyStartTargeting(AbilityData data, Action finished)
        {
            throw new NotImplementedException();
        }
    }
}