using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTileBoardPCG : MonoBehaviour
{
    public GameObject tilePrefab = null;
    public int stepCount = 10;
    public int walkCount = 1;
    public bool wanderWalk = false;

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
        //start coroutine using local variables
    }

    //use while loop to reach a number of tiles
    public IEnumerator DrunkenWalk(QuadTileBoardPCG pcg, GameObject prefab, Vector3 start, List<Vector3> directions, int stepCount, int walkCount = 1)
    {
        System.Random seedGenerator = new System.Random();
        System.Random rnd = new System.Random(seedGenerator.Next());

        List<GameObject> tiles = new List<GameObject>();

        for (int j = 0; j < walkCount; j++)
        {
            Vector3 curPoint = Vector3.zero;
            if (pcg.wanderWalk)
            {
                curPoint = tiles.Count == 0 ? start : tiles[rnd.Next(0, tiles.Count)].transform.position;
            }
            else
            {
                curPoint = start;
            }

            for (int i = 0; i < stepCount; i++)
            {
                GameObject tile = Instantiate<GameObject>(prefab, curPoint, Quaternion.identity, pcg.transform);
                tiles.Add(tile);
                int dirIndex = rnd.Next(0, directions.Count);
                Vector3 dir = directions[dirIndex];
                curPoint += dir;
                yield return null;
            }
        }
    }
}
