using UnityEngine;

public class UIInfiniteScroll : MonoBehaviour
{
    [SerializeField] private float speed = 200f; // Kecepatan gerak (piksel per detik)
    [SerializeField] private float resetPositionX = -1920f; // Posisi X saat gambar harus di-reset
    [SerializeField] private float startPositionX = 1920f; // Posisi X awal saat muncul kembali

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Geser gambar ke kiri setiap frame
        rectTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        // Jika posisi X sudah melewati batas kiri (resetPositionX)
        if (rectTransform.anchoredPosition.x <= resetPositionX)
        {
            // Kembalikan posisi ke sebelah kanan
            rectTransform.anchoredPosition = new Vector2(startPositionX, rectTransform.anchoredPosition.y);
        }
    }
}