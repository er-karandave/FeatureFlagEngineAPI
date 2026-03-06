using FeatureFlagsEngineAPI.Interfaces.ControllerInterfaces;
using FeatureFlagsEngineAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace FeatureFlagsEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureService _featureService;
        public FeatureController(IFeatureService featureService)
        {
            _featureService = featureService;
        }
        [HttpPost("getFeaturesByFeatureMasterId")]
        public IActionResult GetFeaturesByFeatureMasterId(int idfeatureMasterId)
        {
            var response = _featureService.GetFeatureByMasterId(idfeatureMasterId);

            if (response == null || !response.Any())
                return NotFound("No features found");

            return Ok(response);
        }

        [HttpPost("getFeaturesByFeatureId")]
        public IActionResult getFeaturesByFeatureId(int IdFeature)
        {
            if (IdFeature <= 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid feature ID." });
            }

            var feature = _featureService.GetFeatureByFeatureId(IdFeature);

            if (feature == null)
            {
                return NotFound(new { Success = false, Message = "Feature not found." });
            }

            return Ok(new { Success = true, Message = "Feature retrieved successfully.", Data = feature });
        }

        [HttpPost("getAllActiveFeatures")]
        public IActionResult GetAllActiveFeatures()
        {
            var response = _featureService.GetAllActiveFeatures();

            if (response == null || !response.Any())
                return NotFound("No features found");

            return Ok(response);
        }

        [HttpPost("getAllInActiveFeatures")]
        public IActionResult GetAllInActiveFeatures()
        {
            var response = _featureService.GetAllInActiveFeatures();

            if (response == null || !response.Any())
                return NotFound("No features found");

            return Ok(response);
        }

        [HttpPost("getAllFeatures")]
        public IActionResult GetAllFeatures(bool includeInactive = false)
        {
            var response = _featureService.GetAllFeatures(includeInactive);

            if (response == null || !response.Any())
                return NotFound("No features found");

            return Ok(response);
        }

        [HttpPost("updateFeatureStatus")]
        public IActionResult UpdateFeatureStatus(int FeatureId, bool IsActive)
        {
            var response = _featureService.UpdateFeatureStatus(FeatureId, IsActive);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("{featureId}/status")]
        public IActionResult IsFeatureActive(int featureId)
        {
            var response = _featureService.IsFeatureActive(featureId);

            dynamic result = response;

            if (result.IsActive)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("deleteFeatureById")]
        public IActionResult DeleteFeature(int featureId)
        {
            if (featureId <= 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid feature ID." });
            }

            var response = _featureService.DeleteFeatureById(featureId);
            dynamic result = response;

            if (result.IsActive)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
