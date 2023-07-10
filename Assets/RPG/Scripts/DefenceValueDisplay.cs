using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using Stats;
using TMPro;
using UnityEngine;

public class DefenceValueDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI defenceValue = null;

    BaseStats baseStats;
    // Start is called before the first frame update
    void Start()
    {
        baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
    }

    // Update is called once per frame
    void Update()
    {
        defenceValue.text = baseStats.GetStat(Stat.Defence).ToString();
    }
}
