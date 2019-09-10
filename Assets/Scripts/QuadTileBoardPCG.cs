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
                //if(curPoint.x < minX || curPoint.x > maxX || curPoint.z < minZ || curPoint.z > minZ)
                //{
                //    GameObject tile = Instantiate<GameObject>(tilePrefab, curPoint, Quaternion.identity, transform);
                //    tiles.Add(tile);
                //    if(curPoint.x < minX)
                //    {
                //        minX = curPoint.x;
                //    }
                //    if (curPoint.x > maxX)
                //    {
                //        maxX = curPoint.x;
                //    }
                //    if (curPoint.z < minZ)
                //    {
                //        minZ = curPoint.z;
                //    }
                //    if (curPoint.z > minZ)
                //    {
                //        maxZ = curPoint.z;
                //    }
                //    placed++;
                //}
                if(tiles.Where(t => t.transform.position.Equals(curPoint)).FirstOrDefault() == null)
                {
                    GameObject tile = Instantiate<GameObject>(tilePrefab, curPoint, Quaternion.identity, transform);
                    tiles.Add(tile);
                    placed++;
                }
                else
                {
                    i--;
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
