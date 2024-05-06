using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    public int speed;
    public float attackCooldown;
    public float attackDuration;
    public int life;
    public int damage;
    public GameObject[] attackZones;
    public float invicibilityDuration;

}
