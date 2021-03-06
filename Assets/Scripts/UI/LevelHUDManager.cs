﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;

namespace KekeDreamLand
{
    /// <summary>
    /// Manager of the HUD. Display Life points and other value. 
    /// </summary>
    public class LevelHUDManager : MonoBehaviour
    {
        #region Inspector attributes
        
        [Header("Lifepoints :")]
        
        public Transform lifePointsParent;
        public GameObject lifePointSprite;

        [Header("Collectables :")]
        public GameObject featherParent;
        public TextMeshProUGUI featherText;

        [Header("Special items :")]

        public Image[] specialItemSprites = new Image[4];
        public Color notFoundColor;

        [Space]

        public GameObject pausePanel;

        #endregion

        #region Private attributes

        private Animator hudAnimator;
        private bool displayed;

        private List<GameObject> lifePointsSprites = new List<GameObject>();

        private Sprite keySprite;

        #endregion

        #region Unity methods

        void Awake()
        {
            hudAnimator = GetComponent<Animator>();

            displayed = true;
            keySprite = specialItemSprites[0].sprite;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Display or undisplay HUD.
        /// </summary>
        public void ToggleHUD()
        {
            // Prevent spam.

            if (displayed)
            {
                hudAnimator.SetTrigger("MoveUp");
                displayed = false;
            }

            else
            {
                hudAnimator.SetTrigger("MoveDown");
                displayed = true;
            }
        }

        /// <summary>
        /// Update the amount of lifepoints sprites in the HUD.
        /// </summary>
        /// <param name="lifePoints">New amount</param>
        public void UpdateLifePoints(int lifePoints)
        {
            int spriteDiff = Mathf.Abs(lifePointsSprites.Count - lifePoints);

            // Remove sprites from the lifepoints HUD.
            if (lifePoints < lifePointsSprites.Count)
            {
                for (int i = 0; i < spriteDiff; i++)
                {
                    //GameObject spriteRemoved = lifePointsSprites[0];
                    // TODO trigger animation ?

                    Destroy(lifePointsSprites[0]);
                    lifePointsSprites.RemoveAt(0);
                }
            }

            // Add sprites to the lifepoints HUD.
            else
            {
                for (int i = 0; i < spriteDiff; i++)
                {
                    GameObject sprite = Instantiate(lifePointSprite, lifePointsParent);
                    lifePointsSprites.Add(sprite);
                }
            }

            // TODO add animation or effect.
        }

        /// <summary>
        /// Setup the feather indicators on the level HUD.
        /// </summary>
        /// <param name="count">Total number of collectable feathers.</param>
        public void SetupFeatherIndicators(int count)
        {
            if (count == 0)
            {
                featherParent.SetActive(false);
                return;
            }

            featherText.text = 0 + " / " + count;
        }

        /// <summary>
        /// Update the amount of feather picked up.
        /// </summary>
        /// <param name="newAmount">new amount of collected feathers.</param>
        /// <param name="count">Total number of collectable feathers.</param>
        public void UpdateFeatherPickedUp(int newAmount, int count)
        {
            featherText.text = newAmount + " / " + count;
        }

        /// <summary>
        /// Display the specific item indicator or not.
        /// </summary>
        /// <param name="specialItem">Which special item indicator to display.</param>
        /// <param name="enabled">Displayed or not</param>
        public void SetupSpecificItem(int i, bool displayed)
        {
            specialItemSprites[i].gameObject.SetActive(displayed);
        }

        /// <summary>
        /// Unlock a specific item.
        /// </summary>
        /// <param name="specialItem"></param>
        public void UnlockSpecialItem(int i, bool unlocked)
        {
            // TODO trigger animation of Unlock ?

            if(unlocked)
                specialItemSprites[i].color = Color.white;
            else
                specialItemSprites[i].color = notFoundColor;
        }

        /// <summary>
        /// Display the treasure when he is found. Display the key if treasure is lost.
        /// </summary>
        /// <param name="treasure"></param>
        /// <param name="displayed"></param>
        public void DisplayTreasure(Sprite treasure, bool displayed)
        {
            if (displayed)
                specialItemSprites[0].sprite = treasure;
            else
                specialItemSprites[0].sprite = keySprite;
        }

        /// <summary>
        /// Display feedback of pause game.
        /// </summary>
        /// <param name="isGamePaused"></param>
        public void PauseGame(bool isGamePaused)
        {
            pausePanel.SetActive(isGamePaused);

            Reselect();
        }

        #endregion

        #region Button events

        public void OnClickSettings()
        {
            OpenSettings();
        }

        public void OnClickBackToWorldMap()
        {
            GameManager.instance.LoadWorldMap();
        }

        public void OnClickBackToMainMenu()
        {
            GameManager.instance.LoadMainMenu();
        }

        public void OnClickQuitGame()
        {
            GameManager.instance.QuitGame();
        }

        #endregion

        #region Private methods
        
        // Reselect first button if exists.
        private void Reselect()
        {
            Transform buttonsParent = pausePanel.transform.Find("Buttons");
            if (buttonsParent)
            {
                Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();

                foreach(Button b in buttons)
                {
                    if (b.interactable)
                    {
                        b.Select();
                        return;
                    }
                }
            }
        }

        private void OpenSettings()
        {
            // TODO settings panel.
        }

        #endregion
    }

}