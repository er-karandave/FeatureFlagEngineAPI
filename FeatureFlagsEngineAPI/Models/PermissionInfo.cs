namespace FeatureFlagsEngineAPI.Models
{

    public class PermissionInfo
    {
        public int IdPermission { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionDetails { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty; 
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
