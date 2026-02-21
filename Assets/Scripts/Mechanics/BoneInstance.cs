using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;


namespace Platformer.Mechanics
{
    /// <summary>
    /// This class contains the data required for implementing bone collection mechanics.
    /// It does not perform animation of the bone, this is handled in a batch by the 
    /// boneController in the scene.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BoneInstance : MonoBehaviour
    {
        public AudioClip boneCollectAudio;

        //unique index which is assigned by the boneController in a scene.
        internal int boneIndex = -1;
        internal BoneController controller;
        //active frame in animation, updated by the controller.
        internal int frame = 0;
        internal bool collected = false;

        void Awake()
        {
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //only exectue OnPlayerEnter if the player collides with this bone.
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) OnPlayerEnter(player);
        }

        void OnPlayerEnter(PlayerController player)
        {
            if (collected) return;
            //disable the gameObject and remove it from the controller update list.
            frame = 0;
            if (controller != null)
                collected = true;
            //send an event into the gameplay system to perform some behaviour.
            var ev = Schedule<PlayerBoneCollision>();
            ev.bone = this;
            ev.player = player;
        }
    }
}