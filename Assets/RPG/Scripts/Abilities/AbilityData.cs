using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Stats;
using Stats;
using UnityEngine;

public class AbilityData
{
    GameObject user;
    IEnumerable<GameObject> targets;
    Vector3 targetedPoint;

    public AbilityData(GameObject user)
    {
        this.user = user;
    }



    public IEnumerable<GameObject> GetTargets()
    {
        return targets;
    }

    public void SetTargets(IEnumerable<GameObject> targets)
    {
        this.targets = targets;
    }
    public Vector3 GetTargetedPoint()
    {
        return targetedPoint;
    }

    public void SetTargetedPoint(Vector3 targetedPoint)
    {
        this.targetedPoint = targetedPoint;
    }

    public GameObject GetUser()
    {
        return user;
    }
    public void StartCoroutine(IEnumerator coroutine)
    {
        user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
    }

    public void StopAllCoroutines()
    {
        user.GetComponent<MonoBehaviour>().StopAllCoroutines();
    }
}
