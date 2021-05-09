using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FocusPointDriver : MonoBehaviour
{
    public GameObject debugObject;

    private ARFace arFace;
    private ARFaceManager arFaceManager;
    private Transform forehead, nose;
    public Transform head;
    public List<GameObject> hat;
    private int indexOfList;
    public GameObject glasses;
    private Transform instanceOfHat;
    private Transform instanceOfGlasses;
    
    void Awake()
    {
        arFace = GetComponent<ARFace>();
        arFaceManager = FindObjectOfType<ARFaceManager>();
        // forehead = Instantiate(debugObject, transform).transform;
        // nose = Instantiate(debugObject, transform).transform;
        // head = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        // head.SetParent(transform, false);
        Debug.Log("Instantiated debug objects");
    }

    // void OnEnabled()
    // {
    //     arFace.updated += UpdateFace;
    // }

    // void OnDisable()
    // {
    //     arFace.updated -= UpdateFace;
    // }

    void Update()
    {
        if (!arFace)
            return;
        if (arFace.trackingState == TrackingState.Tracking)
        {
            var foreheadVertex = arFace.vertices[10];
            var noseVertex = arFace.vertices[1];
            var headVertex =  - noseVertex / 2;
            float diameterOfHead = Vector3.Distance(headVertex, arFace.vertices[10]) * 2.2f;
            head.localPosition = headVertex;
            head.localScale = new Vector3(diameterOfHead, diameterOfHead, diameterOfHead);
            Debug.Log($"Updating objects. Head position is {head.position} and diameter is {diameterOfHead}");
            // forehead.transform.localPosition = foreheadVertex;
            // nose.transform.localPosition = noseVertex;

            if (instanceOfHat)
            {
                if (instanceOfHat.position.y < head.position.y)
                {
                    Destroy(instanceOfHat.gameObject);
                    instanceOfHat = null;
                    indexOfList = indexOfList == hat.Count -1 ? 0 : indexOfList + 1;
                    if (instanceOfGlasses)
                    {
                        var glassesRigidbody = instanceOfGlasses.GetComponent<Rigidbody>();
                        glassesRigidbody.isKinematic = false;
                        glassesRigidbody.useGravity = true;
                    }
                }
            }
            else
            {
                instanceOfHat = Instantiate(hat[indexOfList], head.position + Vector3.up * diameterOfHead, Quaternion.identity, transform).transform;
                if (instanceOfGlasses)
                {
                    Destroy(instanceOfGlasses.gameObject);
                    instanceOfGlasses = null;
                }
                if (UnityEngine.Random.value > 0.5)
                {
                    instanceOfGlasses = Instantiate(glasses, Vector3.zero, Quaternion.identity, transform).transform;
                    instanceOfGlasses.localPosition = arFace.vertices[6];
                }
            }
        }
    }
}
