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
                    Link = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
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

            string query = @"SELECT IdFeature, FeatureName, FeatureDetails,FeatureDisplayName,Link
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
                    Link = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
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
                     WHERE IdFeature = @idfeatureMasterId
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
    }
}
