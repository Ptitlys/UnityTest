using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    public int speed;
    public int life;
    public float attackCooldown;
    public float attackDuration;
    public int damage;
    public GameObject attackZone;
    public float invicibilityDuration;
}
