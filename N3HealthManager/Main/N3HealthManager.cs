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

        public N3HealthManager()
        {
            pixClient = new PixServiceClient(PixServiceClient.EndpointConfiguration.BasicHttpBinding_IPixService);
            emkClient = new EmkServiceClient(EmkServiceClient.EndpointConfiguration.BasicHttpBinding_IEmkService);
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

                PatientDto findingPatient = new()
                {
                    IdPatientMIS = dto.PatientGlobalId
                };

                var patients = await pixClient.GetPatientAsync(dto.ProjectGuid, dto.Idlpu, findingPatient, SourceType.Fed);
                if (patients == null)
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, "Patient not found");

                var existingPatient = patients.FirstOrDefault(x => x.IdPatientMIS == dto.PatientGlobalId);
                if (existingPatient == null)
                {
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
                }
                else
                {
                    var patientDto = dto.Patient.Patient;

                    // Prefer a stable MIS id from existing patient record.
                    // If your "patients" list stores IdPatientMIS differently, adjust accordingly.
                    var updatePatient = new PatientDto
                    {
                        FamilyName = patientDto.FamilyName,
                        GivenName = patientDto.GivenName,
                        MiddleName = patientDto.MiddleName,
                        BirthDate = patientDto.BirthDate,
                        Sex = (byte)patientDto.Sex,

                        // Use the known MIS identifier for update.
                        // You can also fallback to findingPatient.IdPatientMIS if your flow requires it.
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

                        // This is likely the critical part for your SNILS issue:
                        // Always push the current documents set from dto.
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

                }

                var xmlBytes = Convert.FromBase64String(dto.DataBase64);

                if (xmlBytes.Length < 500)
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error,
                        $"Invalid CDA DataBase64: too small ({xmlBytes.Length} bytes).");

                if (!TryReadCdaMetadata(xmlBytes, out var documentId, out var creationDate, out var metadataError))
                    return new BaseResponse(HttpStatusCode.BadRequest, ExceptionStatus.error, metadataError);

                //string bothSigns = "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";
                //var orgSignBytes = Convert.FromBase64String(dto.OrganizationSignBase64);
                var signBytes = Convert.FromBase64String(dto.SignBase64);
               
                var laboratoryReport = new MedDocument
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
                                    Person = new ()
                                    {
                                        HumanName = new()
                                        {
                                            GivenName = dto.DoctorPerson.GivenName,
                                            MiddleName = dto.DoctorPerson.MiddleName,
                                            FamilyName = dto.DoctorPerson.FamilyName
                                        },
                                        Sex = (byte)dto.DoctorPerson.Sex,
                                        Birthdate = NormalizePersonBirthdate(dto.DoctorPerson.Birthdate),
                                        IdPersonMis = dto.DoctorPerson.IdPersonMis,
                                        Documents = dto.DoctorPerson.Documents.Select(MapIdentityDocument).ToArray()
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
                            Birthdate = NormalizePersonBirthdate(dto.AuthorPerson.Birthdate),
                            IdPersonMis = dto.AuthorPerson.IdPersonMis,
                            Documents = dto.AuthorPerson.Documents.Select(MapIdentityDocument).ToArray()
                        },
                        IdLpu = null,
                        IdPosition = (ushort)N3Enums.N3IdPosition.HeadOfKennel
                    },
                    CreationDate = creationDate.UtcDateTime,
                    Header = "Лабораторные исследования",
                    IdDocumentMis = documentId,
                };

                await emkClient.AddMedRecordAsync(dto.ProjectGuid, dto.Idlpu, dto.PatientGlobalId, null, laboratoryReport, null);

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

        private static DateTime? NormalizePersonBirthdate(DateTime? birthdate)
        {
            if (!birthdate.HasValue)
                return null;

            var value = birthdate.Value;
            if (value == default || value.Year < 1900)
                return null;

            return value;
        }

        private static DateTime? NormalizeOptionalDocumentDate(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            if (value.Value == default || value.Value.Year < 1900)
                return null;

            return value;
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
