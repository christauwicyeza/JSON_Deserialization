using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class WorkoutDetail
{
    public int ballId;
    public float speed;
    public float ballDirection;
}

[System.Serializable]
public class WorkoutInfo
{
    public int workoutID;
    public string workoutName;
    public string description;
    public string ballType;
    public List<WorkoutDetail> workoutDetails;
}

[System.Serializable]
public class WorkoutData
{
    public string ProjectName;
    public int numberOfWorkoutBalls;
    public List<WorkoutInfo> workoutInfo;
}

public class JsonHandler : MonoBehaviour
{
    public Text titleText;
    public GameObject buttonPrefab;
    public Transform buttonParent;
    public GameObject ballPrefab;

    private WorkoutData workoutData;
    private List<GameObject> workoutButtons = new List<GameObject>();

    void Start()
    {
        LoadJson();
        GenerateUI();
    }

    void LoadJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "workoutData.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            workoutData = JsonUtility.FromJson<WorkoutData>(json);
            workoutData.ProjectName = "Exciting JSON Project"; // Change this to your desired project name
        }
        else
        {
            Debug.LogError("Cannot find JSON file!");
        }
    }

    void GenerateUI()
    {
        if (workoutData != null)
        {
            titleText.text = workoutData.ProjectName;

            float buttonSpacing = 40.0f;
            float startY = 100.0f;

            for (int i = 0; i < workoutData.workoutInfo.Count; i++)
            {
                var workout = workoutData.workoutInfo[i];
                GameObject buttonGO = Instantiate(buttonPrefab, buttonParent);
                Text buttonText = buttonGO.GetComponentInChildren<Text>();
                buttonText.text = workout.workoutName;

                RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                buttonRect.anchoredPosition = new Vector2(0, startY - (i * buttonSpacing));

                int index = i; // Capture the index in lambda expression
                Button button = buttonGO.GetComponent<Button>();
                button.onClick.AddListener(() => OnWorkoutButtonClick(index));

                workoutButtons.Add(buttonGO);
            }
        }
    }

    void OnWorkoutButtonClick(int index)
    {
        var workout = workoutData.workoutInfo[index];

        // Hide all buttons except the clicked one
        foreach (var btnGO in workoutButtons)
        {
            if (btnGO != buttonParent.GetChild(index).gameObject)
                btnGO.SetActive(false);
        }

        // Spawn balls for the clicked workout
        SpawnBalls(workout);

        // Delay showing buttons again
        Invoke("ShowButtons", 2.0f); // Adjust delay time as needed
    }

    void SpawnBalls(WorkoutInfo workout)
    {
        foreach (var detail in workout.workoutDetails)
        {
            GameObject ball = Instantiate(ballPrefab, GetBallPosition(detail.ballDirection), Quaternion.identity);
            ball.GetComponent<Rigidbody>().velocity = Vector3.forward * detail.speed;
        }
    }

    void ShowButtons()
    {
        // Show all buttons again
        foreach (var btnGO in workoutButtons)
        {
            btnGO.SetActive(true);
        }
    }

    Vector3 GetBallPosition(float direction)
    {
        return new Vector3(direction * 5f, 1f, 0f); // Adjust ball spawn position as needed
    }
}
