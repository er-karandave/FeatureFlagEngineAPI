using FeatureFlagsEngineAPI.Interfaces.ControllerInterfaces;
using FeatureFlagsEngineAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var response = _featureService.GetFeatureByMasterId(IdFeature);

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
    }
}
