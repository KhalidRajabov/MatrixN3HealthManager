namespace N3HealthManager.DTOs
{
    public class MedDocumentCreateDto
    {
        public string DataBase64 { get; set; } = string.Empty;
        public string OrganizationSignBase64 { get; set; } = string.Empty;
        public string SignBase64 { get; set; } = string.Empty;

        public string Header { get; set; } = "Лабораторные исследования";
        public string IdDocumentMis { get; set; } = "iddocmed332axxz";

        public PersonDto AuthorPerson { get; set; } = new();
        public PersonDto DoctorPerson { get; set; } = new();
    }

    public class PersonDto
    {
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public byte Sex { get; set; }
        public DateTime Birthdate { get; set; }
        public string IdPersonMis { get; set; } = Guid.NewGuid().ToString();
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
        public byte IdDocumentType { get; set; }
        public int? IdProvider { get; set; }
    }


}
