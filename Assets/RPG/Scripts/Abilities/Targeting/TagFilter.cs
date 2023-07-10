using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Filters/Tags", order = 0)]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] string tagToFilter = "";

        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            foreach (var filteredObject in objectsToFilter)
            {
                if (filteredObject.CompareTag(tagToFilter))
                {
                    yield return filteredObject;
                }
            }
        }
    }
}
