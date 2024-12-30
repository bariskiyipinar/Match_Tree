using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GridManager : MonoBehaviour
{
    public GameObject[] tilePrefabs; // Þeker (meyve) prefablarý
    public int gridWidth = 6; // Izgara geniþliði
    public int gridHeight = 6; // Izgara yüksekliði

    private GameObject[,] gridArray; // Izgaradaki þekerlerin tutulduðu iki boyutlu dizi
    public int score; // Skor
    public TextMeshProUGUI scoree; // Skoru gösteren UI öðesi
    private AudioSource volume;
 

    private void Start()
    {
        gridArray = new GameObject[gridWidth, gridHeight]; // Izgara dizisini baþlat
        GenerateGrid(); // Izgarayý oluþtur
        volume= GetComponent<AudioSource>();
        
    }

    public void Update()
    {
        scoree.text = "Score:" + score; // Skoru güncelle
    }

    void GenerateGrid()
    {
        // Izgaranýn ortasýný bulmak için hesaplama
        float xOffset = (gridWidth - 1) / 2.0f;
        float yOffset = (gridHeight - 1) / 2.0f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int randomTile = Random.Range(0, tilePrefabs.Length); // Rastgele bir þeker seç
                Vector3 position = new Vector3(x - xOffset, y - yOffset, 0); // Þekerin pozisyonunu hesapla
                GameObject tile = Instantiate(tilePrefabs[randomTile], position, Quaternion.identity); // Þekeri yarat
                tile.transform.parent = this.transform; // Þekeri GridManager'ýn çocuðu yap
                gridArray[x, y] = tile; // Þekeri ýzgaraya ekle
            }
        }
    }

    public bool CanMove(GameObject tile, Vector2 direction)
    {
        Vector2 tilePosition = GetTilePosition(tile); // Þekerin mevcut pozisyonunu al
        Vector2 targetPosition = tilePosition + direction; // Hedef pozisyonu hesapla

        // Hedef pozisyon ýzgara sýnýrlarý içinde mi kontrol et
        if (targetPosition.x >= 0 && targetPosition.x < gridWidth && targetPosition.y >= 0 && targetPosition.y < gridHeight)
        {
            return true; // Geçerli bir hareket
        }
        return false; // Geçersiz bir hareket
    }

    public void SwapTiles(GameObject tile, Vector2 direction)
    {
        Vector2 tilePosition = GetTilePosition(tile); // Þekerin mevcut pozisyonunu al
        Vector2 targetPosition = tilePosition + direction; // Hedef pozisyonu hesapla

        // Hedef pozisyona hareket edebilir mi kontrolü
        if (CanMove(tile, direction))
        {
            GameObject targetTile = gridArray[(int)targetPosition.x, (int)targetPosition.y]; // Hedef pozisyondaki þekeri al
            gridArray[(int)tilePosition.x, (int)tilePosition.y] = targetTile; // Mevcut pozisyona hedef þekeri koy
            gridArray[(int)targetPosition.x, (int)targetPosition.y] = tile; // Hedef pozisyona mevcut þekeri koy

            Vector3 tempPosition = tile.transform.position; // Geçici olarak þekeri sakla
            tile.transform.position = targetTile.transform.position; // Mevcut þekeri hedef pozisyona taþý
            targetTile.transform.position = tempPosition; // Hedef þekeri mevcut pozisyona taþý

            StartCoroutine(CheckMatches()); // Eþleþmeleri kontrol et
        }
    }

    Vector2 GetTilePosition(GameObject tile)
    {
        // Þekerin mevcut pozisyonunu almak için ýzgarayý tarar
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
        return Vector2.zero; // Þeker bulunamazsa (normalde bu durumda olmamalý)
    }

    public IEnumerator CheckMatches()
    {
        yield return new WaitForSeconds(0.2f); // Hareket sonrasý biraz bekleyin

        List<GameObject> matches = new List<GameObject>(); // Eþleþen þekerlerin listesi

        // Yatay eþleþmeleri kontrol et
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

        // Dikey eþleþmeleri kontrol et
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

        // Eþleþen þekerleri yok et
        if (matches.Count > 0)
        {
            foreach (GameObject tile in matches)
            {
                Destroy(tile); // Þekeri yok et
                score += Random.Range(5, 10); // Skoru artýr
                volume.Play();
                
            }

            yield return new WaitForSeconds(0.5f); // Yok olma animasyonlarý için bekleyin

            FillGrid(); // Izgarayý doldur
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(CheckMatches()); // Yeni eþleþmeler için tekrar kontrol et
        }
    }

    void FillGrid()
    {
        // Boþ hücreleri doldur
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridArray[x, y] == null)
                {
                    int randomTile = Random.Range(0, tilePrefabs.Length); // Rastgele bir þeker seç
                    Vector3 position = new Vector3(x - (gridWidth - 1) / 2.0f, y - (gridHeight - 1) / 2.0f, 0); // Þekerin pozisyonunu hesapla
                    GameObject tile = Instantiate(tilePrefabs[randomTile], position, Quaternion.identity); // Þekeri yarat
                    tile.transform.parent = this.transform; // Þekeri GridManager'ýn çocuðu yap
                    gridArray[x, y] = tile; // Þekeri ýzgaraya ekle
                }
            }
        }
    }
}
