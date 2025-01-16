using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WandAnimator : MonoBehaviour
{
    Animator animator;
    public PlayerMove player;
    public SparkBolt primary;
    public Fireball altFire;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", player.isWalking);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isWalking", player.isWalking);
    }

    public void Fire()
    {
        animator.SetTrigger("Fire");
    }
    public void Bolt()
    {
        animator.SetTrigger("Bolt");
    }

    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }
}
