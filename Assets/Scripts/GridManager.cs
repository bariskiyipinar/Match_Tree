using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GridManager : MonoBehaviour
{
    public GameObject[] tilePrefabs; // �eker (meyve) prefablar�
    public int gridWidth = 6; // Izgara geni�li�i
    public int gridHeight = 6; // Izgara y�ksekli�i

    private GameObject[,] gridArray; // Izgaradaki �ekerlerin tutuldu�u iki boyutlu dizi
    public int score; // Skor
    public TextMeshProUGUI scoree; // Skoru g�steren UI ��esi
    private AudioSource volume;
 

    private void Start()
    {
        gridArray = new GameObject[gridWidth, gridHeight]; // Izgara dizisini ba�lat
        GenerateGrid(); // Izgaray� olu�tur
        volume= GetComponent<AudioSource>();
        
    }

    public void Update()
    {
        scoree.text = "Score:" + score; // Skoru g�ncelle
    }

    void GenerateGrid()
    {
        // Izgaran�n ortas�n� bulmak i�in hesaplama
        float xOffset = (gridWidth - 1) / 2.0f;
        float yOffset = (gridHeight - 1) / 2.0f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int randomTile = Random.Range(0, tilePrefabs.Length); // Rastgele bir �eker se�
                Vector3 position = new Vector3(x - xOffset, y - yOffset, 0); // �ekerin pozisyonunu hesapla
                GameObject tile = Instantiate(tilePrefabs[randomTile], position, Quaternion.identity); // �ekeri yarat
                tile.transform.parent = this.transform; // �ekeri GridManager'�n �ocu�u yap
                gridArray[x, y] = tile; // �ekeri �zgaraya ekle
            }
        }
    }

    public bool CanMove(GameObject tile, Vector2 direction)
    {
        Vector2 tilePosition = GetTilePosition(tile); // �ekerin mevcut pozisyonunu al
        Vector2 targetPosition = tilePosition + direction; // Hedef pozisyonu hesapla

        // Hedef pozisyon �zgara s�n�rlar� i�inde mi kontrol et
        if (targetPosition.x >= 0 && targetPosition.x < gridWidth && targetPosition.y >= 0 && targetPosition.y < gridHeight)
        {
            return true; // Ge�erli bir hareket
        }
        return false; // Ge�ersiz bir hareket
    }

    public void SwapTiles(GameObject tile, Vector2 direction)
    {
        Vector2 tilePosition = GetTilePosition(tile); // �ekerin mevcut pozisyonunu al
        Vector2 targetPosition = tilePosition + direction; // Hedef pozisyonu hesapla

        // Hedef pozisyona hareket edebilir mi kontrol�
        if (CanMove(tile, direction))
        {
            GameObject targetTile = gridArray[(int)targetPosition.x, (int)targetPosition.y]; // Hedef pozisyondaki �ekeri al
            gridArray[(int)tilePosition.x, (int)tilePosition.y] = targetTile; // Mevcut pozisyona hedef �ekeri koy
            gridArray[(int)targetPosition.x, (int)targetPosition.y] = tile; // Hedef pozisyona mevcut �ekeri koy

            Vector3 tempPosition = tile.transform.position; // Ge�ici olarak �ekeri sakla
            tile.transform.position = targetTile.transform.position; // Mevcut �ekeri hedef pozisyona ta��
            targetTile.transform.position = tempPosition; // Hedef �ekeri mevcut pozisyona ta��

            StartCoroutine(CheckMatches()); // E�le�meleri kontrol et
        }
    }

    Vector2 GetTilePosition(GameObject tile)
    {
        // �ekerin mevcut pozisyonunu almak i�in �zgaray� tarar
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridArray[x, y] == tile)
                {
                    return new Vector2(x, y);
                }
            }
        }
        return Vector2.zero; // �eker bulunamazsa (normalde bu durumda olmamal�)
    }

    public IEnumerator CheckMatches()
    {
        yield return new WaitForSeconds(0.2f); // Hareket sonras� biraz bekleyin

        List<GameObject> matches = new List<GameObject>(); // E�le�en �ekerlerin listesi

        // Yatay e�le�meleri kontrol et
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth - 2; x++)
            {
                GameObject currentTile = gridArray[x, y];
                GameObject nextTile1 = gridArray[x + 1, y];
                GameObject nextTile2 = gridArray[x + 2, y];

                if (currentTile != null && nextTile1 != null && nextTile2 != null)
                {
                    if (currentTile.tag == nextTile1.tag && currentTile.tag == nextTile2.tag)
                    {
                        matches.Add(currentTile);
                        matches.Add(nextTile1);
                        matches.Add(nextTile2);
                    }
                }
            }
        }

        // Dikey e�le�meleri kontrol et
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight - 2; y++)
            {
                GameObject currentTile = gridArray[x, y];
                GameObject nextTile1 = gridArray[x, y + 1];
                GameObject nextTile2 = gridArray[x, y + 2];

                if (currentTile != null && nextTile1 != null && nextTile2 != null)
                {
                    if (currentTile.tag == nextTile1.tag && currentTile.tag == nextTile2.tag)
                    {
                        matches.Add(currentTile);
                        matches.Add(nextTile1);
                        matches.Add(nextTile2);
                    }
                }
            }
        }

        // E�le�en �ekerleri yok et
        if (matches.Count > 0)
        {
            foreach (GameObject tile in matches)
            {
                Destroy(tile); // �ekeri yok et
                score += Random.Range(5, 10); // Skoru art�r
                volume.Play();
                
            }

            yield return new WaitForSeconds(0.5f); // Yok olma animasyonlar� i�in bekleyin

            FillGrid(); // Izgaray� doldur
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(CheckMatches()); // Yeni e�le�meler i�in tekrar kontrol et
        }
    }

    void FillGrid()
    {
        // Bo� h�creleri doldur
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridArray[x, y] == null)
                {
                    int randomTile = Random.Range(0, tilePrefabs.Length); // Rastgele bir �eker se�
                    Vector3 position = new Vector3(x - (gridWidth - 1) / 2.0f, y - (gridHeight - 1) / 2.0f, 0); // �ekerin pozisyonunu hesapla
                    GameObject tile = Instantiate(tilePrefabs[randomTile], position, Quaternion.identity); // �ekeri yarat
                    tile.transform.parent = this.transform; // �ekeri GridManager'�n �ocu�u yap
                    gridArray[x, y] = tile; // �ekeri �zgaraya ekle
                }
            }
        }
    }
}
