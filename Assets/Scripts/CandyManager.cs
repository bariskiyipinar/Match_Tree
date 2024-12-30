using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyManager : MonoBehaviour
{
    private Vector3 initialPosition; // Ýlk týklama pozisyonu
    private Vector3 finalPosition; // Býrakma pozisyonu
    private float swipeAngle; // Sürükleme açýsý
    private GridManager gridManager; // GridManager referansý
    private bool isSwapping = false; // Þu anda deðiþtirme yapýlýp yapýlmadýðýný kontrol eder
    private Vector2 swipeDirection; // Sürükleme yönü
    private const float swipeThreshold = 0.5f; // Sürükleme eþiði, çok küçük bir deðer ayarlayýn

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>(); // GridManager'ý bul
    }

    private void OnMouseDown()
    {
        if (isSwapping) return; // Eðer þu anda bir deðiþtirme iþlemi yapýlýyorsa iþlem yapma
        initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Ýlk týklama pozisyonunu al
        initialPosition.z = 0; // Z pozisyonunu sýfýrla
    }

    private void OnMouseDrag()
    {
        if (isSwapping) return; // Eðer þu anda bir deðiþtirme iþlemi yapýlýyorsa iþlem yapma
        finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Sürükleme pozisyonunu al
        finalPosition.z = 0; // Z pozisyonunu sýfýrla
        CalculateSwipeDirection(); // Sürükleme yönünü hesapla
    }

    private void OnMouseUp()
    {
        if (isSwapping) return; // Eðer þu anda bir deðiþtirme iþlemi yapýlýyorsa iþlem yapma
        if (Vector2.Distance(initialPosition, finalPosition) < swipeThreshold) return; // Sürükleme eþiði kontrolü

        MovePieces(); // Taþlarý hareket ettir
    }

    void CalculateSwipeDirection()
    {
        swipeDirection = finalPosition - initialPosition; // Sürükleme yönünü hesapla
        swipeAngle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg; // Sürükleme açýsýný hesapla
    }

    void MovePieces()
    {
        if (swipeDirection.magnitude > swipeThreshold) // Sürükleme eþiði kontrolü
        {
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if (swipeDirection.x > 0 && gridManager.CanMove(this.gameObject, Vector2.right))
                {
                    StartCoroutine(SwapAndCheck(this.gameObject, Vector2.right));
                }
                else if (swipeDirection.x < 0 && gridManager.CanMove(this.gameObject, Vector2.left))
                {
                    StartCoroutine(SwapAndCheck(this.gameObject, Vector2.left));
                }
            }
            else
            {
                if (swipeDirection.y > 0 && gridManager.CanMove(this.gameObject, Vector2.up))
                {
                    StartCoroutine(SwapAndCheck(this.gameObject, Vector2.up));
                }
                else if (swipeDirection.y < 0 && gridManager.CanMove(this.gameObject, Vector2.down))
                {
                    StartCoroutine(SwapAndCheck(this.gameObject, Vector2.down));
                }
            }
        }
    }

    IEnumerator SwapAndCheck(GameObject tile, Vector2 direction)
    {
        isSwapping = true; // Deðiþtirme iþlemi baþlýyor


        // Taþlarý deðiþtir
        gridManager.SwapTiles(tile, direction);

        yield return new WaitForSeconds(0.5f); // Deðiþtirme animasyonu için bekleyin

        // Eþleþme kontrolü yap
        yield return StartCoroutine(gridManager.CheckMatches());

        isSwapping = false; // Deðiþtirme iþlemi bitti
    }
}
