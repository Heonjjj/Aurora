using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMotion : Motion //틀은 부모에 실제동작은 자식에
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Reasource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]  //공격이 가능한지
    public bool doseDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;

    private void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main;
    }

    //public override void OnAttackInput()
    //{
    //    if (!attacking)
    //    {
    //        if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
    //        {
    //            attacking = true;
    //            animator.SetTrigger("Attack");
    //            Invoke("OnCanAttack", attackRate);
    //        }
    //    }
    //}
    //
    //void OnCanAttack()
    //{
    //    attacking = false;
    //}
    //
    //public void OnHit()
    //{
    //    Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    //    RaycastHit hit;
    //
    //    if (Physics.Raycast(ray, out hit, attackDistance))
    //    {
    //        if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
    //        {
    //            resource.Gather(hit.point, hit.normal);
    //        }
    //    }
    //}

}
