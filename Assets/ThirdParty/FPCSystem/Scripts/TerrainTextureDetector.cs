//==========================================================================================================
//
// http://answers.unity3d.com/questions/34328/terrain-with-multiple-splat-textures-how-can-i-det.html
//
// Here is my JavaScript translation of the same cs code: Thanks Ben! (works splendidly) !
//  -- TerrainSurface.js -- // // Ben Pitt // // JS translation by Steven Mikkelson @MartianGames :) //
//
//==========================================================================================================

using UnityEngine;
using System.Collections;

public class TerrainTextureDetector : MonoBehaviour
{
    
    // Test & Debug purposes internal commented code.
    /*private int index = 0;


	void Update()
	{	
	   index = GetMainTexture(this.transform.position);
	}*/

    // Returns an array containing the relative mix of textures on the main terrain at this world position.
    // The number of values in the array will equal the number of textures added to the terrain.
    public static float[] GetTextureMix(Vector3 worldPos)
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        
		// calculate which splat map cell the worldPos falls within (ignoring y)
        int mapX = (int) (((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int) (((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        
		// get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        
		// extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
        int n = 0;
        while (n < cellMix.Length)
        {
            cellMix[n] = splatmapData[0, 0, n];
            ++n;
        }
        return cellMix;
    }

    // returns the zero-based index of the most dominant texture on the main terrain at this world position.
    public static int GetMainTexture(Vector3 worldPos)
    {
        float[] mix = TerrainTextureDetector.GetTextureMix(worldPos);
        float maxMix = 0f;
        int maxIndex = 0;
        
		// loop through each mix value and find the maximum
        int n = 0;
        while (n < mix.Length)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
            ++n;
        }
        return maxIndex;
    }

}