﻿using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// The mob which have this script move along the platform. If he reach an extremity he stops, flips and restart to move.
    /// 
    /// Based on hitbox bounds.
    /// </summary>
    public class MoveAlongThePlatform : MonoBehaviour
    {
        #region Inspector attributes

        public float speed = 0.5f;
        public bool moveToRightFirst;

        #endregion

        #region Private attributes

        private BoxCollider2D hitbox;
        private SpriteRenderer spriteRenderer;

        // Direction to move.
        private float direction;

        // Ray information.
        private Vector3 rayOrigin;
        private float rayOriginX;
        private float rayLength = 0.2f;

        #endregion

        #region Unity methods

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            hitbox = GetComponent<BoxCollider2D>();

            direction = moveToRightFirst ? 1.0f : -1.0f;
            // Flip the sprite if necessary.
            if (direction > 0)
                spriteRenderer.flipX = true;
        }

        private void FixedUpdate()
        {
            CheckPlatformExtremity();

            Move();
        }

        #endregion

        #region Private methods

        private void CheckPlatformExtremity()
        {
            // Determine ray origin.
            if (direction > 0)
                rayOriginX = hitbox.bounds.max.x;
            else
                rayOriginX = hitbox.bounds.min.x;

            rayOrigin = new Vector3(rayOriginX, hitbox.bounds.min.y - 0.01f, hitbox.bounds.min.z);

            // Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red, 0.1f);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength);

            if (!hit || hit.collider && hit.collider.tag == "OutOfBound")
            {
                Flip();
            }
        }

        private void Move()
        {
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
        }

        private void Flip()
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            direction *= -1;
        }

        #endregion
    }
}