using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    public MonsterData data;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private float attackCooldownCounter;
    private bool isFacingRight = true;
    private GameObject player;
    private int life;
    public bool isTakingDamage;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameManager.instance.player;
        life = data.life;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, data.speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, player.transform.position) <= 1)
        {
            if (canAttack())
            {
                attackCooldownCounter = data.attackCooldown;
                _animator.SetTrigger("attack");
            }
        }
        else
        {
            _animator.SetBool("isRunning", true);
            if (needToBeFlipped())
            {
                flipX();
            }
        }
    }

    private bool needToBeFlipped()
    {
        return (player.transform.position.x > transform.position.x && !isFacingRight) || (player.transform.position.x < transform.position.x && isFacingRight);
    }

    private void flipX()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        isFacingRight = !isFacingRight;
    }

    private bool canAttack()
    {
        if (attackCooldownCounter > 0)
        {
            attackCooldownCounter -= Time.deltaTime;
            return false;
        }
        return true;
    }

    public IEnumerator attack()
    {
        GameObject attackZone = Instantiate(data.attackZone, transform.position, Quaternion.identity);
        attackZone.transform.SetParent(gameObject.transform);
        attackZone.transform.localScale = new Vector3(0.4f, 0.4f, 1);
        attackZone.transform.localPosition = new Vector3(0.373f, 0, 0);

        yield return new WaitForSeconds(data.attackCooldown);
        Destroy(attackZone);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(String.Format("Collision with {0}", other.name));
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit");
            PlayerBehavior player = other.GetComponent<PlayerBehavior>();
            if (player.isTakingDamage)
            {
                return;
            }
            StartCoroutine(
                player.takeDamage(data.damage)
                );
        }
    }

    public IEnumerator takeDamage(int damage)
    {
        isTakingDamage = true;
        life -= damage;
        _animator.SetTrigger("hurt");
        _spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(data.invicibilityDuration);
        _spriteRenderer.color = Color.white;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
        isTakingDamage = false;
    }
}
