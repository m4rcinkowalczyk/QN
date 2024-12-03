using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Jeśli używasz TextMeshPro

public class CategoryListView : MonoBehaviour
{
    // Referencja do Content w Scroll View
    public Transform contentPanel;

    // Prefab przycisku kategorii
    public GameObject categoryButtonPrefab;

    // Lista kategorii
    private List<CategoryManager.Category> categories;

    void Start()
    {
        // Wczytanie kategorii z pliku
        categories = LoadCategories();

        // Generowanie przycisków
        PopulateCategoryButtons();
    }

    List<CategoryManager.Category> LoadCategories()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "categories.json");

        // Sprawdź, czy plik istnieje
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError($"Plik {path} nie istnieje!");
            return new List<CategoryManager.Category>(); // Zwróć pustą listę
        }

        string json = System.IO.File.ReadAllText(path);
        Debug.Log($"Pobrany JSON: {json}");

        try
        {
            // Parsowanie JSON
            CategoryManager.CategoryList categoryList = JsonUtility.FromJson<CategoryManager.CategoryList>(json);

            // Sprawdź, czy JSON został poprawnie sparsowany
            if (categoryList == null || categoryList.categories == null)
            {
                Debug.LogError("Nie udało się sparsować JSON do CategoryList lub categories jest null.");
                return new List<CategoryManager.Category>();
            }

            Debug.Log($"Liczba załadowanych kategorii: {categoryList.categories.Count}");
            return categoryList.categories;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Błąd podczas parsowania JSON: {ex.Message}");
            return new List<CategoryManager.Category>(); // Zwróć pustą listę w przypadku błędu
        }
    }


    void PopulateCategoryButtons()
    {

        foreach (var category in categories)
        {



            // Instancjonowanie nowego przycisku
            GameObject newButton = Instantiate(categoryButtonPrefab, contentPanel);

            // Ustawienie nazwy kategorii
            TextMeshProUGUI textComponent = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = category.title;
            }

            // Dodanie listenera do przycisku
            Button button = newButton.GetComponent<Button>();
            string categoryId = category.kid; // Zamknięcie zmiennej w lambdzie
            button.onClick.AddListener(() => OnCategoryButtonClicked(categoryId));
        }
    }

    void OnCategoryButtonClicked(string categoryId)
    {
        Debug.Log("Kliknięto kategorię: " + categoryId);
        // Tutaj możesz zaimplementować przejście do pytań z danej kategorii
    }
}

