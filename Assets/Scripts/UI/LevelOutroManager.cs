﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KekeDreamLand
{
    /// <summary>
    /// Manage the informations displayed on a level outro.
    /// </summary>
    public class LevelOutroManager : MonoBehaviour
    {
        #region Inspector attributes
        
        [Header("Texts")]
        public TextMeshProUGUI levelNameText;
        public Text LevelCommentText;
        public Text bottomText;

        [Header("Items")]
        public GameObject feather;
        public GameObject[] items;

        [Header("Animators")]
        public Animator outroAnimator;
        public Animator boingAnim;

        #endregion

        #region Private attributes

        private Image treasureRenderer;
        private Sprite keySprite;

        #endregion

        private void Awake()
        {
            treasureRenderer = items[0].transform.Find("Sprite").GetComponent<Image>();
            keySprite = treasureRenderer.sprite;
        }

        #region Public methods

        public void UpdateLevelOutro()
        {
            // Check if level has been fully completed.
            if (GameManager.instance.levelMgr.HasCollectAllItems())
                LevelCommentText.text = "Completed";
            else
                LevelCommentText.text = "Finished";
        }

        /// <summary>
        /// Display the level outro step by step with an animation.
        /// </summary>
        public void DisplayStepByStep()
        {
            outroAnimator.SetTrigger("StepByStep");
        }

        /// <summary>
        /// Display the level outro directly.
        /// </summary>
        public void DisplayDirectly()
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }

        /// <summary>
        /// Display an another text when the saving has been completed.
        /// </summary>
        public void DisplaySaveComplete()
        {
            bottomText.text = "Press any key to continue.";
        }

        /// <summary>
        /// Update level numbers and level name in the level outro.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="levelName"></param>
        public void SetupLevelName(string levelNumber, string levelName)
        {
            string levelNumbers = levelNumber.Split(' ')[1];
            levelNameText.text = levelNumbers + " " + levelName;
        }

        /// <summary>
        /// Display the specific special item indicator or not.
        /// </summary>
        /// <param name="specialItem">Which special item indicator to display.</param>
        /// <param name="enabled">Displayed or not</param>
        public void SetupSpecificItem(int i, bool displayed)
        {
            items[i].SetActive(displayed);
        }

        /// <summary>
        /// Update if an item has been found or not.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="found"></param>
        public void UpdateSpecialItems(int i, bool found)
        {
            Text t = items[i].transform.GetChild(1).GetComponent<Text>();

            if (found)
                t.text = "OK";
            else
                t.text = "X";
        }

        /// <summary>
        /// Update amount of feathers collected.
        /// </summary>
        /// <param name="featherPickedUp"></param>
        /// <param name="featherCount"></param>
        public void UpdateFeatherPickedUp(int featherPickedUp, int featherCount)
        {
            feather.transform.GetChild(1).GetComponent<Text>().text = featherPickedUp + " / " + featherCount;
        }

        /// <summary>
        /// Display the treasure when it's got. Display the key if treasure is lost.
        /// </summary>
        /// <param name="treasure"></param>
        /// <param name="displayed"></param>
        public void UpdateTreasure(Sprite treasure, bool displayed)
        {
            if (displayed)
                treasureRenderer.sprite = treasure;
            else
                treasureRenderer.sprite = keySprite;
        }

        /// <summary>
        /// Event animation triggered when the level outro is totaly displayed.
        /// </summary>
        public void OnLevelOutroTotalyDisplayed()
        {
            GameManager.instance.levelMgr.IsDisplayLevelOutro = false; // user can switch to world map.
            GameManager.instance.levelMgr.IsLevelFinished = true;

            // Boing dance if level is totaly completed.
            if (GameManager.instance.levelMgr.HasCollectAllItems())
            {
                if (boingAnim)
                    boingAnim.SetTrigger("FullComplete");
                else
                    Debug.LogWarning("Level outro boing animator is not affected !");

                // Play level completed sound.
                // Cut music ? Fadeout ?
            }

            
            else
            {
                // Play level finished sound.
            }

            DisplaySaveComplete();
        }

        #endregion
    }
}
