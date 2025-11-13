using EMKService;
using MatrixN3HealthManager.DTOs;
using MatrixN3HealthManager.Models;
using N3HealthManager.DTOs;
using PixService;
using System.Net;

namespace MatrixN3HealthManager.Main
{
    public class N3HealthManager : IN3HealthService
    {
        private readonly PixServiceClient pixClient;
        private readonly EmkServiceClient emkClient;
        private readonly string projectGuid = "7e201716-0943-48c9-88d0-6ddd7b1553aa";
        private readonly string idLPU = "95f617cf-4747-4285-9681-4a875357df26";
        public N3HealthManager()
        {
            pixClient = new PixServiceClient(PixServiceClient.EndpointConfiguration.BasicHttpBinding_IPixService);
            emkClient = new EmkServiceClient(EmkServiceClient.EndpointConfiguration.BasicHttpBinding_IEmkService);
        }

        public async Task<BaseResponse> AddPatientAndGetId(AddPatientRequestDto dto)
        {
            try
            {
                var patientDto = dto.Patient;

                var patient = new PatientDto
                {
                    FamilyName = patientDto.FamilyName,
                    GivenName = patientDto.GivenName,
                    MiddleName = patientDto.MiddleName,
                    BirthDate = patientDto.BirthDate,
                    Sex = (byte)patientDto.Sex,
                    IdPatientMIS = patientDto.IdPatientMIS,
                    IdGlobal = dto.PatientGlobalId,
                    BirthPlace = new BirthPlaceDto
                    {
                        City = patientDto.BirthPlace.City,
                        Region = patientDto.BirthPlace.Region,
                        Country = patientDto.BirthPlace.Country
                    },
                    Addresses = patientDto.Addresses.Select(a => new AddressDto
                    {
                        Street = a.Street,
                        Building = a.Building,
                        Appartment = a.Appartment,
                        City = a.City,
                        PostalCode = a.PostalCode,
                        StringAddress = a.StringAddress,
                        IdAddressType = (byte)a.AddressType
                    }).ToArray(),
                    Contacts = patientDto.Contacts.Select(c => new ContactDto
                    {
                        ContactValue = c.ContactValue,
                        IdContactType = (byte)c.ContactType
                    }).ToArray(),
                    Documents = patientDto.Documents.Select(d => new DocumentDto
                    {
                        DocN = d.DocN,
                        DocS = d.DocS,
                        ProviderName = d.ProviderName,
                        RegionCode = d.RegionCode,
                        IdDocumentType = (short)d.DocumentType
                    }).ToArray()
                };

                var existingPatient = await pixClient.GetPatientByGlobalIdAsync(dto.ProjectGuid, dto.PatientGlobalId, dto.IdLpu);
                
                return new BaseResponse(
                    HttpStatusCode.OK,
                    existingPatient != null
                        ? await pixClient.AddPatientAndGetIdAsync(dto.ProjectGuid, dto.IdLpu, patient)
                        : await pixClient.UpdatePatientAndGetIdAsync(dto.ProjectGuid, dto.IdLpu, patient)
                );

            }
            catch (Exception ex)
            {
                return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, ex.Message);
            }
        }
        public async Task<BaseResponse> GetPatient()
        {
            PatientDto patientDto = new()
            {
                IdPatientMIS = "patient3" //bizdeki idsi
            };

            var result = await pixClient.GetPatientAsync(projectGuid, idLPU, patientDto, SourceType.Fed);

            return new BaseResponse(HttpStatusCode.OK, result);
        }
        public async Task<BaseResponse> GetPatientByGlobalId(string patientId)
        {
            try
            {
                var patient = await pixClient.GetPatientByGlobalIdAsync(projectGuid, "98dfe41e-a22b-4838-b61a-f076e0809f23/", idLPU);
                return new BaseResponse(HttpStatusCode.OK, patient);
            }
            catch (Exception ex)
            {
                if (ex.Message == "The creator of this fault did not specify a Reason.")
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, ex.Message);

                throw new Exception("An error occurred while fetching the patient by global ID.", ex);
            }
        }
        public async Task<BaseResponse> UpdatePatientAndGetId()
        {
            PatientDto patientDto = new()
            {
                FamilyName = "Te",
                GivenName = "Sakoc",
                MiddleName = "Sakoci",
                BirthDate = new DateTime(1987, 1, 10, 10, 38, 1),
                Sex = (byte)N3Enums.N3Sex.Male,
                IdPatientMIS = "patient5ssq",
                Contacts = [
                        new(){
                            ContactValue = "+76969696969",
                            IdContactType = 2
                        }
                    ],
                IdGlobal = "5bb3d57f-6203-43eb-b289-2cfb0a51409e"
            };
            var result = await pixClient.UpdatePatientAndGetIdAsync(projectGuid, idLPU, patientDto);

            return new BaseResponse(HttpStatusCode.OK, result);
        }
        public async Task<BaseResponse> AddMedRecord(MedDocumentCreateDto dto)
        {
            try
            {

                var patient = await pixClient.GetPatientByGlobalIdAsync(dto.ProjectGuid, dto.PatientGlobalId, dto.IDLPU);
                if (patient == null)
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, "Patient not found");

                var laboratoryReport = new MedDocument
                {
                    Attachments = [
                    new MedDocumentDtoDocumentAttachment
                    {
                        Data = Convert.FromBase64String(dto.DataBase64),
                        OrganizationSign = Convert.FromBase64String(dto.OrganizationSignBase64),
                        MimeType = "application/pdf",
                        PersonalSigns = [
                            new MedDocumentDtoPersonalSign
                            {
                                Sign = Convert.FromBase64String(dto.SignBase64),
                                Doctor = new ()
                                {
                                    Person = new ()
                                    {
                                        HumanName = new()
                                        {
                                            GivenName = dto.DoctorPerson.GivenName,
                                            MiddleName = dto.DoctorPerson.MiddleName,
                                            FamilyName = dto.DoctorPerson.FamilyName
                                        },
                                        Sex = (byte)dto.DoctorPerson.Sex,
                                        Birthdate = dto.DoctorPerson.Birthdate,
                                        IdPersonMis = dto.DoctorPerson.IdPersonMis,
                                        Documents = dto.DoctorPerson.Documents.Select(d => new IdentityDocument()
                                        {
                                            DocN = d.DocN,
                                            DocS = d.DocS,
                                            DocumentName = d.DocumentName,
                                            ExpiredDate = d.ExpiredDate,
                                            IdDocumentType = (byte)d.IdDocumentType,
                                            IdProvider = d.IdProvider,
                                            IssuedDate = d.IssuedDate,
                                            ProviderName = d.ProviderName,
                                            RegionCode = d.RegionCode,
                                            StartDate = d.StartDate
                                        }).ToArray()
                                    },
                                    IdLpu = null,
                                    IdSpeciality = (ushort)N3Enums.N3IdSpeciality.Geriatrics,
                                    IdPosition = (ushort)N3Enums.N3IdPosition.HeadOfKennel
                                }
                            }
                        ]
                    }
                    ],
                                Author = new()
                                {
                                    Person = new()
                                    {
                                        HumanName = new()
                                        {
                                            GivenName = dto.AuthorPerson.GivenName,
                                            MiddleName = dto.AuthorPerson.MiddleName,
                                            FamilyName = dto.AuthorPerson.FamilyName
                                        },
                                        Sex = (byte)dto.AuthorPerson.Sex,
                                        Birthdate = dto.AuthorPerson.Birthdate,
                                        IdPersonMis = dto.AuthorPerson.IdPersonMis,
                                        Documents = dto.AuthorPerson.Documents.Select(d => new IdentityDocument()
                                        {
                                            DocN = d.DocN,
                                            DocS = d.DocS,
                                            DocumentName = d.DocumentName,
                                            ExpiredDate = d.ExpiredDate,
                                            IdDocumentType = (byte)d.IdDocumentType,
                                            IdProvider = d.IdProvider,
                                            IssuedDate = d.IssuedDate,
                                            ProviderName = d.ProviderName,
                                            RegionCode = d.RegionCode,
                                            StartDate = d.StartDate
                                        }).ToArray()
                                    },
                                    IdLpu = null,
                                    IdPosition = (ushort)N3Enums.N3IdPosition.HeadOfKennel
                                },
                                CreationDate = DateTime.Now,
                                Header = dto.Header,
                                IdDocumentMis = dto.IdDocumentMis
                     };

                await emkClient.AddMedRecordAsync(dto.ProjectGuid, dto.IDLPU, patient.IdPatientMIS, null, laboratoryReport, null);

                return new BaseResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, ex.Message);
            }

        }
        public async Task<BaseResponse> UpdateMedRecord()
        {
            byte[] data = Convert.FromBase64String("");
            byte[] organizationSIgn = Convert.FromBase64String("");
            byte[] sign = Convert.FromBase64String("");
            MedDocument medDocument = new()
            {
                Attachments = [
                            new MedDocumentDtoDocumentAttachment()
                            {
                            Data = data,
                            OrganizationSign = organizationSIgn,
                            PersonalSigns = [
                                    new(){
                                        Sign = sign,
                                        Doctor = new(){
                                            Person = new(){
                                                HumanName = new(){
                                                    GivenName = "sako",
                                                    MiddleName = "null",
                                                    FamilyName = "sako"
                                                },
                                                IdPersonMis = "415",
                                                Documents = [
                                                    new IdentityDocument(){
                                                        DocN = "52293671595",
                                                        DocS = "321312",
                                                        DocumentName = "СНИЛС",
                                                        ExpiredDate = new DateTime(2011, 12, 1),
                                                        IdDocumentType = (byte)N3Enums.N3IdDocumentType.PensionInsuranceCertificate,
                                                        IdProvider = null,
                                                        IssuedDate = new DateTime(2008, 9, 3),
                                                        ProviderName = "ПФР",
                                                        RegionCode = "128",
                                                        StartDate = new DateTime(2008, 9, 4)
                                                    },
                                                    new IdentityDocument()
                                                    {
                                                        DocN = "8769227022",
                                                        DocS = "365431",
                                                        DocumentName = "ДМС",
                                                        ExpiredDate = new DateTime(2010, 12, 1),
                                                        IdDocumentType = (byte)N3Enums.N3IdDocumentType.RussianCitizenPassport,
                                                        IdProvider = 59021,
                                                        IssuedDate = new DateTime(2006, 9, 3),
                                                        ProviderName = "ДМС",
                                                        RegionCode = "128",
                                                        StartDate = new DateTime(2006, 9, 4)
                                                    },
                                                    ]
                                            },
                                            IdSpeciality =28,
                                            IdPosition = 114
                                        },
                                    }
                                ],
                            MimeType = "application/pdf"
                            }
                    ],
                Author = new()
                {
                    Person = new()
                    {
                        HumanName = new()
                        {
                            GivenName = "saksocu",
                            MiddleName = "sako",
                            FamilyName = "sak"
                        },
                        IdPersonMis = "415",
                        Documents = [
                                new(){
                                    DocN ="25857824124",
                                    DocS = "111112",
                                    DocumentName = "СНИЛС",
                                    ExpiredDate = new(2011,12,1),
                                    IdDocumentType =(byte)N3Enums.N3IdDocumentType.PensionInsuranceCertificate,
                                    IdProvider = null,
                                    IssuedDate = new(2008,9,3),
                                    ProviderName = "ПФР",
                                    RegionCode = "128",
                                    StartDate = new(2008,9,4)
                                }
                            ]
                    },
                    IdSpeciality = 28,
                    IdPosition = 114
                },
                CreationDate = DateTime.Now,
                Header = "Лабораторные исследования",
                IdDocumentMis = "iddocmed332axxz",
                IdMedDocumentType = 2
            };

            await emkClient.UpdateMedRecordAsync(projectGuid, idLPU, "patient5ssq", null, medDocument, null);
            return new BaseResponse(HttpStatusCode.OK); ;
        }

    }

}
