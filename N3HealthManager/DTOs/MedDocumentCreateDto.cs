using MatrixN3HealthManager.Models;

namespace MatrixN3HealthManager.DTOs
{
    public class MedDocumentCreateDto
    {
        public string? DataBase64 { get; set; } 
        public string? OrganizationSignBase64 { get; set; }
        public string? SignBase64 { get; set; }
        public string? MimeType { get; set; }

        public string? ProjectGuid { get; set; }
        public string? Idlpu { get; set; }

        /// <summary>
        /// Global ID – will be used to convert into Patient MIS ID
        /// </summary>
        public string PatientGlobalId { get; set; }

        public string Header { get; set; } = "Лабораторные исследования";
        public string IdDocumentMis { get; set; }

        public PersonDto AuthorPerson { get; set; } = new();
        public PersonDto DoctorPerson { get; set; } = new();
    }


    public class PersonDto
    {
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }

        public N3Enums.N3Sex Sex { get; set; }

        public DateTime Birthdate { get; set; }
        public string IdPersonMis { get; set; }

        public List<IdentityDocumentDto> Documents { get; set; } = new();
    }


    public class IdentityDocumentDto
    {
        public string DocN { get; set; }
        public string DocS { get; set; }
        public string DocumentName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string ProviderName { get; set; }
        public string RegionCode { get; set; }

        public N3Enums.N3IdDocumentType IdDocumentType { get; set; }

        public int? IdProvider { get; set; }
    }


}
