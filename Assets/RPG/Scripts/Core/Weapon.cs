using RPG.Stats;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace RPG.Core
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit = null;
        [SerializeField] UnityEvent onSwing = null;
        public void OnHit()
        {
            onHit.Invoke();
            //print("Weapon Hit " + gameObject.name);
        }

        public void OnSwing()
        {
            onSwing.Invoke();
        }

    }

}