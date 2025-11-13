using MatrixN3HealthManager.Main;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MatrixN3HealthManager.DTOs;
using N3HealthManager.DTOs;

namespace MatrixN3HealthManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class N3Controller(IN3HealthService n3HealthService) : ControllerBase
    {
        [HttpPost("addMedRecord")]
        public async Task<IActionResult> AddMedRecord(MedDocumentCreateDto dto)
        {
            var result = await n3HealthService.AddMedRecord(dto);

            return result.Exception == null
                ? Ok(new { result.Code, result.Data })
                : BadRequest(new { result.Code, result.ErrorMessage });
        }

        [HttpPost("modifyPatient")]
        public async Task<IActionResult> ModifyPatientAndGetId(AddPatientRequestDto addPatientRequestDto)
        {
            var result = await n3HealthService.AddPatientAndGetId(addPatientRequestDto);
            
            return result.Exception == null
                ? Ok(new { result.Code, result.Data })
                : BadRequest(new { result.Code, result.ErrorMessage });
        }
    }
}
