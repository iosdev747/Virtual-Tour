using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

public class ObjectManager : MonoBehaviour
{
    public GameObject SpherePrefab, CapturePointPrefab, NeighbourCapturePointPrefab;
	public Shader shader;
	Root root;
	GameObject MainCameraSphere;
	CapturePoint currentCapturePoint;
	float max_x = -Mathf.Infinity, max_z = -Mathf.Infinity, min_x = Mathf.Infinity, min_z = Mathf.Infinity;
	public Texture texture112;
	void Start()
	{
		StreamReader reader = new StreamReader ("Assets/Resources/JSONData/sampleJSON.json");
		string jsonString = reader.ReadToEnd();
		reader.Close();
		root = JsonConvert.DeserializeObject<Root>(jsonString);
		foreach (CapturePoint point in root.CapturePoints)
		{
			if(point.X < min_x) {
				min_x = point.X;
			}
			if(point.Y < min_z) {
				min_z = point.Y;
			}
			if(point.X > max_x) {
				max_x = point.X;
			}
			if(point.Y > max_z) {
				max_z = point.Y;
			}
		}
		Debug.Log(min_x);
		Debug.Log(min_z);
		Debug.Log(max_x);
		Debug.Log(max_z);
		if(max_x != -Mathf.Infinity) { // to check that we have atlease one capture point
			currentCapturePoint = root.CapturePoints[0];
			SetCapturePoint(currentCapturePoint);
		}
	}

	void SetCapturePoint(CapturePoint point) {
		float x = 2.0f;
		GameObject capturePoint;
		capturePoint = Instantiate(CapturePointPrefab, new Vector3(x*point.X, x*point.Z, x*point.Y), Quaternion.identity);
		capturePoint.gameObject.name = point.TextureName;
		foreach (Neighbour neighbour in point.Neighbours) {
			capturePoint = Instantiate(NeighbourCapturePointPrefab, new Vector3(x*neighbour.X, x*neighbour.Z, x*neighbour.Y), Quaternion.identity);
			capturePoint.gameObject.name = neighbour.TextureName;
		}
		SpawnSphereAroundPoint(point);
	}

	void SpawnSphereAroundPoint(CapturePoint point) {
		float x = 2.0f;
		var texture = Resources.Load<Texture2D>("Textures/" + point.TextureName);
		Debug.Log("Assets/Resources/Textures/" + point.TextureName);
		MainCameraSphere = Instantiate(SpherePrefab, new Vector3(x*point.X, x*point.Z, x*point.Y), Quaternion.identity);
		float radius = (Mathf.Max(Mathf.Abs(max_x), Mathf.Abs(max_z)) + Mathf.Max(Mathf.Abs(min_x), Mathf.Abs(min_z)))*10.0f;
		MainCameraSphere.transform.localScale = new Vector3(radius, radius, radius);
		MainCameraSphere.GetComponent<MeshRenderer>().material = new Material(shader);
		MainCameraSphere.GetComponent<MeshRenderer>().material.mainTexture = texture;
		MainCameraSphere.gameObject.name = "The Promised Land";
		GameObject.Find("Main Camera").transform.position = new Vector3(x*point.X, x*point.Z + 1.5f, x*point.Y);
	}
	
	void doom() {
		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
		foreach(GameObject go in allObjects) {
			Debug.Log(go.gameObject.name);
			if(go.gameObject.name == "Directional Light" || go.gameObject.name == "Main Camera")
				continue;
			Destroy(go);
		}
	}

	CapturePoint FindCapturePoint(String pointTextureName) {
		foreach (CapturePoint point in root.CapturePoints) {
			if(point.TextureName == pointTextureName) {
				return point;
			}
		}
		return null;
	}

    void Update()
    {
		if(Input.GetButtonDown("Fire2"))
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, 100 ) )
			{
				if(hit.collider.gameObject.name == "The Promised Land")
					return;
				GameObject selectedCapturePoint = GameObject.Find(hit.collider.gameObject.name);
				CapturePoint cp = FindCapturePoint(selectedCapturePoint.gameObject.name);
				if(cp != null) {
					doom();
					SetCapturePoint(cp);
				}
			}
		}
    }
}
