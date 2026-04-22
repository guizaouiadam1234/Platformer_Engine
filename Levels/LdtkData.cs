using System.Text.Json.Serialization;

public class LdtkData
{
    [JsonPropertyName("levels")]
    public LdtkLevel[] levels { get; set; }
}

public class LdtkLevel
{
    [JsonPropertyName("identifier")]
    public string identifier { get; set; }

    [JsonPropertyName("pxWid")]
    public int pxWid { get; set; }

    [JsonPropertyName("pxHei")]
    public int pxHei { get; set; }

    [JsonPropertyName("layerInstances")]
    public LdtkLayer[] layerInstances { get; set; }
}

public class LdtkLayer
{
    [JsonPropertyName("__identifier")]
    public string identifier { get; set; }

    [JsonPropertyName("__gridSize")]
    public int gridSize { get; set; }

    [JsonPropertyName("__cWid")]
    public int cWid { get; set; }

    [JsonPropertyName("intGridCsv")]
    public int[] intGridCsv { get; set; }

    // --- ADD THIS BRAND NEW PROPERTY! ---
    [JsonPropertyName("gridTiles")]
    public LdtkTile[] gridTiles { get; set; }
}

// --- ADD THIS BRAND NEW CLASS AT THE BOTTOM! ---
public class LdtkTile
{
    [JsonPropertyName("px")]
    public int[] px { get; set; } // Where to draw it on the screen [X, Y]

    [JsonPropertyName("src")]
    public int[] src { get; set; } // What part of Grass.png to cut out [X, Y]
}