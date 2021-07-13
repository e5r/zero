public class ConfigFile
{
    private static ConfigFile _defaultInstance;

    public string SolutionFile { get; set; }
    public string[] PublishGlobalExcludes { get; set; }
    public Artifact[] PublishArtifacts { get; set; }
    public Artifact[] TestArtifacts { get; set; }

    public static ConfigFile Default => _defaultInstance ?? (_defaultInstance = new ConfigFile());
}
