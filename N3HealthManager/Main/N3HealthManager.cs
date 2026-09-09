using EMKService;
using MatrixN3HealthManager.DTOs;
using MatrixN3HealthManager.Models;
using Newtonsoft.Json;
using PixService;
using System.Net;
using System.Globalization;
using System.Xml.Linq;

namespace MatrixN3HealthManager.Main
{
    public class N3HealthManager : IN3HealthService
    {
        private readonly PixServiceClient pixClient;
        private readonly EmkServiceClient emkClient;
        private readonly string projectGuid = "7e201716-0943-48c9-88d0-6ddd7b1553aa";
        private readonly string idLPU = "95f617cf-4747-4285-9681-4a875357df26";
        private readonly ILogger<N3HealthManager> _logger;

        public N3HealthManager(ILogger<N3HealthManager> logger)
        {
            pixClient = new PixServiceClient(PixServiceClient.EndpointConfiguration.BasicHttpBinding_IPixService);
            emkClient = new EmkServiceClient(EmkServiceClient.EndpointConfiguration.BasicHttpBinding_IEmkService);
            _logger = logger;
        }

        public async Task<BaseResponse> AddPatientAndGetId(AddPatientRequestDto dto)
        {
            try
            {
                var existingPatient = await pixClient.GetPatientByGlobalIdAsync(dto.ProjectGuid, dto.PatientGlobalId, dto.IdLpu);

                var patientDto = dto.Patient;

                var patient = new PatientDto
                {
                    FamilyName = patientDto.FamilyName,
                    GivenName = patientDto.GivenName,
                    MiddleName = patientDto.MiddleName,
                    BirthDate = patientDto.BirthDate,
                    Sex = (byte)patientDto.Sex,
                    IdPatientMIS = existingPatient != null ? existingPatient.IdPatientMIS : null,
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
                _logger.LogInformation("N3: AddMedRecord called with PatientGlobalId: {PatientGlobalId}, IdDocumentMis: {IdDocumentMis}", dto.PatientGlobalId, dto.IdDocumentMis);


                PatientDto findingPatient = new()
                {
                    IdPatientMIS = dto.PatientGlobalId
                };

                _logger.LogInformation("N3: Fetching patient with IdPatientMIS: {IdPatientMIS}", findingPatient.IdPatientMIS);
                var patients = await pixClient.GetPatientAsync(dto.ProjectGuid, dto.Idlpu, findingPatient, SourceType.Fed);
                if (patients == null)
                {
                    
                    _logger.LogWarning("N3: Patient not found with IdPatientMIS: {IdPatientMIS}", findingPatient.IdPatientMIS);
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, "Patient not found");
                }

                var existingPatient = patients.FirstOrDefault(x => x.IdPatientMIS == dto.PatientGlobalId);
                if (existingPatient == null)
                {
                    _logger.LogInformation("N3: Patient not found with IdPatientMIS: {IdPatientMIS}, creating new patient.", findingPatient.IdPatientMIS);

                    var patientDto = dto.Patient.Patient;
                    var patient = new PatientDto
                    {
                        FamilyName = patientDto.FamilyName,
                        GivenName = patientDto.GivenName,
                        MiddleName = patientDto.MiddleName,
                        BirthDate = patientDto.BirthDate,
                        Sex = (byte)patientDto.Sex,
                        IdPatientMIS = findingPatient.IdPatientMIS,
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
                            //DocS = d.DocS,
                            ProviderName = d.ProviderName,
                            //RegionCode = d.RegionCode,
                            IdDocumentType = (short)d.DocumentType,
                            IssuedDate = d.IssuedDate,
                        }).ToArray()
                    };




                    var patientCreateDto = await pixClient.AddPatientAndGetIdAsync(dto.ProjectGuid, dto.Idlpu, patient);
                    _logger.LogInformation("N3: New patient created with PatientGlobalId: {IdGlobal}", patient.IdGlobal);
                }
                else
                {
                    _logger.LogInformation("N3: Patient found with IdPatientMIS: {IdPatientMIS}, updating patient.", findingPatient.IdPatientMIS);
                    var patientDto = dto.Patient.Patient;

                    var updatePatient = new PatientDto
                    {
                        FamilyName = patientDto.FamilyName,
                        GivenName = patientDto.GivenName,
                        MiddleName = patientDto.MiddleName,
                        BirthDate = patientDto.BirthDate,
                        Sex = (byte)patientDto.Sex,

                        IdPatientMIS = existingPatient.IdPatientMIS,

                        IdGlobal = dto.PatientGlobalId,

                        BirthPlace = patientDto.BirthPlace == null ? null : new BirthPlaceDto
                        {
                            City = patientDto.BirthPlace.City,
                            Region = patientDto.BirthPlace.Region,
                            Country = patientDto.BirthPlace.Country
                        },

                        Addresses = patientDto.Addresses?
                            .Select(a => new AddressDto
                            {
                                Street = a.Street,
                                Building = a.Building,
                                Appartment = a.Appartment,
                                City = a.City,
                                PostalCode = a.PostalCode,
                                StringAddress = a.StringAddress,
                                IdAddressType = (byte)a.AddressType
                            })
                            .ToArray()
                            ?? Array.Empty<AddressDto>(),

                        Contacts = patientDto.Contacts?
                            .Select(c => new ContactDto
                            {
                                ContactValue = c.ContactValue,
                                IdContactType = (byte)c.ContactType
                            })
                            .ToArray()
                            ?? Array.Empty<ContactDto>(),
                         
                        Documents = patientDto.Documents?
                            .Select(d => new DocumentDto
                            {
                                DocN = d.DocN,
                                //DocS = d.DocS,
                                ProviderName = d.ProviderName,
                                //RegionCode = d.RegionCode,
                                IdDocumentType = (short)d.DocumentType,
                                IssuedDate = d.IssuedDate,
                            })
                            .ToArray()
                            ?? Array.Empty<DocumentDto>()
                    };

                    string json = JsonConvert.SerializeObject(updatePatient);

                    var updatedId =
                        await pixClient.UpdatePatientAndGetIdAsync(dto.ProjectGuid, dto.Idlpu, updatePatient);
                    _logger.LogInformation("N3: Patient updated with PatientGlobalId: {IdGlobal}", updatePatient.IdGlobal);
                }

