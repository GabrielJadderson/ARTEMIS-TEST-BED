using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Construction
{
	[RequireComponent(typeof(ConstructionContainer))]
	public class ConstructionManager : MonoBehaviour
	{
		public Camera _camera;
		public GameObject selectedObject;
		public static ConstructionContainer constructionContainer;

		ConstructableType type;

		RaycastHit hit; // a hit is cast to figure out where to go.
		Ray ray; // our ray, both RaycastHit and Ray are initially referenced to save memory.
		bool canBuild = false;
		int currency = 0;
		float offset = 0.0f;
		float gridSize = 0.10f;
		Vector3 currentPos;

		int rotationFactor = 0;

		void Start()
		{
			constructionContainer = GetComponent<ConstructionContainer>();

			canBuild = true;

			type = ConstructableType.BRICK; //test
		}

		void Update()
		{
			if (canBuild) {
				if (selectedObject != null) {
					ray = _camera.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out hit)) {
						if (hit.collider.tag != "Player") { //ignores the player
							moveObject();

							if (Input.GetMouseButtonDown(0)) {
								//place object.
								placeObject();
							}
						}
					}
				}

				if (Input.GetAxis("Mouse ScrollWheel") != 0) {
					rotateObject(90f);
				}
			}
			if (Input.GetKeyDown(KeyCode.C)) {
				createObject();
				canBuild = true;
			}
		}

		void placeObject()
		{
			canBuild = false;
			//var go = GameObject.Instantiate(selectedObject);
			//selectedObject.transform.SetParent(transform);
			selectedObject.AddComponent<MeshCollider>();
			selectedObject = null;
		}

		void rotateObject(float degrees)
		{
			if (selectedObject != null) {
				//selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				if (rotationFactor == 0) {
					rotationFactor = 1;
				} else if (rotationFactor == 1) {
					rotationFactor = 2;
					//selectedObject.transform.RotateAround(hit.normal, Vector3.up, degrees);
				} else if (rotationFactor == 2) {
					rotationFactor = 3;
					//selectedObject.transform.RotateAround(hit.normal, Vector3.up, degrees);
				} else if (rotationFactor == 3) {
					rotationFactor = 0;
					//selectedObject.transform.RotateAround(hit.normal, Vector3.up, degrees);
				}
				Debug.Log(rotationFactor);
			}
		}

		void moveObject()
		{
			/*
			currentPos = hit.point;
			currentPos -= Vector3.one * offset;
			currentPos /= gridSize;
			currentPos = new Vector3(Mathf.Round(currentPos.x), Mathf.Round(currentPos.y), Mathf.Round(currentPos.z));
			currentPos *= gridSize;
			currentPos += Vector3.one * offset;
			selectedObject.transform.position = currentPos;
			*/

			//selectedObject.transform.position = hit.point;
			if (rotationFactor == 0) {
				selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			} else if (rotationFactor == 1) {
				selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				selectedObject.transform.rotation = Quaternion.AngleAxis(90f, hit.normal);
			} else if (rotationFactor == 2) {
				selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				selectedObject.transform.rotation = Quaternion.AngleAxis(180f, hit.normal);
			} else if (rotationFactor == 3) {
				selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				selectedObject.transform.rotation = Quaternion.AngleAxis(270f, hit.normal);
			}
			//selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.left, hit.normal);
			//selectedObject.transform.position = SnapToGrid(hit.point);
			//selectedObject.transform.position = hit.point;

			float grid_scale = 0.082f;
			var currentPos = hit.point;
			selectedObject.transform.position = new Vector3(
				Mathf.Round(currentPos.x / grid_scale) * grid_scale,
				Mathf.Round(( currentPos.y - ( selectedObject.transform.localScale.y ) / 50 ) / grid_scale) * grid_scale,
				Mathf.Round(currentPos.z / grid_scale) * grid_scale);
		}

		void createObject()
		{
			if (selectedObject == null) {
				if (currency >= (int) type) { //has money
					currency -= ( (int) type ); //substract currency
					switch (type) {
						case ConstructableType.BRICK:
							selectedObject = GameObject.Instantiate(constructionContainer.buildingBlocksBricks[ Random.Range(0, 4) ]);
							selectedObject.transform.SetParent(transform);
							break;
						case ConstructableType.WOOD:
							break;
					}
				}
			}
		}

		Vector3 SnapToGrid(Vector3 Position)
		{
			//    get grid size from the texture tiling
			Vector3 GridSize = new Vector3(0.2f, 1.0f, 1.0f);

			//    get position on the quad from -0.5...0.5 (regardless of scale)
			Vector3 gridPosition = selectedObject.transform.InverseTransformPoint(Position);
			//    scale up to a number on the grid, round the number to a whole number, then put back to local size
			gridPosition.x = Mathf.Round(( gridPosition.x * GridSize.x ) / GridSize.x);
			gridPosition.y = Mathf.Round(( ( gridPosition.y - ( selectedObject.transform.localScale.y / 2 ) ) * GridSize.y ) / GridSize.y);
			gridPosition.z = Mathf.Round(( gridPosition.z * GridSize.z ) / GridSize.z);

			//    don't go out of bounds
			//gridPosition.x = Mathf.Min(0.5f, Mathf.Max(-0.5f, gridPosition.x));
			//gridPosition.y = Mathf.Min(0.5f, Mathf.Max(-0.5f, gridPosition.y));
			//gridPosition.z = Mathf.Min(0.5f, Mathf.Max(-0.5f, gridPosition.z));

			Position = selectedObject.transform.TransformPoint(gridPosition);
			return Position;
		}
	}
}
