using UnityEngine;

namespace DeadFusion.GameCore
{
    public class Viewmodel : MonoBehaviour
    {
        public Vector3 pivotPosition;
        public Animator animator;
        public float fov;
        [HideInInspector]

        public void PlayAudio(AnimationEvent animationEvent)
        {
            //PLAY AUDIO
            //audioSources.PlayAudio(animationEvent.stringParameter, animationEvent.intParameter == 1);
        }
    }
}