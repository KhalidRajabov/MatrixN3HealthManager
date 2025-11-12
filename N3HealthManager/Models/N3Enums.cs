namespace MatrixN3HealthManager.Models
{
    public static class N3Enums
    {
        public enum N3ContactType
        {
            Mobile = 1,
            StationaryPhone,
            Email,
            Fax
        }
        public enum N3IdAddressType
        {
            RegistrationAddress = 1,               // адрес регистрации
            BirthPlaceAddress = 3,                 // адрес места рождения
            MailingAddress = 9,                    // Адрес для писем
            WorkplacePostalAddress = 6,            // Общий почтовый или телекоммуникационный адрес рабочего места
            DirectWorkplaceAddress = 5,            // Прямой почтовый или телекоммуникационный адрес рабочего места
            WorkAddress = 4,                       // Служебный адрес
            TemporaryAddress = 8,                  // Временный адрес
            ActualResidenceAddress = 2,           // адрес фактического проживания
            InvalidAddress = 7                     // Неправильный адрес
        }

        public enum N3Sex
        {
            Male = 1,         // Мужской
            Female = 2,       // Женский
            Undefined = 3     // Неопределенный
        }

        public enum N3IdCaseAidType
        {
            PrimaryCare = 1,                          // Первичная медико-санитарная помощь
            EmergencyCare = 2,                        // Скорая, в том числе специализированная (санитарно-авиационная), медицинская помощь
            HighTechMedicalCare = 4,                  // Высокотехнологичная медицинская помощь
            SpecializedCare = 3,                      // Специализированная, в том числе высокотехнологичная, медицинская помощь
            RehabilitationCare = 5,                   // Реабилитационная медицинская помощь
            Other = 6                                 // Другое
        }
        public enum N3IdPaymentType
        {
            Oms = 1,             // ОМС
            Dms = 4,             // ДМС
            Budget = 2,          // бюджет
            PaidServices = 3,    // платные услуги
            OwnFunds = 5,        // Собственные средства
            Other = 6            // другое
        }
        public enum N3Confidentality
        {
            Unrestricted = 1,     // Не ограничен
            Limited = 2,          // Ограничен
            SpecialRestrictions = 3  // Особые ограничения
        }
        public enum N3IdCaseResult
        {
            Healthy = 5,                                       // Здоров
            Recovery = 1,                                      // Выздоровление
            Improvement = 2,                                   // Улучшение
            FatalOutcome = 6,                                  // Летальный исход
            NoChange = 3,                                      // Без изменения
            Deterioration = 4,                                 // Ухудшение
            ReferralToHospital = 7,                            // Дано направление на госпитализацию
            ReferralToEmergencyHospital = 8,                   // Дано направление на госпитализацию по экстренным показаниям
            ReferralToDayHospital = 9,                         // Дано направление в дневной стационар
            ReferralForExamination = 10,                       // Дано направление на обследование
            ReferralForConsultation = 11,                       // Дано направление на консультацию
            ReferralForSanatoriumTreatment = 12,                // Дано направление на санаторно-курортное лечение
            ReferralForMedicalRehabilitation = 13,             // Дано направление на медицинскую реабилитацию
            RefusedMedicalExams = 14                           // Отказ от прохождения медицинских обследований при диспансеризации или медицинском осмотре
        }
        public enum N3IdSpeciality
        {
            Histology = 216,                                      // Гистология
            Pediatrics = 22,                                      // Педиатрия
            PediatricCardiology = 81,                             // Детская кардиология
            Genetics = 12,                                        // Генетика
            FunctionalDiagnostics = 35,                            // Функциональная диагностика
            Geriatrics = 66,                                      // Гериатрия
            ForensicPsychiatricExamination = 100,                  // Судебно-психиатрическая экспертиза
            Osteopathy = 287,                                     // Остеопатия
            SocialHygieneAndPublicHealthOrganization = 153,        // Социальная гигиена и организация госсанэпидслужбы
            Dentistry = 208,                                      // Стоматология
            ForensicMedicalExamination = 26,                       // Судебно-медицинская экспертиза
            Nephrology = 89,                                      // Нефрология
            AudiologyENT = 75,                                    // Сурдология - оториноларингология
            MedicalSocialExamination = 236,                       // Медико-социальная экспертиза
            XRayEndovascularDiagnosticsAndTreatment = 248,         // Рентгенэндоваскулярные диагностика и лечение
            InfectiousDiseases = 32,                               // Инфекционные болезни
            HygieneEducation = 162,                               // Гигиеническое воспитание
            MedicalSupportPersonnel = 204,                         // Средний медицинский персонал
            Therapy = 27,                                         // Терапия
            DentistryAlt = 3                                      // Стоматология (alternate)
        }
        public enum N3IdPosition
        {
            AdministrativeAndSupportStaffPositions = 323,                     // Должности административно-хозяйственного персонала образовательных и научно-исследовательских организаций
            RectorOfHigherEducationInstitution = 376,                          // Ректор образовательной организации высшего профессионального образования
            Neurosurgeon = 46,                                                // Врач-нейрохирург
            SeniorMethodologist = 301,                                         // Старший методист
            ClinicalLaboratoryDiagnosisPhysician = 35,                         // Врач клинической лабораторной диагностики
            Parasitologist = 56,                                              // Врач-паразитолог
            PhysicalEducationInstructor = 284,                                 // Инструктор по физической культуре
            HeadOfPharmacyOrganizationDepartment = 374,                        // Начальник структурного подразделения аптечной организации
            HeadOfProstheticProductionDepartment = 447,                        // Заведующий производством отделений зубопротезирования
            HeadOfScientificAndTechnicalInformationDepartment = 247,           // Заведующий (начальник) отделом научно-технической информации
            DentistTherapist = 103,                                            // Врач-стоматолог-терапевт
            MedicalPhysicist = 138,                                           // Медицинский физик
            TransfusionPhysician = 115,                                        // Врач-трансфузиолог
            OccupationalTherapyInstructor = 157,                              // Инструктор по трудовой терапии
            HeadOfMedicalDepartmentInOtherOrganization = 340,                  // Заведующий структурным подразделением, осуществляющим медицинскую деятельность, иной организации
            HeadOfMedicalSupplyWarehouse = 220,                               // Заведующий медицинским складом мобилизационного резерва
            DistrictTherapist = 110,                                          // Врач-терапевт участковый
            HeadOfKennel = 458,                                               // Заведующий питомником
            HeadOfStudentDormitory = 421,                                      // Начальник студенческого общежития
            PhysicalEducationLeader = 296                                      // Руководитель физического воспитания
        }

        public enum N3IdRole
        {
            Resident = 9,                  // Ординатор
            Payer = 6,                     // Плательщик
            Intern = 10,                   // Интерн
            Doctor = 3,                    // Врач
            Department = 8,                // Отделение пребывания
            Institution = 2,               // Учреждение
            Patient = 5,                   // Пациент
            AttendingDoctor = 4,           // Лечащий врач
            Subdivision = 1,               // Подразделение
            Registrar = 7                  // Регистратор
        }
        public enum N3AdmissionCondition
        {
            Satisfactory = 1,         // Удовлетворительное
            Moderate = 2,             // Средней тяжести
            Severe = 3,               // Тяжелое
            ExtremelySevere = 4,      // Крайне тяжелое
            ClinicalDeath = 5,        // Клиническая смерть
            Terminal = 6              // Терминальное
        }
        public enum N3CaseVisitType
        {
            Primary = 1,        // Первичный
            Repeated = 2        // Повторный
        }
        public enum N3IdCasePurpose : byte
        {
            TherapeuticAndDiagnosticVisit = 1,      // Лечебно-диагностический прием
            ConsultativeVisit = 2,                  // Консультативный прием
            DispensaryObservation = 3,              // Диспансерное наблюдение
            PreventiveVisit = 4,                    // Профилактический прием
            ProfessionalExamination = 5,            // Профессиональный осмотр
            RehabilitationVisit = 6,                // Реабилитационный прием
            DentalProstheticVisit = 7,              // Зубопротезный прием
            HealthCenterVisit = 9,                  // Обращение в центр здоровья
            ProstheticAndOrthopedicVisit = 8,       // Протезно-ортопедический прием
            AdditionalDispensaryExamination = 10,   // Дополнительная диспансеризация
            HomeCare = 11,                          // Патронаж
            Other = 12                              // Другие
        }
        public enum N3IdCaseType : byte
        {
            MasterCase = 1,                // мастер-случай
            Hospitalization = 3,           // Госпитализация
            OutpatientCase = 2,            // Амбулаторный случай
            DispensaryObservation = 4,     // Диспансеризация
            DispensaryEvery2Years = 7,     // Диспансеризация 1 раз в 2 года
            DispensaryStage2 = 6,          // Диспансеризация 2-й этап
            EmergencyCare = 5,             // Скорая Помощь
            Telemedicine = 8               // Телемедицинская
        }
        public enum N3IdAmbResult
        {
            SentForVMP = 10,                      // направлен на ВМП
            SentForMSEC = 9,                      // направлен на МСЭК
            SentForDayHospital = 5,               // направлен в дневной стационар
            FatalOutcome = 16,                    // летальный исход
            SentForConsultation = 7,              // направлен на консультацию
            SentForConsultationOtherLPU = 8,      // направлен на консультацию в др. ЛПУ
            Improvement = 2,                      // улучшение
            ExpertConclusionDenied = 12,          // отказано в экспертном заключении
            OtherDecision = 17,                   // иное решение
            SentForHomeHospitalization = 6,       // направлен в стационар на дому
            SentForHospitalization = 4,           // направлен на госпитализацию
            SanatoriumVoucher = 14,               // санаторно-курортная карта
            Recovery = 1,                         // выздоровление
            ReferralForVoucher = 13,              // справка для получения путевки
            DynamicObservation = 3,               // динамическое наблюдение
            AdditionalDispensaryExamination = 15, // проведена доп. Диспансеризация
            ExpertIssued = 11                    // выдано экспертное
        }
        public enum N3IdVisitPlace
        {
            Outpatient = 1,            // амбулаторно
            DayHospital = 3,          // стационар дневной
            AtHome = 2,               // на дому
            HomeHospitalization = 4,  // стационар на дому
            ActiveCareAtHome = 5,     // актив на дому
            OnSite = 6                // на выезде
        }

        public enum N3IdVisitPurpose
        {
            TherapeuticDiagnostic = 1,    // лечебно-диагностическая
            Consultation = 2,             // консультационная
            DispensaryObservation = 3,    // диспансерное наблюдение
            PreventiveCare = 4,           // профилактическая
            MedicalExamination = 5,       // медосмотр
            HomeCare = 6,                 // патронаж
            Rehabilitation = 7,           // реабилитационная
            DentureProsthetics = 8,       // зубопротезная
            OrthopedicProsthetics = 9,    // протезно-ортопедическая
            AdditionalDispensaryExam = 10 // доп. Диспансеризация
        }
        public enum N3IdDocumentType
        {
            BirthCertificate = 3,                                 // Свидетельство о рождении
            TemporaryCertificate = 227,                           // Временное свидетельство
            OfficerID = 220,                                      // Удостоверение личности офицера
            RefugeeID = 12,                                       // Удостоверение беженца
            PensionInsuranceCertificate = 223,                    // Пенсионное страховое свидетельство (СНИЛС)
            RegistrationCertificateIndividualEntrepreneur = 245,  // Свидетельство о регистрации физического лица в качестве индивидуального предпринимателя
            TemporaryResidencePermit = 248,                       // Временное разрешение на проживание
            INNCertificate = 224,                                 // Свидетельство о присвоении ИНН
            ImmigrationPetitionRegistrationCertificate = 10,      // Свидетельство о регистрации ходатайства иммигранта о признании его беженцем
            DriverLicense = 225,                                  // Водительское удостоверение
            DisabilityCertificate = 238,                          // Справка об инвалидности
            DiplomaticPassport = 8,                               // Дипломатический паспорт гражданина РФ
            TemporaryAsylumCertificate = 247,                      // Свидетельство временного убежища в РФ
            RussianCitizenPassport = 14,                          // Паспорт гражданина РФ
            USSRPassport = 2,                                     // Загранпаспорт гражданина СССР
            RussianForeignPassport = 15,                          // Загранпаспорт гражданина РФ
            MilitaryID = 7,                                       // Военный билет
            ForeignPassport = 9,                                  // Иностранный паспорт
            MinistryOfShippingPassport = 6,                       // Паспорт Минморфлота
            IDCard = 239                                           // Удостоверение
        }
    }
}
