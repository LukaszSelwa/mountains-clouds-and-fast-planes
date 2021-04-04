﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public TerrainData terrainData;
    public float[,] heightmap;
    public int resolution;

    // Start is called before the first frame update
    void Start()
    {
        //Temporary, should be done elsewhere
        Random.InitState(2133);

        resolution = terrainData.heightmapResolution;
        heightmap = terrainData.GetHeights(0, 0, resolution, resolution);

        GenerateTerrainExperimental(); 
        terrainData.SetHeights(0, 0, heightmap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateTerrainSimple()
    {
        SetNeutralHeight();
        for (int l = 0; l < 1; l++)
        {
            AddRandomPoints(50000, 0.0F, 1.0F);
            Erode(0.01F, 1000);
        }
        terrainData.SetHeights(0, 0, heightmap);
    }

    void GenerateTerrainExperimental()
    {
        SetNeutralHeight();

        GenerateChains(10, 0.999F, 0.01F, 4, -0.15F, -0.1F, 80, 0.8F);
        GenerateChains(100, 0.999F, 0.01F, 8, -0.1F, 0.35F, 100, 0.4F);
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] += Random.Range(-0.006F, 0.006F);
        }

        Erode(0.1F, 12);
    }

    void GenerateChains(int chains, float chainingRate, float changeRate, int chainDist, float lowH, float highH, int epochs, float erosionRate)
    {
        for (int i = 0; i < chains; i++)
        {
            int r_x = Random.Range(0, resolution-1);
            int r_y = Random.Range(0, resolution-1);
            AddChainedPoint(r_x, r_y, lowH, highH, chainDist, chainingRate, changeRate);
        }

        Erode(erosionRate, epochs);
    }

    void SetNeutralHeight()
    {
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] = 0.5F;
    }

    void Erode(float rate, int epochs)
    {
        for (int k = 0; k < epochs; k++)
        {
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    float n1 = GetCell(i-1, j) + GetCell(i+1, j);
                    float n2 = GetCell(i, j-1) + GetCell(i, j+1);
                    heightmap[i,j] = ((1.0F-rate)*heightmap[i,j] + rate*((n1 + n2)/4.0F));
                }
            }
        }
    }

    float GetCell(int x, int y)
    {
        if(x<0 || x >= resolution || y < 0 || y >= resolution)
            return 0.5F;
        return heightmap[x,y];
    }

    void AddRandomPoints(int num, float rangeLow, float rangeHigh)
    {
        for(int k = 0; k<num; k++)
        {
            int i = Random.Range(0, resolution);
            int j = Random.Range(0, resolution);
            heightmap[i,j] = Random.Range(rangeLow, rangeHigh);
        }
    }

    void AddChainedPoint(int x, int y, float rangeLow, float rangeHigh, int chainDist, float chainProb, float levelChange)
    {
        float heightChange = Random.Range(rangeLow, rangeHigh);
        heightmap[x, y] = heightmap[x,y] + heightChange;
        var positionChange = Random.insideUnitCircle * chainDist;

        int x_new = x + (int)positionChange.x;
        int y_new = y + (int)positionChange.y;

        x_new = (x_new + resolution) % resolution;
        y_new = (y_new + resolution) % resolution;

        if (Random.Range(0.0F, 1.0F) < chainProb)
            AddChainedPoint(x_new, y_new, heightChange - levelChange, heightChange + levelChange, chainDist, chainProb, levelChange);
    }
}