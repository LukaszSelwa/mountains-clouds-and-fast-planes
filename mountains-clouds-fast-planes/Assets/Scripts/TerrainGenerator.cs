﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    public TerrainData terrainData;
    float[,] heightmap;
    int resolution;
    public int seed = 2133;
    public int chainNumber = 100;
    public float chainProbability = 0.999F;
    public int chainSparsity = 8;
    public float scale = 2.0F;
    public float roughness = 0.005F;

    public float stErosionRate = 0.3F;
    public int stErosionDuration = 38;
    public float ndErosionRate = 0.1F;
    public int ndErosionDuration = 12;

    // Start is called before the first frame update
    void Start()
    {
        //Temporary, should be done elsewhere
        Random.InitState(seed);

        resolution = terrainData.heightmapResolution;
        heightmap = terrainData.GetHeights(0, 0, resolution, resolution);

        GenerateTerrainChained(); 
        terrainData.SetHeights(0, 0, heightmap);
    }

    float Sigmoid(float X)
    {
        return (float)(System.Math.Exp(X)/(System.Math.Exp(X) + 1));
    }

    void PutInRange()
    {
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] = Sigmoid(heightmap[i,j]);
        }
    }

    void ScaleHeightmap(float neutral, float factor)
    {
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] = ((heightmap[i,j] - neutral) * factor) + neutral;
        }
    }

    void GenerateTerrainChained()
    {
        SetNeutralHeight();

        GenerateChains(chainNumber, chainProbability, 0.01F, chainSparsity, -0.1F, 0.35F);
        
        PutInRange();
        Erode(stErosionRate, stErosionDuration);
        
        float neutral = Sigmoid(0.5F);
        ScaleHeightmap(neutral, scale);

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
                heightmap[i,j] += Random.Range(-roughness, roughness);
        }

        Erode(ndErosionRate, ndErosionDuration);
    }


    void GenerateChains(int chains, float chainingRate, float changeRate, int chainDist, float lowH, float highH)
    {
        for (int i = 0; i < chains; i++)
        {
            int r_x = Random.Range(0, resolution-1);
            int r_y = Random.Range(0, resolution-1);
            AddChainedPoint(r_x, r_y, lowH, highH, chainDist, chainingRate, changeRate);
        }
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
            return Sigmoid(0.5F);
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