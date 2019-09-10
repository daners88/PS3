using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadTileBoardPCG : MonoBehaviour
{
    public GameObject tilePrefab = null;
    public int stepCount = 10;
    public int walkCount = 1;
    public int seed = 1;
    public bool wanderWalk = false;
    public bool randomSeed = false;
    private float minX = 0, minZ = 0, maxX = 0, maxZ = 0;
    public GameObject placementCounter;
    private UnityEngine.UI.Text counterText;

    private void Start()
    {
        counterText = placementCounter.GetComponent<UnityEngine.UI.Text>();
    }

    IEnumerator LerpObj(Transform target, Vector3 start, Vector3 end, float duration)
    {
        float time = 0;
        while(time < duration)
        {
            float t = time / duration;
            target.position = Vector3.Lerp(start, end, t);

            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
    }

    public void DrunkenWalk()
    {
        StartCoroutine(DrunkenWalk(Vector3.zero, new List<Vector3>(){
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left }));
    }

    public float SetHeight(System.Random rnd, Vector3 curPoint, List<GameObject> tiles)
    {
        int determinator = rnd.Next(0, 100);
        if(determinator > 60)
        {
            return curPoint.y;
        }
        else
        {
            float minY = 5.0f, maxY = 0.0f;
            List<GameObject> possibleAdjacents = new List<GameObject>();
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x - 1 && t.transform.position.z == curPoint.z + 1).FirstOrDefault());
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x && t.transform.position.z == curPoint.z + 1).FirstOrDefault());
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x + 1 && t.transform.position.z == curPoint.z + 1).FirstOrDefault());
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x - 1 && t.transform.position.z == curPoint.z).FirstOrDefault());
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x + 1 && t.transform.position.z == curPoint.z).FirstOrDefault());
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x - 1 && t.transform.position.z == curPoint.z - 1).FirstOrDefault());
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x && t.transform.position.z == curPoint.z - 1).FirstOrDefault());
            possibleAdjacents.Add(tiles.Where(t => t.transform.position.x == curPoint.x + 1 && t.transform.position.z == curPoint.z - 1).FirstOrDefault());


            foreach (var adj in possibleAdjacents)
            {
                if (adj != null)
                {
                    if (adj.transform.position.y < minY)
                    {
                        minY = adj.transform.position.y;
                    }
                    if (adj.transform.position.y > maxY)
                    {
                        maxY = adj.transform.position.y;
                    }
                }
            }
            if (minY == 5.0f)
            {
                minY = 0.33f;
            }
            if (maxY == 0.0f)
            {
                maxY = 4.66f;
            }
            if(maxY - minY < .50f)
            {
                minY = 0.25f;
                maxY = 4.75f;
            }

            float myheight = rnd.Next((int)(minY * 100), (int)(maxY * 100)) / 100.0f;
            float minDif = myheight - minY;
            float maxDif = maxY - myheight;

            if (maxDif > minDif)
            {
                if (maxDif > 1.0)
                {
                    myheight = maxY - 0.75f;
                }
            }
            else
            {
                if (minDif > 1.0)
                {
                    myheight = minY + 0.75f;
                }
            }

            return myheight;
        }
    }

    //use while loop to reach a number of tiles
    public IEnumerator DrunkenWalk(Vector3 start, List<Vector3> directions)
    {
        int placed = 0;
        System.Random rnd;
        if (randomSeed)
        {
            System.Random seedGenerator = new System.Random();
             rnd = new System.Random(seedGenerator.Next());
        }
        else
        {
            rnd = new System.Random(seed);
        }

        List<GameObject> tiles = new List<GameObject>();

        for (int j = 0; j < walkCount; j++)
        {
            Vector3 curPoint = Vector3.zero;
            if (wanderWalk)
            {
                curPoint = tiles.Count == 0 ? start : tiles[rnd.Next(0, tiles.Count)].transform.position;
            }
            else
            {
                curPoint = start;
            }

            for (int i = 0; i < stepCount; i++)
            {
                int stepsWithoutPlacing = 0;
                if (tiles.Where(t => t.transform.position.x == curPoint.x && t.transform.position.z == curPoint.z).FirstOrDefault() == null)
                {
                    stepsWithoutPlacing = 0;
                    curPoint.y = SetHeight(rnd, curPoint, tiles);
                    GameObject tile = Instantiate<GameObject>(tilePrefab, curPoint, Quaternion.identity, transform);
                    tile.transform.localScale = new Vector3(1, curPoint.y * 2, 1);
                    tiles.Add(tile);
                    placed++;
                }
                else
                {
                    i--;
                    stepsWithoutPlacing++;
                }
                if(stepsWithoutPlacing > 15)
                {
                    curPoint = tiles[rnd.Next(0, tiles.Count)].transform.position;
                }
                int dirIndex = rnd.Next(0, directions.Count);
                Vector3 dir = directions[dirIndex];
                curPoint += dir;
                counterText.text = placed.ToString();
                yield return null;
            }
        }
    }
}
