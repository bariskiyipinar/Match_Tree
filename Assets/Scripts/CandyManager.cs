using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyManager : MonoBehaviour
{
    private Vector3 initialPosition; // �lk t�klama pozisyonu
    private Vector3 finalPosition; // B�rakma pozisyonu
    private float swipeAngle; // S�r�kleme a��s�
    private GridManager gridManager; // GridManager referans�
    private bool isSwapping = false; // �u anda de�i�tirme yap�l�p yap�lmad���n� kontrol eder
    private Vector2 swipeDirection; // S�r�kleme y�n�
    private const float swipeThreshold = 0.5f; // S�r�kleme e�i�i, �ok k���k bir de�er ayarlay�n

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>(); // GridManager'� bul
    }

    private void OnMouseDown()
    {
        if (isSwapping) return; // E�er �u anda bir de�i�tirme i�lemi yap�l�yorsa i�lem yapma
        initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // �lk t�klama pozisyonunu al
        initialPosition.z = 0; // Z pozisyonunu s�f�rla
    }

    private void OnMouseDrag()
    {
        if (isSwapping) return; // E�er �u anda bir de�i�tirme i�lemi yap�l�yorsa i�lem yapma
        finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // S�r�kleme pozisyonunu al
        finalPosition.z = 0; // Z pozisyonunu s�f�rla
        CalculateSwipeDirection(); // S�r�kleme y�n�n� hesapla
    }

    private void OnMouseUp()
    {
        if (isSwapping) return; // E�er �u anda bir de�i�tirme i�lemi yap�l�yorsa i�lem yapma
        if (Vector2.Distance(initialPosition, finalPosition) < swipeThreshold) return; // S�r�kleme e�i�i kontrol�

        MovePieces(); // Ta�lar� hareket ettir
    }

    void CalculateSwipeDirection()
    {
        swipeDirection = finalPosition - initialPosition; // S�r�kleme y�n�n� hesapla
        swipeAngle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg; // S�r�kleme a��s�n� hesapla
    }

    void MovePieces()
    {
        if (swipeDirection.magnitude > swipeThreshold) // S�r�kleme e�i�i kontrol�
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
        isSwapping = true; // De�i�tirme i�lemi ba�l�yor


        // Ta�lar� de�i�tir
        gridManager.SwapTiles(tile, direction);

        yield return new WaitForSeconds(0.5f); // De�i�tirme animasyonu i�in bekleyin

        // E�le�me kontrol� yap
        yield return StartCoroutine(gridManager.CheckMatches());

        isSwapping = false; // De�i�tirme i�lemi bitti
    }
}
