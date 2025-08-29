using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EndingPlayer1 : MonoBehaviour
{
    public GameObject player2;
    private EndingPlayer2 player2Script;
    Animator animator;
    private Rigidbody rb;

    public Vector3 targetPosition;

    public GameObject chocolate;

    public GameObject UI;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        player2Script = player2.GetComponent<EndingPlayer2>();
    }

    public void Present()
    {
        animator.Play("Player1_Present");
        StartCoroutine(Chocolate());
    }

    public void Shame()
    {
        animator.Play("Player1_Shame");
    }

    public void HandsUp()
    {
        animator.Play("Player1_HandsUp");
    }

    public void JoyfulJump()
    {
        animator.Play("Player1_JoyfulJump");
    }

    public void Walk()
    {
        animator.Play("Player1_Walk");
    }

    public void Hug()
    {
        SetTargetTransform(new Vector3(-10,-7,6));
        animator.Play("Player1_Hug");
    }

    public void HugEnd()
    {
        player2Script.Surprise();
    }

    public void Backwalk()
    {
        animator.Play("Player1_Backwalk");
    }

    public void BackwalkEnd()
    {
        player2Script.Idle3();
    }

    public void Okay()
    {
        animator.Play("Player1_Okay");
    }
    public void OkayEnd()
    {
        player2Script.Walk();
    }

    public void RightTurn()
    {
        animator.Play("Player1_RightTurn");
    }

    public void RightTurn2()
    {
        animator.Play("Player1_RightTurn2");
    }

    public void Shame2()
    {
        animator.Play("Player1_Shame2");
    }

    public void Walk2()
    {
        animator.Play("Player1_Walk2");
    }

    public void Walk3()
    {
        animator.Play("Player1_Walk3");
    }

    public void Dance()
    {
        animator.Play("Player1_Dance");
    }

    public void Nice()
    {
        animator.Play("Player1_Nice");
    }

    public void SetPanel()
    {
        UI.SetActive(true);
    }

    void OnAnimatorMove()
    {
        // 애니메이션 위치 변화량을 가져옴
        Vector3 deltaPosition = animator.deltaPosition;
        Quaternion deltaRotation = animator.deltaRotation;

        // 위치 변화량을 캐릭터의 실제 위치에 적용
        if (rb != null)
        {
            rb.MovePosition(rb.position + deltaPosition);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    public void SetTargetTransform(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    IEnumerator Chocolate()
    {
        yield return new WaitForSeconds(1.2f);
        chocolate.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        chocolate.SetActive(false);
    }
}