using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadTileBoardPCG : MonoBehaviour
{
    public GameObject cubePrefab = null;
    public GameObject tileParent = null;
    public GameObject waterPrefab = null;
    public Material desertMaterial = null;
    public Material snowMaterial = null;
    public Material grassMaterial = null;
    public Material forestMaterial = null;

    public int waterBuffer = 5;
    public int stepCount = 10;
    public int walkCount = 1;
    public int seed = 1;
    public bool wanderWalk = false;
    public bool elevationBased = true;
    public bool nsBased = false;
    public bool randomSeed = false;
    private float minX = 0, minZ = 0, maxX = 0, maxZ = 0;
    private UnityEngine.UI.Text counterText;
    private UnityEngine.UI.Text timeElapsed;
    private int indexAtWhichThingsBecomeWater = 0;
    private int anticipatedCubeCount = 0;
    List<Vector3> directions = new List<Vector3>(){
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left };


    private void Start()
    {
        anticipatedCubeCount = stepCount * walkCount;
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

    public struct IntKey
    {
        private readonly int first;
        private readonly int second;

        public int First { get { return first; } }
        public int Second { get { return second; } }

        public IntKey(int first, int second)
        {
            this.first = first;
            this.second = second;
        }

        public bool Equals(IntKey other)
        {
            return this.first == other.first && this.second == other.second;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is IntKey && Equals((IntKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.first * 397) ^ this.second;
            }
        }
    }

    public void StartDrunkenWalk()
    {
        StartCoroutine(DrunkenWalk());
    }

    public float SetHeight(System.Random rnd, Vector3 curPoint, Dictionary<IntKey, GameObject> tileDictionary)
    {
        int determinator = rnd.Next(0, 100);
        if(determinator > 24)
        {
            return curPoint.y;
        }
        else
        {
            float minY = 5.0f, maxY = 0.0f;
            List<GameObject> possibleAdjacents = new List<GameObject>();
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x, (int)curPoint.z + 1))).FirstOrDefault().Value);
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x, (int)curPoint.z + 1))).FirstOrDefault().Value);
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x, (int)curPoint.z + 1))).FirstOrDefault().Value);
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x - 1, (int)curPoint.z))).FirstOrDefault().Value);
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x + 1, (int)curPoint.z))).FirstOrDefault().Value);
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x - 1, (int)curPoint.z - 1))).FirstOrDefault().Value);
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x, (int)curPoint.z - 1))).FirstOrDefault().Value);
            possibleAdjacents.Add(tileDictionary.Where(t => t.Key.Equals(new IntKey((int)curPoint.x + 1, (int)curPoint.z - 1))).FirstOrDefault().Value);


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
                minY = 0.0f;
            }
            if (maxY == 0.0f)
            {
                maxY = 0.66f;
            }
            if (maxY - minY < .75f)
            {
                minY = maxY - 0.75f;
                if(minY < 0.0f)
                {
                    minY = 0.0f;
                    maxY = 0.75f;
                }
            }

            float myheight = rnd.Next((int)(minY * 100), (int)(maxY * 100)) / 100.0f;
            float minDif = myheight - minY;
            float maxDif = maxY - myheight;

            if (maxDif > minDif)
            {
                if (maxDif > 1.0)
                {
                    myheight = maxY - 0.75f;
                    if(myheight < 0)
                    {
                        myheight = 0.0f;
                    }
                    else if(myheight > 5)
                    {
                        myheight = 5.0f;
                    }
                }
            }
            else
            {
                if (minDif > 1.0)
                {
                    myheight = minY + 0.75f;
                    if (myheight < 0)
                    {
                        myheight = 0.0f;
                    }
                    else if (myheight > 5)
                    {
                        myheight = 5.0f;
                    }
                }
            }

            //if(myheight > 2.5)
            //{
            //    int b = rnd.Next(0, 10);
            //    if(b > 4)
            //    {
            //        myheight = rnd.Next((int)(2.5 * 100), (int)(myheight * 100))/100.0f;
            //    }
            //}

            return myheight;
        }
    }

    public float GetScoreFromElevation(float y)
    {
        float score = 0.0f;

        if (y > -0.28)
        {
            score = 1.0f;
        }
        else if (y > -0.48)
        {
            score = 0.75f;
        }
        else if (y > -0.68)
        {
            score = 0.5f;
        }
        else if(y > -0.88)
        {
            score = 0.25f;
        }

        return score;
    }

    public float GetScoreFromNS(float z, System.Random rnd)
    {
        float score = 0.0f;

        if(Mathf.Abs(z) > anticipatedCubeCount * .005)
        {
            int totesRand = rnd.Next(0, 100);
            if (totesRand > 95)
            {
                score = rnd.Next(0, 40) / 100.0f;
            }
            else if (totesRand > 80)
            {
                score = rnd.Next(0, 75) / 100.0f;
            }
            else if (totesRand > 50)
            {
                score = rnd.Next(60, 110) / 100.0f;
            }
            else
            {
                score = rnd.Next(80, 120) / 100.0f;
            }
        }
        else if (Mathf.Abs(z) > anticipatedCubeCount * .0025)
        {
            score = rnd.Next(50, 100) / 100.0f;
        }
        else if (Mathf.Abs(z) > anticipatedCubeCount * .0015)
        {
            score = rnd.Next(25, 75) / 100.0f;
        }
        else if (Mathf.Abs(z) > anticipatedCubeCount * .0005)
        {
            score = rnd.Next(25, 50) / 100.0f;
        }
        else
        {
            int totesRand = rnd.Next(0, 100);
            if(totesRand >95)
            {
                score = rnd.Next(0, 120) / 100.0f;
            }
            else if(totesRand > 80)
            {
                score = rnd.Next(0, 75) / 100.0f;
            }
            else if (totesRand > 50)
            {
                score = rnd.Next(0, 40) / 100.0f;
            }
        }

        return score;
    }

    //use while loop to reach a number of tiles
    public IEnumerator DrunkenWalk()
    {
        Vector3 start = Vector3.zero;
        int maxX = 0, minX = 0, maxZ = 0, minZ = 0;
        int placed = 0;
        //float time = 0.0f;
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

        Dictionary<IntKey, GameObject> tileDictionary = new Dictionary<IntKey, GameObject>();

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
                if (!tileDictionary.ContainsKey(new IntKey((int)curPoint.x, (int)curPoint.z)))
                {
                    stepsWithoutPlacing = 0;
                    curPoint.y = SetHeight(rnd, curPoint, tileDictionary);
                   
                    GameObject tile = Instantiate<GameObject>(cubePrefab, curPoint, Quaternion.identity, tileParent.transform);
                    tile.transform.localScale = new Vector3(1, curPoint.y < 0.5f ? 1 : curPoint.y * 2, 1);
                    float biomeTemperatureScore = 0.0f;
                    if(elevationBased && nsBased)
                    {
                        biomeTemperatureScore += GetScoreFromElevation(tile.transform.localPosition.y);
                        biomeTemperatureScore += GetScoreFromNS(tile.transform.localPosition.z, rnd);
                    }
                    else if(nsBased)
                    {
                        biomeTemperatureScore += GetScoreFromNS(tile.transform.localPosition.z, rnd);
                    }
                    else
                    {
                        biomeTemperatureScore += GetScoreFromElevation(tile.transform.localPosition.y);
                    }

                    if (biomeTemperatureScore >= 1.0)
                    {
                        tile.GetComponent<MeshRenderer>().material = snowMaterial;
                    }
                    else if (biomeTemperatureScore >= 0.7)
                    {
                        tile.GetComponent<MeshRenderer>().material = forestMaterial;
                    }
                    else if (biomeTemperatureScore >= 0.4)
                    {
                        tile.GetComponent<MeshRenderer>().material = grassMaterial;
                    }
                    else
                    {
                        tile.GetComponent<MeshRenderer>().material = desertMaterial;
                    }
                    //tile.transform.localPosition = curPoint;
                    tiles.Add(tile);
                    tileDictionary.Add(new IntKey((int)curPoint.x, (int)curPoint.z), tile);
                    placed++;
                    if(curPoint.x < minX)
                    {
                        minX = (int)curPoint.x;
                    }
                    else if (curPoint.x > maxX)
                    {
                        maxX = (int)curPoint.x;
                    }
                    if (curPoint.z < minZ)
                    {
                        minZ = (int)curPoint.z;
                    }
                    else if(curPoint.z > maxZ)
                    {
                        maxZ = (int)curPoint.z;
                    }
                    
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
                //counterText.text = placed.ToString();
                //time += Time.deltaTime;
                //timeElapsed.text = time.ToString();
                
            }
            yield return null;
        }

        indexAtWhichThingsBecomeWater = tiles.Count - 1;

        List<GameObject> FullMap = tiles;
        for(int i = minX - waterBuffer; i <= maxX + waterBuffer; i++)
        {
            for(int j = minZ - waterBuffer; j <= maxZ + waterBuffer; j++)
            {
                if(i < minX || i > maxX || j < minZ || j > maxZ)
                {
                    GameObject water = Instantiate<GameObject>(waterPrefab, new Vector3(i, 0, j), Quaternion.identity, tileParent.transform);
                    //water.transform.localPosition = new Vector3(i, 0, j);
                    FullMap.Add(water);
                }
                else if (!tileDictionary.ContainsKey(new IntKey(i, j)))
                {
                    GameObject water = Instantiate<GameObject>(waterPrefab, new Vector3(i, 0, j), Quaternion.identity, tileParent.transform);
                    //water.transform.localPosition = new Vector3(i, 0, j);
                    FullMap.Add(water);
                }
                //time += Time.deltaTime;
                //timeElapsed.text = time.ToString();
                
            }
            yield return null;
        }

    }
}
