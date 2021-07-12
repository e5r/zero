public class ConfigData
{
    private static ConfigData _defaultInstance;

    public string SolutionFile { get; set; }
    public string[] PublishGlobalExcludes { get; set; }
    public Artifact[] PublishArtifacts { get; set; }

    public static ConfigData Default => _defaultInstance ?? (_defaultInstance = new ConfigData());
}
