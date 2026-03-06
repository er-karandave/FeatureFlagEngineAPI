using FeatureFlagsEngineAPI.Interfaces.DataLayerInterfaces;
using FeatureFlagsEngineAPI.Models;
using Microsoft.Data.SqlClient;

namespace FeatureFlagsEngineAPI.Services
{
    public class NavigationService: INavigationService
    {
        private readonly string _connectionString;

        public NavigationService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }


        public async Task<List<NavItem>> GetNavigationItemsAsync(int userId, List<string> userPermissions)
        {
            var navItems = new List<NavItem>();

            try
            {

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"
            SELECT 
                f.IdFeature,
                f.FeatureName,
                f.FeatureDisplayName,
                f.Link,
                f.DimFeatureMasterId,
                dm.FeatureDesc AS MasterName,
                dm.IddimFeatureMaster AS MasterId
            FROM tblFeatures f
            LEFT JOIN tbldimFeatureMaster dm ON f.DimFeatureMasterId = dm.IddimFeatureMaster
            WHERE f.IsActive = 1 
            AND (dm.IsActive = 1 OR dm.IsActive IS NULL)
            ORDER BY dm.IddimFeatureMaster, f.FeatureName";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                var features = new List<dynamic>();
                int rowCount = 0;

                while (await reader.ReadAsync())
                {
                    rowCount++;

                    var feature = new
                    {
                        IdFeature = GetSafeInt32(reader, 0),
                        FeatureName = GetSafeString(reader, 1),
                        FeatureDisplayName = GetSafeString(reader, 2),
                        Link = GetSafeString(reader, 3),
                        DimFeatureMasterId = GetSafeInt32(reader, 4),
                        MasterName = GetSafeString(reader, 5),
                        MasterId = GetSafeInt32(reader, 6)
                    };

                    features.Add(feature);
                }


                if (features.Count == 0)
                {
                    return GetDefaultNavItems();
                }

                var featuresWithValidMaster = features.ToList();

                var masterGroups = featuresWithValidMaster
                    .GroupBy(f => new { f.MasterId, f.MasterName })
                    .ToList();


                foreach (var master in masterGroups)
                {
                    try
                    {
                        var masterId = master.Key.MasterId ?? 0;
                        var masterName = master.Key.MasterName ?? "Other";

                        var masterPermissionCode = $"MODULE_{masterName.Replace(" ", "_").ToUpper()}";

                        bool hasMasterPermission = userPermissions?.Any(p =>
                            p.Contains("ADMIN", StringComparison.OrdinalIgnoreCase) ||
                            p.Contains("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                            masterId == 1
                        ) ?? false;

                        if (!hasMasterPermission && masterId != 1)
                        {
                            continue;
                        }

                        var parentItem = new NavItem
                        {
                            Id = masterId,
                            Label = masterName,
                            Link = master.First()?.Link ?? string.Empty,
                            Icon = GetIconForModule(masterId),
                            ParentId = null,
                            DimMasterId = masterId,
                            HasPermission = hasMasterPermission,
                            Children = new List<NavItem>()
                        };

                        foreach (var feature in master)
                        {
                            var featureLink = feature.Link ?? string.Empty;
                            var featurePermissionCode = GetPermissionCodeFromLink(featureLink);

                            bool hasFeaturePermission = userPermissions?.Any(p =>
                                p.Contains("ADMIN", StringComparison.OrdinalIgnoreCase) ||
                                p.Contains("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                                p.Contains(featurePermissionCode, StringComparison.OrdinalIgnoreCase) ||
                                featurePermissionCode.Contains("VIEW", StringComparison.OrdinalIgnoreCase)
                            ) ?? true;

                            if (hasFeaturePermission || string.IsNullOrEmpty(featurePermissionCode))
                            {
                                parentItem.Children.Add(new NavItem
                                {
                                    Id = feature.IdFeature,
                                    Label = feature.FeatureDisplayName ?? string.Empty,
                                    Link = featureLink,
                                    Icon = GetIconForFeature(featureLink),
                                    ParentId = parentItem.Id,
                                    Exact = true,
                                    HasPermission = hasFeaturePermission
                                });
                            }
                        }

                        if (parentItem.Children.Any())
                        {
                            navItems.Add(parentItem);
                           
                        }
                    }
                    catch (Exception innerEx)
                    {
                       
                        continue;
                    }
                }
                var standaloneFeatures = features.ToList();


                foreach (var feature in standaloneFeatures)
                {
                    navItems.Add(new NavItem
                    {
                        Id = feature.IdFeature,
                        Label = feature.FeatureDisplayName ?? string.Empty,
                        Link = feature.Link ?? string.Empty,
                        Icon = GetIconForFeature(feature.Link ?? string.Empty),
                        ParentId = null,
                        Exact = true
                    });
                }

                navItems.Insert(0, new NavItem
                {
                    Id = 0,
                    Label = "Dashboard",
                    Link = "/dashboard",
                    Icon = "bx bx-home",
                    Exact = true
                });

                return navItems;
            }
            catch (SqlException sqlEx)
            {
                Console.Error.WriteLine($"❌ SQL Error: {sqlEx.Message}");
                Console.Error.WriteLine($"❌ SQL Error Number: {sqlEx.Number}");
                return GetDefaultNavItems();
            }
            catch (NullReferenceException nullEx)
            {
                return GetDefaultNavItems();
            }
            catch (Exception ex)
            {
                return GetDefaultNavItems();
            }
        }

        private int GetSafeInt32(SqlDataReader reader, int ordinal) =>
            reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);

        private int? GetSafeNullableInt32(SqlDataReader reader, int ordinal) =>
            reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);

        private string GetSafeString(SqlDataReader reader, int ordinal) =>
            reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);

        private string GetIconForModule(int? masterId) => masterId switch
        {
            1 => "bx bx-user",
            2 => "bx bx-group",
            3 => "bx bx-key",
            4 => "bx bx-flag",
            5 => "bx bx-bar-chart",
            6 => "bx bx-cog",
            _ => "bx bx-folder"
        };
        private string GetIconForFeature(string link)
        {
            if (link.Contains("user")) return "bx bx-user";
            if (link.Contains("role")) return "bx bx-group";
            if (link.Contains("permission")) return "bx bx-key";
            if (link.Contains("feature")) return "bx bx-flag";
            if (link.Contains("report")) return "bx bx-bar-chart";
            if (link.Contains("dashboard")) return "bx bx-home";
            if (link.Contains("setting")) return "bx bx-cog";
            return "bx bx-link";
        }

        private string GetPermissionCodeFromLink(string link)
        {
            if (link.Contains("add")) return "CREATE";
            if (link.Contains("edit")) return "EDIT";
            if (link.Contains("delete")) return "DELETE";
            if (link.Contains("list") || link.Contains("view")) return "VIEW";
            return "VIEW";
        }

        private List<NavItem> GetDefaultNavItems()
        {
            return new List<NavItem>
            {
                new NavItem { Id = 0, Label = "Dashboard", Link = "/dashboard", Icon = "bx bx-home", Exact = true },
                new NavItem {
                    Id = 1,
                    Label = "User Management",
                    Link = "/user-list",
                    Icon = "bx bx-user",
                    Children = new List<NavItem>
                    {
                        new NavItem { Id = 11, Label = "User List", Link = "/user-list", Icon = "bx bx-list-ul", ParentId = 1 },
                        new NavItem { Id = 12, Label = "Add User", Link = "/add-user", Icon = "bx bx-user-plus", ParentId = 1 }
                    }
                }
            };
        }
    }
}
