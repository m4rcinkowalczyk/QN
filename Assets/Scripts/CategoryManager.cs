using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CategoryManager : MonoBehaviour
{
    // Klasa reprezentująca pojedynczą kategorię
    [System.Serializable]
    public class Category
    {
        public int id;
        public string kid;
        public string title;
        public string subtitle;
    }

    // Klasa opakowująca listę kategorii
    [System.Serializable]
    public class CategoryList
    {
        public List<Category> categories;
    }

    private string apiUrl = "https://quizapp2.bluesfera.pl/kategorie.php"; // Adres API
    private string fileName = "categories.json"; // Nazwa pliku do zapisu

    void Start()
    {
        StartCoroutine(FetchCategories());
    }

    IEnumerator FetchCategories()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        // Wysyłamy żądanie do serwera
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Błąd połączenia: " + request.error);
        }
        else
        {
            // Pobieramy odpowiedź z serwera
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Pobrane dane: " + jsonResponse);

            // Tworzymy obiekt listy kategorii z odpowiedzi JSON
            try
            {
                CategoryList categoryList = JsonUtility.FromJson<CategoryList>(jsonResponse);

                if (categoryList.categories != null && categoryList.categories.Count > 0)
                {
                    Debug.Log($"Załadowano {categoryList.categories.Count} kategorii.");
                }
                else
                {
                    Debug.LogError("Lista kategorii jest pusta.");
                }

                // Zapisujemy dane do pliku JSON
                SaveToFile(categoryList);

                // Odczytujemy dane z pliku i wyświetlamy w konsoli
                CategoryList loadedCategories = LoadFromFile();
                if (loadedCategories != null && loadedCategories.categories != null)
                {
                    foreach (var category in loadedCategories.categories)
                    {
                        Debug.Log($"ID: {category.id}, Title: {category.title}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Błąd podczas parsowania JSON: {ex.Message}");
            }
        }
    }


    void SaveToFile(CategoryList data)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        // Serializujemy dane do JSON
        string json = JsonUtility.ToJson(data, true); // `true` dla czytelnego formatu
        Debug.Log($"JSON do zapisu: {json}");

        // Zapisujemy dane do pliku
        System.IO.File.WriteAllText(path, json);

        Debug.Log($"Dane zapisane do pliku: {path}");
    }



    CategoryList LoadFromFile()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            Debug.Log($"Odczytany JSON: {json}");

            // Parsowanie JSON
            CategoryList categoryList = JsonUtility.FromJson<CategoryList>(json);

            if (categoryList == null || categoryList.categories == null)
            {
                Debug.LogError("Nie udało się sparsować JSON do CategoryList.");
                return null;
            }

            return categoryList;
        }
        else
        {
            Debug.LogError($"Plik {path} nie istnieje.");
            return null;
        }
    }


}
