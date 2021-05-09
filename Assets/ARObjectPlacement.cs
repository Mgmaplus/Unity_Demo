using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARObjectPlacement : MonoBehaviour
{
    public GameObject[] objectToPlace; 

    public GameObject[] placementIndicator;

    private int currentObject;



    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycastManager;
    private Camera cam;
    private Pose placementPose, fingerPlacementPose;
    private GameObject[] placedObject;

    private bool placementPoseIsValid = false;
    private bool fingerPlacementIsValid = false;

    public int CurrentObject { get => currentObject; 
    set {
        placementIndicator[currentObject].SetActive(false);
        currentObject = value;}}

    public void DeleteCurrentObject()
    {
        if (!placedObject[currentObject])
            return;
        Destroy(placedObject[currentObject]);
        placedObject[currentObject] = null ;
    
    }

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycastManager = FindObjectOfType<ARRaycastManager>();
        cam = Camera.main;
        placedObject = new GameObject[objectToPlace.Length] ;
    }

    void Update()
    {
        UpdateCenterPlacementPose();
        if (placementIndicator[CurrentObject])
            UpdatePlacementIndicator();

        if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0))
        {
            UpdateFingerPlacementPose(Input.mousePosition);
            if (objectToPlace[CurrentObject])
                PlaceObject();
        }
    }

    private void PlaceObject()
    {
        if (!fingerPlacementIsValid)
            return;

        if (placedObject[CurrentObject])
        {
            placedObject[CurrentObject].transform.SetPositionAndRotation(fingerPlacementPose.position, fingerPlacementPose.rotation);
        }
        else
        {
            placedObject[CurrentObject] = Instantiate(objectToPlace[CurrentObject], fingerPlacementPose.position, fingerPlacementPose.rotation);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && !placedObject[CurrentObject])
        {
            placementIndicator[CurrentObject].SetActive(true);
            placementIndicator[CurrentObject].transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator[CurrentObject].SetActive(false);
        }
    }

    private void UpdateCenterPlacementPose()
    {
        var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            var cameraForward = cam.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            // placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            placementPose.rotation = Quaternion.LookRotation(placementPose.up);
        }
    }

    private void UpdateFingerPlacementPose(Vector3 fingerPosition)
    {
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(fingerPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        fingerPlacementIsValid = hits.Count > 0;
        if (fingerPlacementIsValid)
        {
            fingerPlacementPose = hits[0].pose;
            fingerPlacementPose.rotation = Quaternion.LookRotation(fingerPlacementPose.up);
        }
    }
}