using Newtonsoft.Json;
using Saving;

public class Level
{
    public LevelType levelType;
    public Matrix matrix;

    //[JsonIgnore]
    public float directionalLightIntensity = 1;
    [JsonIgnore]
    public float sphereLightIntensity;
    [JsonIgnore]
    public float sphereLightRange;

    public int timeForFTLlevel;
}
