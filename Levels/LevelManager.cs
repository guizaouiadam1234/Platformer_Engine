using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


public struct LevelTile
{
    public Rectangle DestinationRect; // Screen position
    public Rectangle SourceRect;      // Grass.png position
}
public class LevelManager
{
    // A list to hold all the solid boxes we generate
    public List<Rectangle> SolidColliders { get; private set; }
    public List<LevelTile> VisualTiles { get; private set; }

    public LevelManager()
    {
        SolidColliders = new List<Rectangle>();
        VisualTiles = new List<LevelTile>();
    }

    public void LoadLevel(string filePath)
    {
        // 1. Read the JSON file text
        string jsonString = File.ReadAllText(filePath);

        // 2. Turn the text into our C# objects
        LdtkData worldData = JsonSerializer.Deserialize<LdtkData>(jsonString);

        // 3. Grab the very first level (Level_0)
        LdtkLevel level = worldData.levels[0];

        // 4. Find the Collisions Layer!
        LdtkLayer collisionLayer = null;
        foreach (var layer in level.layerInstances)
        {
            // 1. Did we find the Physics?
            if (layer.identifier.Equals("Collisions", StringComparison.OrdinalIgnoreCase))
            {
                int gridSize = layer.gridSize;
                int gridWidth = layer.cWid;
                if (gridWidth == 0) continue;

                for (int i = 0; i < layer.intGridCsv.Length; i++)
                {
                    if (layer.intGridCsv[i] > 0)
                    {
                        int xPos = (i % gridWidth) * gridSize;
                        int yPos = (i / gridWidth) * gridSize;
                        SolidColliders.Add(new Rectangle(xPos, yPos, gridSize, gridSize));
                    }
                }
            }
            // 2. Did we find the Art?
            else if (layer.identifier.Equals("Visuals", StringComparison.OrdinalIgnoreCase))
            {
                int gridSize = layer.gridSize;

                // Loop through the art instructions
                foreach (LdtkTile tile in layer.gridTiles)
                {
                    // Where on the screen does it go?
                    Rectangle destRect = new Rectangle(tile.px[0], tile.px[1], gridSize, gridSize);

                    // What part of the Grass.png image are we cutting out?
                    Rectangle srcRect = new Rectangle(tile.src[0], tile.src[1], gridSize, gridSize);

                    VisualTiles.Add(new LevelTile { DestinationRect = destRect, SourceRect = srcRect });
                }
            }
        }
    }
    }