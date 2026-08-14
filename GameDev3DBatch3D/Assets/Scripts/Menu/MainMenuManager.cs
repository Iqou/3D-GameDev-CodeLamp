using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada untuk manajemen pindah Scene

public class MainMenuManager : MonoBehaviour
{
    // --- NAVIGASI SCENE SEMENTARA / DENGAN NAMA SCENE ---

    // Panggil fungsi ini dari Tombol "Mulai" di Main Menu
    public void GoToLevelSelect()
    {
        // Ganti "LevelSelect" sesuai NAMA EXACT scene level select kamu
        SceneManager.LoadScene("Level Select");
    }

    // Panggil fungsi ini dari Tombol "Level 1" di Level Select
    public void GoToGameplay()
    {
        // Ganti "Gameplay" sesuai NAMA EXACT scene gameplay kamu
        SceneManager.LoadScene("Gameplay");
    }


    // --- FLEKSIBEL: BISA DIPAKAI UNTUK TOMBOL LEVEL LAINNYA (LEVEL 2, 3, DST) ---

    // Fungsi fleksibel untuk pindah ke scene mana saja berdasarkan nama yang dimasukkan
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi untuk keluar dari game (Tombol Exit/Quit)
    public void QuitGame()
    {
        Debug.Log("Keluar dari Game...");
        Application.Quit();
    }
}