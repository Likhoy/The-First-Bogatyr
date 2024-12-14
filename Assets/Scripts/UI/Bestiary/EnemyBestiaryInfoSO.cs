using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Bestiary/Enemy")]
public class EnemyBestiaryInfoSO : BestiaryCreatureInfoSO
{
    public List<Vulnerability> Vulnerabilities;
}

