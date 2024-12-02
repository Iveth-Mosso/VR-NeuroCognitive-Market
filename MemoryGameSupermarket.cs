using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MemoryGameSupermarket : MonoBehaviour
{
    public GameObject[] objectsToSpawn;
    public int numberOfColumns = 3;
    public int numberOfRows = 2;
    public float countdownTime = 10f;  // Tiempo de la cuenta regresiva en segundos
    public int numberOfObjectsToKeepVisible = 3;
    public float separationDistanceX = 2f;
    public float separationDistanceY = 2f;
    public int totalObjectsToSpawn;
    public float initialPauseTime = 15f;

    public TMP_Text timerText;
    public Color warningColor = Color.red;
    public Color timerColor = Color.blue;
    public float warningTime = 3f;
    public AudioClip warningSound;
    public AudioClip startSound;
    private AudioSource audioSource;

    private float timer;
    private bool timerRunning = false;
    private bool warningSoundPlayed = false;
    private GameObject[] spawnedObjects;
    private HashSet<GameObject> spawnedObjectSet = new HashSet<GameObject>();
    public float xMem = 0f;
    public float yMem = 0f;
    public float zMem = 0f;

    public List<GameObject> selectedObjects = new List<GameObject>();

    public MemoryGameEvaluator evaluator; // Referencia al evaluador

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timerText.gameObject.SetActive(false);
        StartCoroutine(InitialPause());
    }

    void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            UpdateTimerText();
            if (timer <= 0)
            {
                timer = 0;
                timerRunning = false;
                HideObjectsAndText();
            }
        }
    }

    IEnumerator InitialPause()
    {
        yield return new WaitForSeconds(initialPauseTime);
        timer = countdownTime;
        timerText.gameObject.SetActive(true);
        timerRunning = true;

        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound);
        }

        SpawnAndDisplayAllObjects();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        //timerText.text = $"{minutes:00}:{seconds:00}";
        timerText.text = seconds.ToString();  // Muestra solo los segundos

        if (timer > warningTime)
        {
            timerText.color = timerColor;
        }
        else if (timer > 0f && timer <= warningTime)
        {
            timerText.color = warningColor;
            if (!warningSoundPlayed && audioSource != null && warningSound != null)
            {
                audioSource.PlayOneShot(warningSound);
                warningSoundPlayed = true;
            }
        }
    }

    void SpawnAndDisplayAllObjects()
    {
        // Crear una copia de la lista de prefabs para evitar repeticiones
        List<GameObject> availableObjects = new List<GameObject>(objectsToSpawn);

        spawnedObjects = new GameObject[totalObjectsToSpawn];
        float initialX = -((numberOfColumns - 1) * separationDistanceX) / 2;
        float initialY = -((numberOfRows - 1) * separationDistanceY) / 2;

        int objectIndex = 0;

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < numberOfColumns; col++)
            {
                if (objectIndex < totalObjectsToSpawn && availableObjects.Count > 0)
                {
                    // Selecciona un prefab único de la lista temporal
                    int randomIndex = Random.Range(0, availableObjects.Count);
                    GameObject prefab = availableObjects[randomIndex];

                    // Instancia el objeto y elimina el prefab de la lista temporal
                    GameObject obj = Instantiate(prefab);
                    availableObjects.RemoveAt(randomIndex);

                    // Asigna posición al objeto
                    float xPos = initialX + col * separationDistanceX + xMem;
                    float yPos = initialY + row * separationDistanceY + yMem;
                    float zPos = 0f + zMem;

                    obj.transform.position = new Vector3(xPos, yPos, zPos);
                    obj.SetActive(true);

                    // Añade el objeto instanciado a las estructuras de datos
                    spawnedObjects[objectIndex] = obj;
                    spawnedObjectSet.Add(obj);

                    Debug.Log($"Objeto {objectIndex} generado: {obj.name} en {xPos}, {yPos}, {zPos}");
                    objectIndex++;
                }
            }
        }

        // Seleccionar objetos aleatorios después de generarlos
        SelectRandomObjects();

        // Pasar la selección al evaluador
        if (evaluator != null)
        {
            evaluator.selectedObjects = new List<GameObject>(selectedObjects);
        }
    }


// Método para seleccionar objetos aleatorios
void SelectRandomObjects()
{
    selectedObjects.Clear();

    if (spawnedObjects.Length < numberOfObjectsToKeepVisible)
    {
        Debug.LogError("No hay suficientes objetos generados para seleccionar.");
        return;
    }

    for (int i = 0; i < numberOfObjectsToKeepVisible; i++)
    {
        GameObject randomObj;
        do
        {
            randomObj = spawnedObjects[Random.Range(0, spawnedObjects.Length)];
        } while (selectedObjects.Contains(randomObj));

        selectedObjects.Add(randomObj);
    }

    Debug.Log("Objetos seleccionados: " + selectedObjects.Count);
}


    void HideObjectsAndText()
    {
        // Oculta el texto del temporizador
        timerText.gameObject.SetActive(false);

        // Oculta todos los objetos generados
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // Llama al evaluador para verificar si los objetos seleccionados coinciden
        if (evaluator != null)
        {
            evaluator.EvaluateSelection();
        }
    }
}
