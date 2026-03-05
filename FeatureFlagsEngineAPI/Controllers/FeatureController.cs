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
            var response = _featureService.GetFeatureByFeatureId(IdFeature);

            if (response == null || !response.Any())
                return NotFound("No features found");

            return Ok(response);
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
        public IActionResult GetAllFeatures()
        {
            var response = _featureService.GetAllFeatures();

            if (response == null || !response.Any())
                return NotFound("No features found");

            return Ok(response);
        }

        [HttpPost("updateFeatureStatus")]
        public IActionResult UpdateFeatureStatus(int FeatureId, bool IsActive)
        {
            var response = _featureService.UpdateFeatureStatus(FeatureId,IsActive);

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
            var result = _featureService.IsFeatureActive(featureId);

            if (result is SimpleFeatureResponse response)
            {
                return response.IsActive
                    ? Ok(response)
                    : BadRequest(response);
            }

            return StatusCode(500, new { message = "Unexpected response type" });
        }

        [HttpPost("deleteFeatureById")]  
        public IActionResult DeleteFeature(int featureId)
        {
            if (featureId <= 0)
            {
                return BadRequest(new SimpleFeatureResponse
                {
                    IsActive = false,
                    Message = "Invalid feature ID."
                });
            }

            var result = _featureService.DeleteFeatureById(featureId);

            if (result is SimpleFeatureResponse response)
            {
                return response.IsActive
                    ? Ok(response)
                    : BadRequest(response);
            }

            return StatusCode(500, new { message = "Unexpected response type" });
        }
    }
}
