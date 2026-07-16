using FPS.Player;
using FPS.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPS.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private UIManager uiManager;

        private bool gameOver;

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.OnDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.OnDeath -= HandlePlayerDeath;
        }

        private void Start()
        {
            Time.timeScale = 1f;

            if (playerHealth == null)
            {
                Debug.LogError(
                    "GameManager no tiene asignado PlayerHealth.",
                    this
                );
            }

            if (uiManager == null)
            {
                Debug.LogError(
                    "GameManager no tiene asignado UIManager.",
                    this
                );
            }
        }

        private void HandlePlayerDeath(PlayerHealth player)
        {
            if (gameOver)
                return;

            gameOver = true;

            Debug.Log("GameManager recibió la muerte del jugador.");

            uiManager?.ShowLose();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;

            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}