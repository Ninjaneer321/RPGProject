using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using Stats;
using UnityEngine;

public interface iModifierProvider
{
    IEnumerable<int> GetAdditiveModifiers(Stat stat);
    IEnumerable<int> GetPercentageModifiers(Stat stat);

}