                var xmlBytes = Convert.FromBase64String(dto.DataBase64);

                if (xmlBytes.Length < 500)
                {
                    _logger.LogError("N3: Invalid CDA DataBase64: too small ({Length} bytes) for PatientGlobalId: {PatientGlobalId}, IdDocumentMis: {IdDocumentMis}", xmlBytes.Length, dto.PatientGlobalId, dto.IdDocumentMis);
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error,
                        $"Invalid CDA DataBase64: too small ({xmlBytes.Length} bytes).");
                }

                if (!TryReadCdaMetadata(xmlBytes, out var documentId, out var creationDate, out var metadataError))
                {
                    _logger.LogError("N3: Invalid CDA DataBase64: {MetadataError} for PatientGlobalId: {PatientGlobalId}, IdDocumentMis: {IdDocumentMis}", metadataError, dto.PatientGlobalId, dto.IdDocumentMis);
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, metadataError);
                }

                //var orgSignBytes = Convert.FromBase64String(dto.OrganizationSignBase64);
                var signBytes = Convert.FromBase64String(dto.SignBase64);

                var doctorPosition = dto.DoctorPerson?.IdPosition
                    ?? (ushort)N3Enums.N3IdPosition.DistrictTherapist;
                var doctorSpeciality = dto.DoctorPerson?.IdSpeciality
                    ?? (ushort)N3Enums.N3IdSpeciality.Therapy;
                var authorPosition = dto.AuthorPerson?.IdPosition ?? doctorPosition;

                var consultationDocument = new MedDocument
                {
                    IdMedDocumentType = 316,
                    Attachments = [
                    new MedDocumentDtoDocumentAttachment
                    {
                        Data = xmlBytes,
                        OrganizationSign = signBytes,
                        MimeType = "text/xml",
                        PersonalSigns = [
                            new MedDocumentDtoPersonalSign
                            {
                                Sign = signBytes,
                                Doctor = new ()
                                {
                                    Person = MapPersonWithIdentity(dto.DoctorPerson),
                                    IdLpu = null,
                                    IdSpeciality = doctorSpeciality,
                                    IdPosition = doctorPosition
                                }
                            }
                        ]
                    }
                    ],
                    Author = new()
                    {
                        Person = MapPersonWithIdentity(dto.AuthorPerson),
                        IdLpu = null,
                        IdSpeciality = dto.AuthorPerson?.IdSpeciality ?? doctorSpeciality,
                        IdPosition = authorPosition
                    },
                    CreationDate = creationDate.UtcDateTime,
                    Header = string.IsNullOrWhiteSpace(dto.Header)
                        ? "Протокол консультации"
                        : dto.Header,
                    IdDocumentMis = documentId,
                };

                await emkClient.AddMedRecordAsync(dto.ProjectGuid, dto.Idlpu, dto.PatientGlobalId, null, consultationDocument, null);

                _logger.LogInformation("N3: Medical record added successfully for PatientGlobalId: {PatientGlobalId}, IdDocumentMis: {IdDocumentMis}", dto.PatientGlobalId, documentId);

                return new BaseResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "N3: An error occurred while adding a medical record for PatientGlobalId: {PatientGlobalId}, IdDocumentMis: {IdDocumentMis}", dto.PatientGlobalId, dto.IdDocumentMis);
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

        private static DateTime? NormalizePersonBirthdate(DateTime? birthdate)
        {
            if (!birthdate.HasValue)
                return null;

            var value = birthdate.Value;
            if (value == default || value == DateTime.MinValue || value.Year < 1900)
                return null;

            return value.Date;
        }

        private static DateTime? NormalizeOptionalDocumentDate(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            if (value.Value == default || value.Value == DateTime.MinValue || value.Value.Year < 1900)
                return null;

            return value;
        }

        private static PersonWithIdentity MapPersonWithIdentity(PersonDto person)
        {
            var result = new PersonWithIdentity
            {
                HumanName = new HumanName
                {
                    GivenName = person.GivenName,
                    MiddleName = person.MiddleName,
                    FamilyName = person.FamilyName
                },
                Sex = (byte)person.Sex,
                IdPersonMis = person.IdPersonMis,
                Documents = person.Documents?.Select(MapIdentityDocument).ToArray() ?? Array.Empty<IdentityDocument>()
            };

            var birthdate = NormalizePersonBirthdate(person.Birthdate);
            if (birthdate.HasValue)
                result.Birthdate = birthdate;

            return result;
        }

        private static IdentityDocument MapIdentityDocument(IdentityDocumentDto d)
        {
            var isSnils = d.IdDocumentType == N3Enums.N3IdDocumentType.PensionInsuranceCertificate;

            if (isSnils)
            {
                return new IdentityDocument
                {
                    DocN = d.DocN,
                    IdDocumentType = (byte)d.IdDocumentType,
                    ProviderName = string.IsNullOrWhiteSpace(d.ProviderName) ? "ПФР" : d.ProviderName.Trim()
                };
            }

            return new IdentityDocument
            {
                DocN = d.DocN,
                DocS = d.DocS,
                DocumentName = d.DocumentName,
                ExpiredDate = NormalizeOptionalDocumentDate(d.ExpiredDate),
                IdDocumentType = (byte)d.IdDocumentType,
                IdProvider = d.IdProvider.HasValue ? (int)d.IdProvider.Value : null,
                IssuedDate = NormalizeOptionalDocumentDate(d.IssuedDate),
                ProviderName = d.ProviderName,
                RegionCode = d.RegionCode,
                StartDate = NormalizeOptionalDocumentDate(d.StartDate)
            };
        }

        private static bool TryReadCdaMetadata(
            byte[] xmlBytes,
            out string documentId,
            out DateTimeOffset creationDate,
            out string error)
        {
            documentId = string.Empty;
            creationDate = default;
            error = string.Empty;

            try
            {
                using var stream = new MemoryStream(xmlBytes);
                var document = XDocument.Load(stream, LoadOptions.None);
                XNamespace cda = "urn:hl7-org:v3";

                if (document.Root?.Name != cda + "ClinicalDocument")
                {
                    error = "CDA-документ должен содержать корневой элемент ClinicalDocument";
                    return false;
                }

                documentId = document.Root
                    .Element(cda + "id")?
                    .Attribute("extension")?
                    .Value?
                    .Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(documentId))
                {
                    error = "В CDA не заполнен extension корневого id";
                    return false;
                }

                var effectiveTime = document.Root
                    .Element(cda + "effectiveTime")?
                    .Attribute("value")?
                    .Value?
                    .Trim();

                if (string.IsNullOrWhiteSpace(effectiveTime)
                    || effectiveTime.Length != 17
                    || effectiveTime[12] is not ('+' or '-'))
                {
                    error = "В CDA не заполнен корректный корневой effectiveTime";
                    return false;
                }

                var normalizedEffectiveTime = effectiveTime.Insert(15, ":");
                if (!DateTimeOffset.TryParseExact(
                        normalizedEffectiveTime,
                        "yyyyMMddHHmmzzz",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out creationDate))
                {
                    error = "В CDA не заполнен корректный корневой effectiveTime";
                    return false;
                }

                if (creationDate > DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    error = "В CDA указано будущее время создания документа";
                    return false;
                }

                return true;
            }
            catch (Exception ex) when (ex is FormatException or System.Xml.XmlException)
            {
                error = "CDA-документ содержит некорректный XML";
                return false;
            }
        }


        private static void ValidateBase64OrThrow(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{fieldName} is empty");

            var s = value.Trim();

            // quick character scan
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                bool ok =
                    (ch >= 'A' && ch <= 'Z') ||
                    (ch >= 'a' && ch <= 'z') ||
                    (ch >= '0' && ch <= '9') ||
                    ch == '+' || ch == '/' || ch == '=' ||
                    ch == '\r' || ch == '\n' || ch == ' ' || ch == '\t' ||
                    ch == '-' || ch == '_'; // allow base64url for now

                if (!ok)
                    throw new Exception($"{fieldName} has illegal char '{ch}' at index {i}");
            }

            // now try tolerant decode
            _ = DecodeBase64Tolerant(s);
        }

        private static byte[] DecodeBase64Tolerant(string s)
        {
            s = s.Replace("\r", "")
                 .Replace("\n", "")
                 .Replace(" ", "")
                 .Replace("\t", "");

            // base64url -> base64
            s = s.Replace('-', '+').Replace('_', '/');

            // padding fix
            int mod = s.Length % 4;
            if (mod != 0)
                s += new string('=', 4 - mod);

            return Convert.FromBase64String(s);
        }
        static byte[] DecodeBase64Strict(string? s, string field)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new Exception($"{field} is empty");

            try
            {
                var bytes = Convert.FromBase64String(s);
                if (bytes.Length == 0)
                    throw new Exception($"{field} decoded to empty bytes");
                return bytes;
            }
            catch (FormatException)
            {
                throw new Exception($"{field} is not valid Base64");
            }
        }

    }

}
