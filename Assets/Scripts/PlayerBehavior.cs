using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public PlayerData data;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector2 moveInput;
    private bool isFacingRight = true;
    private float attackCooldownCounter;
    private int life;
    public bool isTakingDamage;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        life = data.life;
        isTakingDamage = false;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (isRunning())
        {
            _animator.SetBool("isRunning", true);
            if (needToBeFlipped())
            {
                flipX();
            }
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }

        if (isAttacking() && canAttack())
        {
            attackCooldownCounter = data.attackCooldown;
            StartCoroutine(attack(0));
        }

        moveInput.Normalize();
        _rb.velocity = new Vector2(moveInput.x * data.speed, moveInput.y * data.speed);
    }

    private bool isRunning()
    {
        return moveInput.x != 0 || moveInput.y != 0;
    }

    private bool needToBeFlipped()
    {
        return (moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight);
    }

    private bool isAttacking()
    {
        return Input.GetKeyDown(KeyCode.Space);
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

    private IEnumerator attack(int Attacknumber)
    {
        _animator.SetTrigger("attack1");
        GameObject attackZone = Instantiate(data.attackZones[Attacknumber], transform.position, Quaternion.identity);
        attackZone.transform.SetParent(gameObject.transform);

        yield return new WaitForSeconds(data.attackDuration);
        Destroy(attackZone);
    }

    private void flipX()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        isFacingRight = !isFacingRight;
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
            GameManager.instance.GameOver();
        }
        isTakingDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterBehavior monster = other.GetComponent<MonsterBehavior>();
            if (monster.isTakingDamage)
            {
                return;
            }
            StartCoroutine(monster.takeDamage(data.damage));
        }
    }
}
