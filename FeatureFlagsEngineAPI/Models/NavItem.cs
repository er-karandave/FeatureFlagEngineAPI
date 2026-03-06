namespace FeatureFlagsEngineAPI.Models
{
    public class NavItem
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool Exact { get; set; } = false;
        public int? ParentId { get; set; }
        public int? DimMasterId { get; set; }
        public List<NavItem> Children { get; set; } = new();
        public bool HasPermission { get; set; } = true;
    }

    public class NavigationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<NavItem> Data { get; set; } = new();
    }
}
