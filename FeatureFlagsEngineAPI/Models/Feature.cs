namespace FeatureFlagsEngineAPI.Models
{

    public class Feature
    {
        public int IdFeature { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureDetails { get; set; } = string.Empty;
        public string FeatureDisplayName { get; set; } = string.Empty;
        public string? Link { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public int? ActiveStatusChangedBy { get; set; }
        public DateTime? ActiveStatusChangedOn { get; set; }
    }

    public class StatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
