using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutGameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] public  FruitSpritesData fruitsList;
    [SerializeField] public Image fruitImagePrefab;
    [SerializeField] public Transform fruitImageHolder;
    [Header("References")]
    [SerializeField] public CutGameEndScreen endScreen;
    [SerializeField] public Transform objectsHolder;
    [SerializeField] public GameobjectCutter cutter;
    [SerializeField] public WeightCalculator calculator;
    [Header("Precision")]
    [SerializeField] public Scales precisionText;
    [SerializeField] public Color loseColor;
    [SerializeField] public Color winColor;
    [Header("Parameters")]
    public int totalFruitsToSpawn = 5;
    public float precision = 10f;

    private float delay = 1f;
    public int spawnedFruits = 0;

    public const float delayForEndScreen = 3f;

    public DataManager dataManager;

    private List<GameObject> fruits = new List<GameObject>();
    private List<Image> fruitImages = new List<Image>();

    public void Start()
    {
        //dataManager = DataManager.Instance;
        //totalFruitsToSpawn = dataManager.GetLevel(Attribute.Neutrality);

        //cutter.Init(OnCut);
        //cutter.EnableCutting();
        //SetUpGame();
        //SetUpFruits();
        //SpawnNewObject();
    }

    public void SetUpGame()
    {
        precisionText.UpdateScales(precision);
        //precisionText.SetColor(winColor);
    }

    public void OnCut()
    {
        UpdatePrecisionColor();
        if (spawnedFruits < totalFruitsToSpawn)
        {
            ResetCutting();
        }
        else
        {
            RemoveImage();
            CheckWinCondiiton();
        }
    }

    private void UpdatePrecisionColor()
    {
        if (calculator.GetWeightDifference() <= precision)
            precisionText.SetColor(winColor);
        else
            precisionText.SetColor(loseColor);
    }

    public void CheckWinCondiiton()
    {
        if (calculator.GetWeightDifference() <= precision)
        {
            StartCoroutine(Win());
        }
        else
        {
            StartCoroutine(Lose());
        }
    }

    private IEnumerator Win()
    {
        yield return new WaitForSeconds(delayForEndScreen);
        Debug.Log("You Won");
        dataManager.AddLevel(Attribute.Neutrality);
        endScreen.ShowWinScreen();
    }

    private IEnumerator Lose()
    {
        yield return new WaitForSeconds(delayForEndScreen);
        Debug.Log("You Lost");
        endScreen.ShowLoseScreen();
    }

    private void ResetCutting()
    {
        StartCoroutine(DelayedSpawnNewObject());
    }

    IEnumerator DelayedSpawnNewObject()
    {
        yield return new WaitForSeconds(delay);
        cutter.EnableCutting();
        RemoveImage();
        SpawnNewObject();
    }

    private void SpawnNewObject()
    {
        fruits[fruits.Count - 1].gameObject.SetActive(true);
        fruits.RemoveAt(fruits.Count - 1);
        spawnedFruits++;
    }

    private void RemoveImage()
    {
        fruitImages[fruitImages.Count - 1].gameObject.SetActive(false);
        fruitImages.RemoveAt(fruitImages.Count - 1);
    }

    private void SetUpFruits()
    {
        for (int i = 0; i < totalFruitsToSpawn; i++)
        {
            int index = Random.Range(0, fruitsList.fruit.Length);
            GameObject newFruit = Instantiate(fruitsList.fruit[index].Prefab, objectsHolder);
            newFruit.SetActive(false);
            fruits.Add(newFruit);

            Image fruitImage = Instantiate(fruitImagePrefab, fruitImageHolder);
            fruitImage.sprite = fruitsList.fruit[index].Image;
            fruitImages.Add(fruitImage);
            fruitImage.transform.SetAsFirstSibling();
        }
    }
}
