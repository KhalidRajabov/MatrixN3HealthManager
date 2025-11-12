using N3HealthManager.DTOs;
using N3HealthManager.Models;
using PixService;

namespace MatrixN3HealthManager.Main
{
    public interface IN3HealthService
    {
        /// <summary>
        /// pasient elave etmek ve idsini global goturmek
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse> AddPatientAndGetId();
        /// <summary>
        /// pasient update etmek ve idsini global goturmek
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse> UpdatePatientAndGetId();

        /// <summary>
        /// pasienti bizdeki id ile chekmek
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse> GetPatient();
        /// <summary>
        /// pasienti glbobal id ile chekmek
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        Task<BaseResponse> GetPatientByGlobalId(string patientId);
        /// <summary>
        /// pasientin medrecordunu elave etmek
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse> AddMedRecord(MedDocumentCreateDto dto);
        /// <summary>
        /// pasientin medrecordunu update etmek
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse> UpdateMedRecord();
    }
}
