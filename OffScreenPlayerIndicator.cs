using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenPlayerIndicator : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    [Header("Target/Follow")]
    public bool isTargetFollow = true;
    private GameObject[] targetObjects;
    public GameObject markerPrefab;
    private Dictionary<GameObject, GameObject> markerIcons = new Dictionary<GameObject, GameObject>();

    [Header("Sprites")]
    public Sprite squareSprite;
    public Sprite arrowSprite;





    void Start()
    {
        TargetFind();
    }





    void Update()
    {
        if (isTargetFollow)
        {
            foreach (var targetObject in targetObjects)
            {
                if (targetObject && mainCamera)
                {
                    GameObject markerIcon;
                    if (markerIcons.TryGetValue(targetObject, out markerIcon))
                    {
                        TargetFollow(targetObject, markerIcon);
                    }
                    else
                    {
                        // Hedef nesne için marker ikonu yoksa oluştur
                        markerIcon = Instantiate(markerPrefab, FindObjectOfType<Canvas>().transform);
                        markerIcons.Add(targetObject, markerIcon);
                    }
                }
                else
                {
                    Debug.LogError("Hedef Nesne veya Ana Kamera bağlı değil.");
                }
            }

            // Yok olan hedef nesnelerin marker ikonlarını temizle
            List<GameObject> targetsToRemove = new List<GameObject>();
            foreach (var pair in markerIcons)
            {
                if (!pair.Key || pair.Key.activeSelf == false)
                {
                    Destroy(pair.Value);
                    targetsToRemove.Add(pair.Key);
                }
            }
            foreach (var targetToRemove in targetsToRemove)
            {
                markerIcons.Remove(targetToRemove);
            }
        }
    }




    // Düşmanları bul
    void TargetFind()
    {
        targetObjects = GameObject.FindGameObjectsWithTag("Enemy");
    }




    // Hedefi takip et
    void TargetFollow(GameObject targetObject, GameObject markerIcon)
    {
        Vector3 positionOnScreen = mainCamera.WorldToScreenPoint(targetObject.transform.position);

        if (positionOnScreen.x < 0 || positionOnScreen.x > Screen.width || positionOnScreen.y < 0 || positionOnScreen.y > Screen.height)
        {
            Debug.Log("Alan dışında");
            // Alanın dışında ise ok resmi kullanılsın
            Image markerSprite = markerIcon.GetComponent<Image>();
            markerSprite.sprite = arrowSprite;

            Vector3 iconPositionDetermination = new Vector3(Mathf.Clamp(positionOnScreen.x, 0, Screen.width), Mathf.Clamp(positionOnScreen.y, 0, Screen.height), 0);
            markerIcon.transform.position = iconPositionDetermination;
            LookAtTarget(targetObject, markerIcon, true); // Hedefe bak
        }
        else
        {
            Debug.Log("Alan içinde");
            // Alanın içinde ise kutu resmi kullanılsın
            Image markerSprite = markerIcon.GetComponent<Image>();
            markerSprite.sprite = squareSprite;

            Vector3 iconPosition = new Vector3(positionOnScreen.x, positionOnScreen.y, 0);
            markerIcon.transform.position = iconPosition;
            LookAtTarget(targetObject, markerIcon, false); // Hedefe bakma
        }
    }




    // Hedefe bak
    void LookAtTarget(GameObject targetObject, GameObject markerIcon, bool isLookAtTarget)
    {
        if (isLookAtTarget)
        {
            Vector3 dirToTarget = (targetObject.transform.position - mainCamera.transform.position).normalized;
            float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
            markerIcon.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
        else
        {
            markerIcon.transform.rotation = Quaternion.identity;
        }
    }
}
