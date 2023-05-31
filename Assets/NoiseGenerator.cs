using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public static float[,] Generate(int width, int height, float scale, Wave[] waves, Vector2 offset)
    {
        // create the noise map
        float[,] noiseMap = new float[width, height];
        // loop through each element in the noise map
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                // calculate the sample positions
                float samplePosX = x * scale + offset.x;
                float samplePosY = y * scale + offset.y;
                float normalization = 0.0f;
                // loop through each wave
                foreach (Wave wave in waves)
                {
                    // sample the perlin noise taking into consideration amplitude and frequency
                    var _x = samplePosX * wave.frequency + wave.seed;
                    var _y = samplePosY * wave.frequency + wave.seed;
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(_x, _y);
                    normalization += wave.amplitude;
                    
                }
                // normalize the value
                noiseMap[x, y] /= normalization;
            }
        }

        return noiseMap;
    }
}
