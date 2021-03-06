﻿using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// AreaEditor. Permit to place an area anywhere, resize it and display landmark. Place also automatically the side walls and the killzone of this area.
    /// </summary>
    public class ForcedScrollingArea : AreaEditor
    {
        #region Inspector attributes
        
        [Header("Forced Scrolling Area")]
        public GameObject forcedScrollingKillZone;
        public GameObject forcedScrollingBlockingWall;
        public float delayBeforeScrolling;
        public float scrollingSpeed = 1.0f;
        public Direction scrollingDirection;

        #endregion

        #region Private attributes

        private Vector2 cameraOffset;
        private bool scrollOn;
        public bool ScrollOn
        {
            get { return scrollOn; }
            set { scrollOn = value; }
        }

        #endregion

        #region Unity methods

        #endregion

        #region Public methods

        /// <summary>
        /// Setup positions and sizes of the kill wall and the blocking wall.
        /// </summary>
        /// <param name="cameraPos"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public void SetupMovingWalls(Vector2 cameraPos, float cameraWidth, float cameraHeight)
        {
            // Moving killzone/blokcing walls offset and size.
            float offsetX = 0.0f, offsetY = 0.0f;
            float sizeX = 1.0f, sizeY = 1.0f;

            // Determine size and offset of walls depending the scrolling direction.
            if (scrollingDirection == Direction.UP || scrollingDirection == Direction.DOWN)
            {
                offsetY = -cameraHeight / 2 - 0.5f;
                if (scrollingDirection == Direction.DOWN)
                    offsetY *= -1;

                sizeX = cameraWidth;
            }

            else
            {
                offsetX = -cameraWidth / 2 - 0.5f;
                if (scrollingDirection == Direction.LEFT)
                    offsetX *= -1;

                sizeY = cameraHeight;
            }
            
            // Modify walls.
            Vector2 cameraOffset = new Vector2(offsetX, offsetY);
            Vector2 wallSize = new Vector2(sizeX, sizeY);

            forcedScrollingKillZone.transform.position = (cameraOffset * 0.92f) + cameraPos;
            forcedScrollingKillZone.GetComponent<BoxCollider2D>().size = wallSize;
            
            forcedScrollingBlockingWall.transform.position = -(cameraOffset * 1.05f) + cameraPos;
            forcedScrollingBlockingWall.GetComponent<BoxCollider2D>().size = wallSize;

            // Attach walls to the main camera.
            AttachWallsTo(Camera.main.transform);
        }

        /// <summary>
        /// Return the distance between current position of the camera and the finale destination of scrolling.
        /// </summary>
        /// <param name="cameraWidth"></param>
        /// <param name="cameraHeight"></param>
        /// <returns></returns>
        public float GetDestinationDistance(float cameraWidth, float cameraHeight)
        {
            float destinationDistance = 0.0f;

            if (scrollingDirection == Direction.UP || scrollingDirection == Direction.DOWN)
                destinationDistance = area.raw - cameraHeight;
            else
                destinationDistance = area.column - cameraWidth;

            return destinationDistance;
        }

        // Start forced scrolling after a certain delay.
        public IEnumerator StartForcedScrollingWithDelay()
        {
            // Wait while the level is in a transition.
            yield return new WaitWhile(() => GameManager.instance.levelMgr.IsTransition);

            // Wait the configurated delay.
            yield return new WaitForSeconds(delayBeforeScrolling);

            scrollOn = true;
        }

        /// <summary>
        /// Attach the forced scrolling walls to the specified transform.
        /// </summary>
        /// <param name="t"></param>
        public void AttachWallsTo(Transform t)
        {
            forcedScrollingKillZone.transform.parent = t;
            forcedScrollingBlockingWall.transform.parent = t;
        }

        #endregion
    }

}