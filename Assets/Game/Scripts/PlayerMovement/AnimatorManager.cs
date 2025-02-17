using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
   public Animator animator;
    int horizontalValue;
    int verticalValue;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalValue = Animator.StringToHash("Horizontal");
        verticalValue = Animator.StringToHash("Vertical");
    }
    public void ChangeAnimatorValue(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontalMovement;
        float snappedVerticalMovement;

        #region Snapped Horizontal
        //neu toc do chay lon  hon 0 va nho hon .55 thi animation la di(0.5)
        if(horizontalMovement >0 && horizontalMovement< 0.55f)
        {
            snappedHorizontalMovement = 0.5f;

        }
        //neu toc do chay lon hon .5 thi la chay nhanh 
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontalMovement = 1f;
        }
        else if(horizontalMovement<0 &&horizontalMovement > -0.55f)
        {
            snappedHorizontalMovement = -0.55f;

        }
        else if(horizontalMovement < -0.55f)
        {
            snappedHorizontalMovement = -1f;
        }
        else
        {
            snappedHorizontalMovement = 0f;
        }
        #endregion
        #region Snapped Vertical
        //neu toc do chay lon  hon 0 va nho hon .55 thi animation la di(0.5)
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVerticalMovement = 0.5f;

        }
        //neu toc do chay lon hon .5 thi la chay nhanh 
        else if (verticalMovement > 0.55f)
        {
            snappedVerticalMovement = 1f;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVerticalMovement = -0.55f;

        }
        else if (verticalMovement < -0.55f)
        {
            snappedVerticalMovement = -1f;
        }
        else
        {
            snappedVerticalMovement = 0f;
        }
        #endregion
        if (isSprinting)
        {
            snappedHorizontalMovement = horizontalMovement;
            snappedVerticalMovement = 2;
        }


        animator.SetFloat(horizontalValue, snappedHorizontalMovement, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalValue, snappedVerticalMovement, 0.1f, Time.deltaTime);
    }
    public void  PlayTargetAnim(string targetAnim, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnim, 0.2f);
    }
}
