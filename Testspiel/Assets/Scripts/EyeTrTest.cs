﻿using UnityEngine;
using System.Collections;
using iView;

public class EyeTrTest : GazeMonobehaviour
{
    public bool isMouseModusActive = false;


    //public bool showClue = false;
    Vector3 averageGazePosition, vectorToClue2D, vectorToGaze;
    Vector2 gazePos, gazePoint1, gazePoint2;
    GameObject objectInFocus, clue, player;
    SampleData sample;
    private bool hasFirstPoint = false;
    private float angle = 0f;


    public delegate void cueHandler(bool isShown);
    public event cueHandler OnClueStatus = delegate { };



    //public void CallEvent(int variable)
    //{
    //    //OnShowCue(variable);
    //}


    // Use this for initialization
    void Start()
    {
        //Debug.Log("Screen width " + Screen.width);
        //Debug.Log("Screen height " + Screen.height);
        //Debug.Log("calc " + Screen.width/(Screen.dpi/2.54f));
    }


    // Update is called once per frame
    void Update()
    {
        clue = GetComponent<FindClosestClue>().FindClue();
        sample = SMIGazeController.Instance.GetSample();


        // averageGazePosition = sample.averagedEye.gazePosInUnityScreenCoords();
        gazePos = sample.averagedEye.gazePosInScreenCoords();

        getGazes();
        checkGazeOnObject();
        //screenData();

    }



    private void getGazes()
    {
        //Debugmode is Active
        if (!isMouseModusActive)
        {
            if (hasFirstPoint == false || gazePoint1 == Vector2.zero)
            {

                gazePoint1 = sample.averagedEye.eyePosition;
                hasFirstPoint = true;

            }
            else
            {

                gazePoint2 = sample.averagedEye.eyePosition;
                //Debug.Log("gazePoint2 " + gazePoint2);
                calcDirection();
            }
        }

            //DebugMode
        else
        {
            if (hasFirstPoint == false || gazePoint1 == Vector2.zero)
            {

                gazePoint1 = Input.mousePosition;
                hasFirstPoint = true;

            }
            else
            {

                gazePoint2 = Input.mousePosition;
                //calcDirection();
            }
        }

    }



    private void calcDirection()
    {


        Vector3 cluePos3D = clue.transform.position;
        Vector2 cluePos2D = new Vector2(cluePos3D.x, cluePos3D.y);
        vectorToClue2D = cluePos2D - gazePoint1;
        vectorToGaze = gazePoint2 - gazePoint1;


        float distanceOfGazeVectors = Vector3.Distance(gazePoint1, gazePoint2);
        float distanceGP1ToClue = Vector3.Distance(gazePoint1, cluePos2D);
        float distanceGP2ToClue = Vector3.Distance(gazePoint2, cluePos2D);

        if (distanceOfGazeVectors <= 5)
        {
            hasFirstPoint = true;
            //showClue = false;

        }
        else
        {
            // Debug.Log("next");
            angle = Vector3.Angle(vectorToClue2D, vectorToGaze);
            //Debug.Log("Angle " + angle);


            // looking in correct direction -> don't show clue image
            if (angle <= 20)
            {

                OnClueStatus(false);

                //Debug.Log("in richtige Richtung");
                //Debug.Log("GP1" + gazePoint1);
                //Debug.Log("GP2" + gazePoint2);
                //Debug.Log("distance" + distanceOfGazeVectors);

            }


            //if gaze turns away from target
            if (distanceGP1ToClue > distanceGP2ToClue)
            {
                OnClueStatus(true);
            }

            else
            {

                OnClueStatus(true);
            }

            //resets gazepoints
            hasFirstPoint = false;

        }

    }


    //check if user's gaze is directly on clue
    private void checkGazeOnObject()
    {

        try
        {
            objectInFocus = SMIGazeController.Instance.GetObjectInFocus(FocusFilter.WorldSpaceObjects);
            if (objectInFocus.tag == "Clue")
            {
                clue = objectInFocus;
                OnClueStatus(false);
                //showClue = false;
            }
            //else
            //showClue = true;
            //OnClueStatus(true);
        }

        catch (System.Exception e)
        {
            Debug.Log(e);
        }

    }



    //draw gaze as rectangle
    private void OnGUI()
    {
        Texture2D square = new Texture2D(20, 20);
        square.SetPixel(1, 1, Color.white);
        square.Apply();
        GUI.DrawTexture(new Rect(gazePos.x, gazePos.y, square.width, square.height), square);

    }



    //Nur ein Versuch, funktioniert so nicht ganz
    private void screenData()
    {
        Vector2 clueVec = Camera.main.WorldToScreenPoint(clue.transform.position);
        Vector2 gazeInScreenCoord = Camera.main.WorldToScreenPoint(gazePoint2);

        //double screenWidthInCm = (Screen.width * 2.54 / 96.0);
        //double screenHeightInCm = (Screen.height * 2.54 / 96.0);

        //calculating 2,5 cm in px
        double radiusCirclePx = (2.5 * Screen.dpi / 2.54);
        float distanceOfGaze = Vector2.Distance(gazeInScreenCoord, clueVec);
        Debug.Log("Distance" + distanceOfGaze);
        Debug.Log("gaze screen" + gazeInScreenCoord);
        Debug.Log("clueVec" + clueVec);
        //Debug.Log("radius" + radiusCirclePx);

        if (radiusCirclePx <= distanceOfGaze)
        {
            OnClueStatus(false);
            //Debug.Log("don't show");
        }
        else
        {
            OnClueStatus(true);
            //Debug.Log("show");
        }
    }
}

