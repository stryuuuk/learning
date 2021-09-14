using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    public GameObject allCubes, vfx;
    public GameObject[] canvasStartPage;
    public GameObject[] cubesToCreate;
    public GameObject restart;
    private Rigidbody allCubesRb;
    private bool IsLose;
    private bool firstCube;
    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,1),
        new Vector3(-1,0,-1),
        new Vector3(1,0,-1),
        new Vector3(-1,0,1),
    };
    private Coroutine showCubePlace;
    private Transform mainCam;
    private float camMoveToYPosition;
    private int prevCountMaxHorizontal;
    public Text scoreTxt;
    public Color[] bgColors;
    private Color toCameraColor;
    private List<GameObject> possibleCubes = new List<GameObject>();
    private void Start()
    {
        if (PlayerPrefs.GetInt("score") < 5)
            possibleCubes.Add(cubesToCreate[0]);
        else if (PlayerPrefs.GetInt("score") < 10)
            AddCube(2);
        else if (PlayerPrefs.GetInt("score") < 20)
            AddCube(3);
        else if (PlayerPrefs.GetInt("score") < 30)
            AddCube(4);
        else if (PlayerPrefs.GetInt("score") < 40)
            AddCube(5);
        else if (PlayerPrefs.GetInt("score") < 50)
            AddCube(6);
        else if (PlayerPrefs.GetInt("score") < 60)
            AddCube(7);
        else if (PlayerPrefs.GetInt("score") < 70)
            AddCube(8);
        else if (PlayerPrefs.GetInt("score") < 80)
            AddCube(9);
        else AddCube(10);
        mainCam = Camera.main.transform;
        camMoveToYPosition = 7.69f + nowCube.y - 1f;
        toCameraColor = Camera.main.backgroundColor;
        scoreTxt.text = "HIGH SCORE : " + PlayerPrefs.GetInt("score") + "\nnow: 0";

        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }
 
    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !EventSystem.current.IsPointerOverGameObject() && cubeToPlace != null && allCubes != null)
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            //string res = "";

            if (!firstCube)
            {
                firstCube = true;
                foreach(GameObject obj in canvasStartPage)
                {
                    Destroy(obj);
                }
               
            }
            restart.SetActive(true);
            GameObject Cube = null;
            if (possibleCubes.Count == 1) Cube = possibleCubes[0];
            else Cube = possibleCubes[Random.Range(0, possibleCubes.Count)];
            GameObject newCube = Instantiate(Cube, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.SetVector(cubeToPlace.position);
            allCubesPositions.Add(cubeToPlace.position);
            if (PlayerPrefs.GetString("music") != "No")
            {
                GetComponent<AudioSource>().Play();
            }
            GameObject newVfx = Instantiate(vfx, cubeToPlace.position, Quaternion.identity);
            Destroy(newVfx, 1.5f);
            //Debug.Log(nowCube.GetVector().x + " " + nowCube.GetVector().y + " " + nowCube.GetVector().z);
            //foreach(Vector3 i in allCubesPositions)
            //{
            //    res += i.x + " " + i.y + " " + i.z + ",";
            //    //Debug.Log("All cubes: " + i.x + " " + i.y + " " + i.z);
            //}
            //Debug.Log(res);
            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;
            SpawnPositions();
            MoveCameraChangeBg();
        }
         if(!IsLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            restart.SetActive(false);
            StopCoroutine(showCubePlace);
        }
         if(Camera.main.backgroundColor != toCameraColor)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
        }
        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),2f*Time.deltaTime);
    }
    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }
    private void AddCube(int till)
    {
        for (int i = 0; i < till; i++)
            possibleCubes.Add(cubesToCreate[i]);
    }
    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && cubeToPlace.position.x != nowCube.x + 1)
        {
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && cubeToPlace.position.x != nowCube.x - 1)
        {
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && cubeToPlace.position.y != nowCube.y + 1)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && cubeToPlace.position.y != nowCube.y - 1)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && cubeToPlace.position.z != nowCube.z + 1)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        }

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && cubeToPlace.position.z != nowCube.z - 1)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        }

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0) IsLose = true;
        else
            cubeToPlace.position = positions[0];

    }
    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0) return false;
        foreach(Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;
        foreach(Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs((int)pos.x) > maxX) maxX = (int)pos.x;
            if ((int)pos.y > maxY) maxY = (int)pos.y;
            if (Mathf.Abs((int)pos.z) > maxZ) maxZ = (int)pos.z;
        }

        if (PlayerPrefs.GetInt("score") < maxY - 1) 
            PlayerPrefs.SetInt("score", maxY - 1);

        scoreTxt.text = "HIGH SCORE : " + PlayerPrefs.GetInt("score") + "\nnow: " + (maxY - 1);

        camMoveToYPosition = 7.69f + nowCube.y - 1f;

        maxHor= maxX > maxZ ? maxX : maxZ;
        if (maxHor % 3 == 0 && prevCountMaxHorizontal != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0,2.5f);
            prevCountMaxHorizontal = maxHor;
        }

        if (maxY >= 10)
            toCameraColor = bgColors[2];
        else if (maxY >= 5)
            toCameraColor = bgColors[1];
        else if (maxY >= 2)
            toCameraColor = bgColors[0];
    }
}
struct CubePos
{
    public int x,y,z;

    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector()
    {
        return new Vector3(x, y, x);
    }

    public void SetVector(Vector3 pos)
    {
        x = (int)pos.x;
        y = (int)pos.y;
        z = (int)pos.z;
    }
}
