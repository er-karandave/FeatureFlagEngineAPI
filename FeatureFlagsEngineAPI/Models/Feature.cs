namespace FeatureFlagsEngineAPI.Models
{
    public class Feature
    {
        public int IdFeature {  get; set; }
        public string FeatureName { get; set; }
        public string FeatureDisplayName { get; set; }
        public string FeatureDetails { get; set; }
        public string? Link { get; set; }
        public bool IsActive { get; set; }

    }

    public class StatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
