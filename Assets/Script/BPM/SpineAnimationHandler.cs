using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineAnimationHandler : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public RythmManager rhythmManager;


    // Références d'animation pour chaque état
    public AnimationReferenceAsset idleAnimation;
    public AnimationReferenceAsset walkingAnimation;
    public AnimationReferenceAsset backWalkingAnimation;
    public AnimationReferenceAsset attackFastAnimation;
    public AnimationReferenceAsset attackHeavyAnimation;
    public AnimationReferenceAsset parryAnimation;
    public AnimationReferenceAsset gimmickAnimation;
    public AnimationReferenceAsset redStateAnimation;
    public AnimationReferenceAsset deadStateAnimation;

    public AnimationStateType currentAnimationState = AnimationStateType.Idle;
    private bool loopAnimation = true;

    public enum AnimationStateType
    {
        Idle,
        Walking,
        BackWalking,
        AttackFast,
        AttackHeavy,
        Parry,
        Gimmick,
        RedState,
        DeadState
    }

    void Start()
    {
        // Vérification que les composants nécessaires sont assignés
        if (skeletonAnimation == null)
        {
            Debug.LogError("SkeletonAnimation n'est pas assigné.");
        }

        if (rhythmManager != null)
        {
            rhythmManager.OnBeat += PlaySyncedAnimation;
        }
        else
        {
            Debug.LogError("RhythmManager n'est pas assigné.");
        }

        skeletonAnimation.AnimationState.Event += HandleSpineEvent;
    }

    private void PlaySyncedAnimation()
    {
        AnimationReferenceAsset animationToPlay = GetAnimationForState(currentAnimationState);

        if (animationToPlay != null)
        {
            // Joue l'animation synchronisée avec le BPM
            skeletonAnimation.AnimationState.SetAnimation(0, animationToPlay, loopAnimation);
        }
    }

    private AnimationReferenceAsset GetAnimationForState(AnimationStateType state)
    {
        switch (state)
        {
            case AnimationStateType.Idle:
                return idleAnimation;
            case AnimationStateType.Walking:
                return walkingAnimation;
            case AnimationStateType.BackWalking:
                return backWalkingAnimation;
            case AnimationStateType.AttackFast:
                return attackFastAnimation;
            case AnimationStateType.AttackHeavy:
                return attackHeavyAnimation;
            case AnimationStateType.Parry:
                return parryAnimation;
            case AnimationStateType.Gimmick:
                return gimmickAnimation;
            case AnimationStateType.RedState:
                return redStateAnimation;
            case AnimationStateType.DeadState:
                return deadStateAnimation;
            default:
                return null;
        }
    }

    private void HandleSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "PlayNote")
        {
            Debug.Log("L'événement PlayNote a été déclenché dans Spine !");
            // Action spécifique à synchroniser avec le rythme ou l'animation
        }
    }
}
