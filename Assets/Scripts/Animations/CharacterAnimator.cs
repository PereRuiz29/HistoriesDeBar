using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum characterState
{
    character_idle,
    character_walkRight,
    character_walkLeft
}


public class CharacterAnimator : MonoBehaviour
{
    Animator animator;
    private characterState currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimationState(characterState newState)
    {
        //stop the same animation from interrupting itself
        if (currentState == newState)
            return;
        
        animator.Play(newState.ToString());
        currentState = newState;
    }
}
