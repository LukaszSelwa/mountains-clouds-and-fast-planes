﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

public class TerrainGenerator : MonoBehaviour
{
    public Terrain firstTerrain;
    public TerrainData terrainData;
    float[,] heightmap;
    int resolution;
    public int seed = 2133;
    public int terrainSize = 1000;
    public int chainNumber = 100;
    public float chainProbability = 0.999F;
    public int chainSparsity = 8;
    public float scale = 2.0F;
    public float roughness = 0.005F;

    public float stErosionRate = 0.3F;
    public int stErosionDuration = 38;
    public float ndErosionRate = 0.1F;
    public int ndErosionDuration = 12;

    Thread workingThread;
    bool waitingForResult;
    System.Tuple<int,int> resultCoordinates;
    Dictionary<System.Tuple<int,int>, Terrain> grid;
    System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        //Temporary, should be done elsewhere
        Random.InitState(seed);
        random = new System.Random();
        grid = new Dictionary<System.Tuple<int, int>, Terrain>();

        resolution = terrainData.heightmapResolution;
        heightmap = terrainData.GetHeights(0, 0, resolution, resolution);

        GenerateTerrainChained(); 
        terrainData.SetHeights(0, 0, heightmap);
        grid.Add(new System.Tuple<int, int>(0,0), Terrain.activeTerrain);
        waitingForResult = false;
    }

    float Sigmoid(float X)
    {
        return (float)(System.Math.Exp(X)/(System.Math.Exp(X) + 1));
    }

    Terrain createTerrain(Vector3 position)
    {
        TerrainData td = Instantiate(terrainData);
        GameObject t = Terrain.CreateTerrainGameObject(td);
        t.transform.position = 1000 * position;
        t.GetComponent<Terrain>().materialTemplate = Terrain.activeTerrain.materialTemplate;
        return t.GetComponent<Terrain>();
    }
    void Update()
    {
        if(waitingForResult)
        {
            if(!workingThread.IsAlive)
            {
                grid.Add(new System.Tuple<int, int>(resultCoordinates.Item1, resultCoordinates.Item2), createTerrain(new Vector3(resultCoordinates.Item1, -0.3F, resultCoordinates.Item2)));
                grid[new System.Tuple<int, int>(resultCoordinates.Item1, resultCoordinates.Item2)].terrainData.SetHeights(0,0,heightmap);
                waitingForResult = false;
            }
            return;
        }

        GameObject Plane = GameObject.Find("PlayerPlane");
        int x = (int)System.Math.Floor(Plane.transform.position.x / terrainSize);
        int z = (int)System.Math.Floor(Plane.transform.position.z / terrainSize);
        int tileRange = 2;

        for(int i=-tileRange;i<=tileRange;i++)
        {
            for(int j=-(tileRange - System.Math.Abs(i));j <= tileRange - System.Math.Abs(i);j++)
            {
                if(!grid.ContainsKey(new System.Tuple<int, int>(x+i, z+j)))
                {
                    resultCoordinates = new System.Tuple<int, int>(x+i, z+j);
                    workingThread = new Thread(GenerateTerrainChained);
                    workingThread.Start();
                    waitingForResult = true;
                    return;
                }
            }
        }
    }

    bool inSquare(Vector3 pos, Vector3 corner, int side)
    {
        if(pos.x < corner.x || pos.x > corner.x + side)
            return false;
        return !(pos.z < corner.z || pos.z > corner.z + side);
    }

    /*void Clamp()
    {
        if(t.rightNeighbor != null)
        {
            float[,] tp = t.rightNeighbor.terrainData.GetHeights(0, 0, resolution, resolution);
            for(int i=0;i<resolution;i++)
                hm[i, resolution-1] = tp[i, 0];
        }

        if(t.leftNeighbor != null)
        {
            float[,] tp = t.leftNeighbor.terrainData.GetHeights(0, 0, resolution, resolution);
            for(int i=0;i<resolution;i++)
                hm[i, 0] = tp[i, resolution-1];
        }

        if(t.topNeighbor != null)
        {
            float[,] tp = t.topNeighbor.terrainData.GetHeights(0, 0, resolution, resolution);
            for(int i=0;i<resolution;i++)
                hm[0, i] = tp[resolution-1, i];
        }

        if(t.bottomNeighbor != null)
        {
            float[,] tp = t.bottomNeighbor.terrainData.GetHeights(0, 0, resolution, resolution);
            for(int i=0;i<resolution;i++)
                hm[resolution-1, i] = tp[0, i];
        }

        if(t.bottomNeighbor != null)
        {
            float[,] tp = t.terrainData.GetHeights(0, 0, resolution, resolution);
            for(int i=0;i<resolution;i++)
            {
                print(hm[resolution-1, i] + " " + tp[resolution-1, i]);
            }
        }
    }*/

    void PutInRange()
    {
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] = Sigmoid(heightmap[i,j]);
    }

    void ScaleHeightmap(float neutral, float factor)
    {
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] = ((heightmap[i,j] - neutral) * factor) + neutral;
    }

    void GenerateTerrainChained()
    {
        SetNeutralHeight(0.0F);
        //Clamp();
        GenerateChains(chainNumber, chainProbability, 0.01F, chainSparsity, -0.005F, 0.02F);
        
        PutInRange();

        var list = new List<System.Tuple<int,int>>();
        for(int i = 0;i<resolution;i++)
        {
            for(int j = 0;j<resolution;j++)
            {
                list.Add(new System.Tuple<int, int>(i,j));
            }
        }

        var randomOrder = list.OrderBy(x => random.Next()).ToList();

        foreach(System.Tuple<int,int> p in randomOrder)
        {
                int i = p.Item1;
                int j = p.Item2;
                if(System.Math.Abs(heightmap[i, j] - 0.5F) > 0.01F) 
                {
                    for(int k=i;k<resolution && k < i+5;k++)
                    {
                        for(int l=j;l<resolution && l < j+5;l++)
                        {
                            if(System.Math.Abs(heightmap[k, l] - 0.5F) < 0.01F)
                                heightmap[k, l] = heightmap[i, j];
                        }
                    }
                }
        }

        Erode(stErosionRate, stErosionDuration);
        
        ScaleHeightmap(0.5F, scale);

        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] += (float)random.NextDouble()*roughness;

        Erode(ndErosionRate, ndErosionDuration);
    }


    void GenerateChains(int chains, float chainingRate, float changeRate, int chainDist, float lowH, float highH)
    {
        for (int i = 0; i < chains; i++)
        {
            int r_x = random.Next(resolution);
            int r_y = random.Next(resolution);
            AddChainedPoint(r_x, r_y, lowH, highH, chainDist, chainingRate, changeRate);
        }
    }

    void SetNeutralHeight(float val)
    {
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] = val;
    }

    void Erode(float rate, int epochs, bool fill=false)
    {
        for (int k = 0; k < epochs; k++)
        {
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    float sum = 0.0F;
                    int num = 0;
                    if(ValidCell(i-1, j))
                    {
                        sum += heightmap[i-1, j];
                        num ++;
                    }

                    if(ValidCell(i+1, j))
                    {
                        sum += heightmap[i+1, j];
                        num ++;
                    }

                    if(ValidCell(i, j-1))
                    {
                        sum += heightmap[i, j-1];
                        num ++;
                    }

                    if(ValidCell(i, j+1))
                    {
                        sum += heightmap[i, j+1];
                        num ++;
                    }

                    if(!fill)
                        heightmap[i,j] = ((1.0F-rate)*heightmap[i,j] + rate*(sum/num));
                    else if(System.Math.Abs(heightmap[i,j] - 0.5F) < 0.02F)
                        heightmap[i,j] = sum/num;
                }
            }
        }
    }

    bool ValidCell(int x, int y)
    {
        return !(x<0 || x >= resolution || y < 0 || y >= resolution);
    }

    Vector2 randInUnitCircle()
    {
        Vector2 res = new Vector2();
        do {
            res.x = (float)random.NextDouble() - 0.5F;
            res.y = (float)random.NextDouble() - 0.5F;
        } while(res.magnitude > 1.0F);
        return res;
    }

    void AddChainedPoint(int x, int y, float rangeLow, float rangeHigh, int chainDist, float chainProb, float levelChange)
    {
        float heightChange = (float)random.NextDouble()*(rangeHigh-rangeLow)+rangeLow;
        heightmap[x, y] = heightmap[x,y] + (1.0F - 2.0F*System.Math.Abs(heightmap[x,y])) * heightChange;
        
        var positionChange = randInUnitCircle() * chainDist;

        int x_new = x + (int)positionChange.x;
        int y_new = y + (int)positionChange.y;

        x_new = (x_new + resolution) % resolution;
        y_new = (y_new + resolution) % resolution;

        float lc = 2.0F*((float)random.NextDouble()-0.5F)*levelChange;

        if (random.NextDouble() < chainProb)
            AddChainedPoint(x_new, y_new, heightChange - lc, heightChange + lc, chainDist, chainProb, levelChange);
    }
}