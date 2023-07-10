using RPG.Stats;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace RPG.Core
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit = null;
        public void OnHit()
        {
            onHit.Invoke();
            //print("Weapon Hit " + gameObject.name);
        }

    }

}