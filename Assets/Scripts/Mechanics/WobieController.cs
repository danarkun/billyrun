using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class WobieController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        public float maxSpeed = 7;

        internal PatrolPath.Mover mover;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        private Vector3 lastPosition;

        public Bounds Bounds => _collider.bounds;

        void Start()
        {
            lastPosition = transform.position;
        }

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerWobieCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            // 1. Calculate the movement direction
            float movementDelta = transform.position.x - lastPosition.x;

            // 2. If moving right, flipX is false. If moving left, flipX is true.
            if (movementDelta > 0.001f)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementDelta < -0.001f)
            {
                spriteRenderer.flipX = false;
            }

            lastPosition = transform.position;
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(maxSpeed * 0.5f);
                transform.position = mover.Position;
            }
        }
    }
}