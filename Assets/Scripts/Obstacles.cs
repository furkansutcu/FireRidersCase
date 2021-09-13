using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour {
    public float maxVariance = 2f;
    public int count = 16;
    public GameObject OneColumn;
    public Transform ball;
    public GameObject targetPrefab;
    public int targetFrequency = 20;

    private List<GameObject> columnList;
    private int EstColumnIndex;
    private int NewestColumnIndex;
    private float LastAppliedBallZ;
    private float Colors;
    private int targetCounter;
    private float MainSize;

	void Start ()
    {
        StartCoroutine(nameof(BallPosition));
        MainSize = OneColumn.transform.localScale.z;
        columnList = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            if (i == 0)
            {
                columnList.Add(OneColumn);
                SetColor(0);
            }
            else
            {
                GameObject newColumn = Instantiate(OneColumn);
                newColumn.transform.parent = OneColumn.transform.parent;
                columnList.Add(newColumn);
                PlaceColumn(columnList.Count - 1, columnList[columnList.Count - 2].transform.position, 1);
            }
        }
        Colors = 0;
        EstColumnIndex = 0;
        NewestColumnIndex = columnList.Count - 1;
	}
    void Update ()
    {
		
	}

    private void PlaceColumn(int columnIndex, Vector3 prevColumnPos, float maximumVariance )
    {
        Vector3 movePos = new Vector3(prevColumnPos.x, Random.Range(prevColumnPos.y - maximumVariance, prevColumnPos.y + maximumVariance), prevColumnPos.z + MainSize);
        columnList[columnIndex].transform.position = movePos;
        SetColor(columnIndex);
        targetCounter++;
        if (columnList[columnIndex].transform.Find("Goal"))
        {
            Destroy(columnList[columnIndex].transform.Find("Goal").gameObject);
        }
        if (targetCounter >= targetFrequency)
        {
            GameObject target = Instantiate(targetPrefab);
            target.transform.parent = columnList[columnIndex].transform;
            target.name = "Goal";
            target.transform.localPosition = new Vector3(0, Random.Range(-5f, 5f), 0);
            targetCounter = 0;
        }
    }

    private void SetColor(int columnIndex)
    {
        Colors += (1f / 250f);
        if (Colors > 1f)
            Colors = 0;
        float saturation = .7f;
        if (columnIndex % 2 == 0)
        {
            saturation = .5f;
        }
        foreach (MeshRenderer renderer in columnList[columnIndex].GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material.color = Color.HSVToRGB(Colors, saturation, .7f);
        }
    }

    public IEnumerator BallPosition()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (ball.position.z > LastAppliedBallZ + MainSize)
            {
                PlaceColumn(EstColumnIndex, columnList[NewestColumnIndex].transform.position, maxVariance);
                NewestColumnIndex = EstColumnIndex;
                EstColumnIndex = (EstColumnIndex + 1) % columnList.Count;
                LastAppliedBallZ += MainSize;
            }
        }
    }
}
