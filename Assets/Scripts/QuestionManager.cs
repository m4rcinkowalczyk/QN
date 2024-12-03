using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionManager : MonoBehaviour
{
    // Klasa reprezentująca pojedyncze pytanie
    [System.Serializable]
    public class Question
    {
        public int id;        // ID pytania
        public string kid;    // ID kategorii
        public string pytanie; // Treść pytania
        public string trivia; // Ciekawostka
    }

    // Klasa opakowująca listę pytań
    [System.Serializable]
    public class QuestionList
    {
        public List<Question> questions;
    }

    private string apiUrl = "https://quizapp2.bluesfera.pl/pytania.php"; // Adres API
    private string fileName = "questions.json"; // Nazwa pliku do zapisu

    void Start()
    {
        StartCoroutine(FetchQuestions());
    }

    IEnumerator FetchQuestions()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        // Wysyłamy żądanie do serwera
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Błąd połączenia: {request.error}");
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Pobrane dane: {jsonResponse}");

            try
            {
                // Parsowanie JSON
                QuestionList questionList = JsonUtility.FromJson<QuestionList>(jsonResponse);

                if (questionList == null || questionList.questions == null || questionList.questions.Count == 0)
                {
                    Debug.LogWarning("Lista pytań jest pusta lub null.");
                }
                else
                {
                    Debug.Log($"Załadowano {questionList.questions.Count} pytań.");
                }

                // Zapis danych do pliku
                SaveToFile(questionList);

                // Odczyt danych z pliku
                QuestionList loadedQuestions = LoadFromFile();
                if (loadedQuestions != null && loadedQuestions.questions.Count > 0)
                {
                    foreach (var question in loadedQuestions.questions)
                    {
                        Debug.Log($"ID: {question.id}, Kategoria: {question.kid}, Pytanie: {question.pytanie}, Trivia: {question.trivia}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Błąd podczas przetwarzania danych JSON: {ex.Message}");
            }
        }
    }

    void SaveToFile(QuestionList data)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            string json = JsonUtility.ToJson(data, true); // Serializacja do JSON
            File.WriteAllText(path, json);
            Debug.Log($"Dane zapisane do pliku: {path}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Błąd podczas zapisywania do pliku: {ex.Message}");
        }
    }

    QuestionList LoadFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                Debug.Log($"Odczytany JSON: {json}");

                QuestionList questionList = JsonUtility.FromJson<QuestionList>(json);

                if (questionList == null || questionList.questions == null)
                {
                    Debug.LogWarning("Nie udało się sparsować JSON do QuestionList.");
                    return null;
                }

                return questionList;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Błąd podczas odczytu pliku: {ex.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"Plik {fileName} nie istnieje.");
            return new QuestionList { questions = new List<Question>() };
        }
    }
}
