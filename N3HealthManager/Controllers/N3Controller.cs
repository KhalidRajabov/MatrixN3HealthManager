using MatrixN3HealthManager.Main;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MatrixN3HealthManager.DTOs;

namespace MatrixN3HealthManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class N3Controller(IN3HealthService n3HealthService) : ControllerBase
    {
        [HttpPost("addMedRecord")]
        public async Task<IActionResult> AddMedRecord(MedDocumentCreateDto dto)
        {
            await n3HealthService.AddMedRecord(dto);
            return Ok();
        }
    }
}
