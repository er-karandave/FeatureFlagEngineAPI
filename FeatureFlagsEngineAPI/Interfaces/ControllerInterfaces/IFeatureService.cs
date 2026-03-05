using FeatureFlagsEngineAPI.Models;

namespace FeatureFlagsEngineAPI.Interfaces.ControllerInterfaces
{
    public interface IFeatureService
    {
        List<Feature> GetFeatureByMasterId(int idfeatureMasterId);
        List<Feature> GetAllActiveFeatures();
        List<Feature> GetAllInActiveFeatures();
        List<Feature> GetAllFeatures();
        List<Feature> GetFeatureByFeatureId(int IdFeature);
        StatusResponse UpdateFeatureStatus(int FeatureId, bool IsActive);
        object IsFeatureActive(int featureId);
        object DeleteFeatureById(int featureId);
    }
}
