using System;
using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class GestureRecogniser : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] InputActionProperty drawButton;
    [SerializeField] Transform drawSource;

    [Header("Gesture Config")]
    [SerializeField] bool creationMode;
    [SerializeField] string newGestureName;
    [SerializeField] float thresholdPointDistance = 0.05f;
    [SerializeField] GameObject debugSphere;

    [Header("Gesture Spawner")]
    [SerializeField] GestureSpawn gestureSpawn;

    [Header("Line Rendering")]
    [SerializeField] float lineWidth = 0.02f;



    private bool drawButtonPressed;
    private bool isDrawing = false;
    private LineRenderer lineRenderer;
    private int pointIdx;
    private List<Vector3> positionsList = new List<Vector3>();
    private List<Gesture> trainingSet = new List<Gesture>();
    private List<GameObject> debugSphereList = new List<GameObject>();

    void Start()
    {
        //Read gesture files
        string[] gestureFiles = Directory.GetFiles(Application.dataPath + "/Gestures", "*_gesture.xml");
        foreach (string fileName in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(fileName));
        }
        lineRenderer = GetComponent<LineRenderer>();
    }


    void Update()
    {
        drawButtonPressed = drawButton.action.ReadValue<float>() > 0;

        //Start Drawing
        if (!isDrawing && drawButtonPressed)
        {
            StartDrawing();
        }
        //End Drawing
        else if (isDrawing && !drawButtonPressed)
        {
            EndDrawing();
        }
        //Update Drawing
        else if (isDrawing && drawButtonPressed)
        {
            UpdateDrawing();
        }
    }

    private void StartDrawing()
    {
        isDrawing = true;
        positionsList.Clear();
        positionsList.Add(drawSource.position);

        //Debug sphere
        if (debugSphere) debugSphereList.Add(Instantiate(debugSphere, drawSource.position, Quaternion.identity));

        //Initialise line renderer
        lineRenderer.enabled = true;
        pointIdx = 0;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        //lineRenderer.SetPosition(pointIdx, drawSource.position);
    }

    private void EndDrawing()
    {
        isDrawing = false;

        //Create array of points for PDollar gesture from the Positions List
        Point[] pointArray = new Point[positionsList.Count];
        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 _2dPointProjection = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(_2dPointProjection.x, _2dPointProjection.y, 0);
        }
        Gesture newGesture = new Gesture(pointArray);

        //Add new gesture
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.dataPath + "/Gestures/" + newGestureName + "_gesture.xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        //Read gesture
        else
        {
            //Find result from gesture analyser
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log(result.GestureClass + " || " + result.Score);
            if (result.Score >= 0.9f)
            {
                Vector3 centerPoint = new Vector3();
                //find center point
                foreach (Vector3 point in positionsList)
                {
                    centerPoint += point;
                }
                centerPoint /= positionsList.Count;
                //spawn spell rune
                gestureSpawn.Spawn(result.GestureClass, centerPoint);
            }
        }

        //Destroy debug spheres
        foreach (GameObject sphere in debugSphereList)
        {
            Destroy(sphere);
        }
        //Disable line renderer
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    private void UpdateDrawing()
    {
        Vector3 previousPosition = positionsList[positionsList.Count - 1];
        if (Vector3.Distance(previousPosition, drawSource.position) > thresholdPointDistance)
        {
            positionsList.Add(drawSource.position);

            //Debug sphere
            if (debugSphere) debugSphereList.Add(Instantiate(debugSphere, drawSource.position, Quaternion.identity));

            //Add point to line renderer
            Vector3[] positions = positionsList.ToArray();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }
    }
}
