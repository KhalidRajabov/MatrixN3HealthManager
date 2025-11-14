using MatrixN3HealthManager.Models;

namespace MatrixN3HealthManager.DTOs
{
    public class AddPatientRequestDto
    {
        public string ProjectGuid { get; set; }
        public string IdLpu { get; set; }
        /// <summary>
        /// Global ID – will be used to convert into Patient MIS ID
        /// </summary>
        public string PatientGlobalId { get; set; }
        public PatientCreateDto Patient { get; set; } = new();
    }

    public class PatientCreateDto
    {
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }

        public DateTime BirthDate { get; set; }

        public N3Enums.N3Sex Sex { get; set; }   
        public BirthPlaceCreateDto BirthPlace { get; set; }

        public List<AddressCreateDto> Addresses { get; set; } = new();

        public List<ContactCreateDto> Contacts { get; set; } = new();

        public List<DocumentCreateDto> Documents { get; set; } = new();
    }
    public class BirthPlaceCreateDto
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
    }
    public class AddressCreateDto
    {
        public string Street { get; set; }
        public string Building { get; set; }
        public string Appartment { get; set; }
        public string City { get; set; }

        public int? PostalCode { get; set; }

        public string StringAddress { get; set; }

        public N3Enums.N3IdAddressType AddressType { get; set; } 
    }
    public class ContactCreateDto
    {
        public string ContactValue { get; set; }

        public N3Enums.N3ContactType ContactType { get; set; } 
    }
    public class DocumentCreateDto
    {
        public string DocN { get; set; }
        public string DocS { get; set; }

        public N3Enums.N3IdDocumentType DocumentType { get; set; }

        public string ProviderName { get; set; }
        public string RegionCode { get; set; }
    }
}
