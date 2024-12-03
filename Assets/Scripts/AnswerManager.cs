using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AnswerManager : MonoBehaviour
{
    // Klasa reprezentująca pojedynczą odpowiedź
    [System.Serializable]
    public class Answer
    {
        public int id;      // ID odpowiedzi
        public int pid;     // ID pytania, do którego odpowiedź należy
        public string text; // Treść odpowiedzi
    }

    // Klasa opakowująca listę odpowiedzi
    [System.Serializable]
    public class AnswerList
    {
        public List<Answer> answers;
    }

    private string apiUrl = "https://quizapp2.bluesfera.pl/odpowiedzi.php"; // Adres API
    private string fileName = "answers.json"; // Nazwa pliku do zapisu

    void Start()
    {
        StartCoroutine(FetchAnswers());
    }

    IEnumerator FetchAnswers()
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
                AnswerList answerList = JsonUtility.FromJson<AnswerList>(jsonResponse);

                if (answerList == null || answerList.answers == null || answerList.answers.Count == 0)
                {
                    Debug.LogWarning("Lista odpowiedzi jest pusta lub null.");
                }
                else
                {
                    Debug.Log($"Załadowano {answerList.answers.Count} odpowiedzi.");
                }

                // Zapis danych do pliku
                SaveToFile(answerList);

                // Odczyt danych z pliku
                AnswerList loadedAnswers = LoadFromFile();
                if (loadedAnswers != null && loadedAnswers.answers.Count > 0)
                {
                    foreach (var answer in loadedAnswers.answers)
                    {
                        Debug.Log($"ID: {answer.id}, Pytanie: {answer.pid}, Tekst odpowiedzi: {answer.text}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Błąd podczas przetwarzania danych JSON: {ex.Message}");
            }
        }
    }

    void SaveToFile(AnswerList data)
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

    AnswerList LoadFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                Debug.Log($"Odczytany JSON: {json}");

                AnswerList answerList = JsonUtility.FromJson<AnswerList>(json);

                if (answerList == null || answerList.answers == null)
                {
                    Debug.LogWarning("Nie udało się sparsować JSON do AnswerList.");
                    return null;
                }

                return answerList;
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
            return new AnswerList { answers = new List<Answer>() };
        }
    }
}
