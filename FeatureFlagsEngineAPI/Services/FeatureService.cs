using FeatureFlagsEngineAPI.Interfaces.ControllerInterfaces;
using FeatureFlagsEngineAPI.Models;
using Microsoft.Data.SqlClient;

namespace FeatureFlagsEngineAPI.Services
{
    public class FeatureService : IFeatureService
    {

        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public FeatureService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Feature> GetAllActiveFeatures()
        {
            List<Feature> features = new List<Feature>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT IdFeature, FeatureName, FeatureDetails,FeatureDisplayName,Link
                     FROM tblFeatures
                     WHERE IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Feature feature = new Feature
                {
                    IdFeature = reader.GetInt32(0),
                    FeatureName = reader.GetString(1),
                    FeatureDetails = reader.GetString(2),
                    FeatureDisplayName = reader.GetString(3),
                    Link = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    IsActive = reader.GetBoolean(5)
                };

                features.Add(feature);
            }

            return features;
        }

        public List<Feature> GetAllFeatures()
        {
            List<Feature> features = new List<Feature>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT IdFeature, FeatureName, FeatureDetails,FeatureDisplayName,Link,IsActive
                     FROM tblFeatures";

            using SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Feature feature = new Feature
                {
                    IdFeature = reader.GetInt32(0),
                    FeatureName = reader.GetString(1),
                    FeatureDetails = reader.GetString(2),
                    FeatureDisplayName = reader.GetString(3),
                    Link = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    IsActive = reader.GetBoolean(5)
                };

                features.Add(feature);
            }

            return features;
        }

        public List<Feature> GetAllInActiveFeatures()
        {
            List<Feature> features = new List<Feature>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT IdFeature, FeatureName, FeatureDetails,FeatureDisplayName,Link
                     FROM tblFeatures
                     WHERE IsActive = 0";

            using SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Feature feature = new Feature
                {
                    IdFeature = reader.GetInt32(0),
                    FeatureName = reader.GetString(1),
                    FeatureDetails = reader.GetString(2),
                    FeatureDisplayName = reader.GetString(3),
                    Link = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };

                features.Add(feature);
            }

            return features;
        }

        public List<Feature> GetFeatureByMasterId(int idfeatureMasterId)
        {
            List<Feature> features = new List<Feature>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT IdFeature, FeatureName, FeatureDetails,FeatureDisplayName,Link
                     FROM tblFeatures
                     WHERE DimFeatureMasterId = @idfeatureMasterId
                     AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@idfeatureMasterId", idfeatureMasterId);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Feature feature = new Feature
                {
                    IdFeature = reader.GetInt32(0),
                    FeatureName = reader.GetString(1),
                    FeatureDetails = reader.GetString(2),
                    FeatureDisplayName = reader.GetString(3),
                    Link = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };

                features.Add(feature);
            }

            return features; // returns empty list if no data
        }

        public List<Feature> GetFeatureByFeatureId(int IdFeature)
        {
            List<Feature> features = new List<Feature>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT IdFeature, FeatureName, FeatureDetails,FeatureDisplayName,Link
                     FROM tblFeatures
                     WHERE IdFeature = @IdFeature
                     AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdFeature", IdFeature);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Feature feature = new Feature
                {
                    IdFeature = reader.GetInt32(0),
                    FeatureName = reader.GetString(1),
                    FeatureDetails = reader.GetString(2),
                    FeatureDisplayName = reader.GetString(3),
                    Link = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };

                features.Add(feature);
            }

            return features; // returns empty list if no data
        }

        //public bool UpdateFeatureStatus(int FeatureId, int IsActive)
        //{
        //    using SqlConnection connection = new SqlConnection(_connectionString);
        //    connection.Open();

        //    string query = @"UPDATE tblFeatures 
        //             SET IsActive = @IsActive,
        //                 ActiveStatusChangedOn = GETDATE()
        //             WHERE IdFeature = @IdFeature";

        //    using SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@IsActive", IsActive);
        //    command.Parameters.AddWithValue("@IdFeature", FeatureId);

        //    int rowsAffected = command.ExecuteNonQuery();
        //    return rowsAffected > 0; // true if update succeeded
        //}

        public StatusResponse UpdateFeatureStatus(int FeatureId, bool IsActive)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                // ✅ Simple UPDATE query - only IsActive field
                string query = @"UPDATE tblFeatures 
                     SET IsActive = @IsActive,
                         ActiveStatusChangedOn = GETDATE()
                     WHERE IdFeature = @IdFeature";

                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IsActive", IsActive);
                command.Parameters.AddWithValue("@IdFeature", FeatureId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new StatusResponse
                    {
                        Success = true,
                        Message = "Successfully changed status."
                    };
                }
                else
                {
                    return new StatusResponse
                    {
                        Success = false,
                        Message = "Feature not found or no changes made."
                    };
                }
            }
            catch (Exception ex)
            {

                return new StatusResponse
                {
                    Success = false,
                    Message = "Error: " + ex.Message
                };
            }
        }

        public object IsFeatureActive(int featureId)  // Keep return type as object for compatibility
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                string query = @"SELECT IsActive, FeatureDisplayName 
                            FROM tblFeatures 
                            WHERE [IdFeature] = @FeatureId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FeatureId", featureId);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    bool isActive = reader.GetBoolean(0);
                    string featureName = reader.GetString(1);

                    // ✅ Return strongly-typed object (but as 'object')
                    return new SimpleFeatureResponse
                    {
                        IsActive = isActive,
                        Message = isActive
                            ? $""
                            : $"Feature '{featureName}' is currently inactive."
                    };
                }

                return new SimpleFeatureResponse
                {
                    IsActive = false,
                    Message = "Feature not found."
                };
            }
            catch (Exception ex)
            {
                return new SimpleFeatureResponse
                {
                    IsActive = false,
                    Message = "Error checking feature status."
                };
            }
        }

        public object DeleteFeatureById(int featureId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                string selectQuery = @"SELECT IsActive, FeatureDisplayName 
                              FROM tblFeatures 
                              WHERE [IdFeature] = @FeatureId";

                using var selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@FeatureId", featureId);

                using var reader = selectCommand.ExecuteReader();

                if (!reader.Read())
                {
                    return new SimpleFeatureResponse
                    {
                        IsActive = false,
                        Message = "Feature not found."
                    };
                }

                bool isActive = reader.GetBoolean(0);
                string featureName = reader.GetString(1);
                reader.Close(); 

                string deleteQuery = @"DELETE FROM tblFeatures WHERE [IdFeature] = @FeatureId";

                using var deleteCommand = new SqlCommand(deleteQuery, connection);
                deleteCommand.Parameters.AddWithValue("@FeatureId", featureId);

                int rowsAffected = deleteCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new SimpleFeatureResponse
                    {
                        IsActive = true,
                        Message = $"Feature '{featureName}' deleted successfully."
                    };
                }
                else
                {
                    return new SimpleFeatureResponse
                    {
                        IsActive = false,
                        Message = "Failed to delete feature. Please try again."
                    };
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 547)
            {
                return new SimpleFeatureResponse
                {
                    IsActive = false,
                    Message = "Cannot delete: Feature is referenced by other data."
                };
            }
            catch (Exception ex)
            {
                return new SimpleFeatureResponse
                {
                    IsActive = false,
                    Message = "Error deleting feature. Please try again."
                };
            }
        }
    }
}
