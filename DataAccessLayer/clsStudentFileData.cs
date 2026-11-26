using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsStudentFileData
    {
        private static clsStudentFile _MapStudentFile(SqlDataReader reader) => new clsStudentFile
        {
            StudentFileId          = (int)reader["StudentFileId"],
            StudentId              = (int)reader["StudentId"],
            FilledAt               = (DateTime)reader["FilledAt"],
            UpdatedAt              = reader["UpdatedAt"]              == DBNull.Value ? null : (DateTime?)reader["UpdatedAt"],
            BloodType              = reader["BloodType"]              == DBNull.Value ? null : (string)reader["BloodType"],
            BirthPlace             = reader["BirthPlace"]             == DBNull.Value ? null : (string)reader["BirthPlace"],
            BirthDate              = reader["BirthDate"]              == DBNull.Value ? null : DateOnly.FromDateTime((DateTime)reader["BirthDate"]),
            Nationality            = reader["Nationality"]            == DBNull.Value ? null : (string)reader["Nationality"],
            PhotoPath              = reader["PhotoPath"]              == DBNull.Value ? null : (string)reader["PhotoPath"],
            HousingType            = reader["HousingType"]            == DBNull.Value ? null : (string)reader["HousingType"],
            HousingTypeDescription = reader["HousingTypeDescription"] == DBNull.Value ? null : (string)reader["HousingTypeDescription"],
            Address                = reader["Address"]                == DBNull.Value ? null : (string)reader["Address"],
            FamilyAddress          = reader["FamilyAddress"]          == DBNull.Value ? null : (string)reader["FamilyAddress"],
            FamilyPhone            = reader["FamilyPhone"]            == DBNull.Value ? null : (string)reader["FamilyPhone"],
            PreviousSchool         = reader["PreviousSchool"]         == DBNull.Value ? null : (string)reader["PreviousSchool"],
            HasDisability          = reader["HasDisability"]          == DBNull.Value ? null : (bool?)(bool)reader["HasDisability"],
            DisabilityRate         = reader["DisabilityRate"]         == DBNull.Value ? null : (int?)(int)reader["DisabilityRate"],
            DisabilityDescription  = reader["DisabilityDescription"]  == DBNull.Value ? null : (string)reader["DisabilityDescription"],
            HasChronicIllness      = reader["HasChronicIllness"]      == DBNull.Value ? null : (bool?)(bool)reader["HasChronicIllness"],
            ChronicIllnessDetails  = reader["ChronicIllnessDetails"]  == DBNull.Value ? null : (string)reader["ChronicIllnessDetails"],
            IsMartyrOrVeteranChild = reader["IsMartyrOrVeteranChild"] == DBNull.Value ? null : (bool?)(bool)reader["IsMartyrOrVeteranChild"],
            StayedInCareHome       = reader["StayedInCareHome"]       == DBNull.Value ? null : (bool?)(bool)reader["StayedInCareHome"],
            Gano                   = reader["Gano"]                   == DBNull.Value ? null : (decimal?)(decimal)reader["Gano"],
            ParentsStatus          = reader["ParentsStatus"]          == DBNull.Value ? null : (string)reader["ParentsStatus"],
            IsMotherDisabled       = reader["IsMotherDisabled"]       == DBNull.Value ? null : (bool?)(bool)reader["IsMotherDisabled"],
            IsFatherDisabled       = reader["IsFatherDisabled"]       == DBNull.Value ? null : (bool?)(bool)reader["IsFatherDisabled"],
            IsSiblingDisabled      = reader["IsSiblingDisabled"]      == DBNull.Value ? null : (bool?)(bool)reader["IsSiblingDisabled"],
            FamilyMonthlyIncome    = reader["FamilyMonthlyIncome"]    == DBNull.Value ? null : (decimal?)(decimal)reader["FamilyMonthlyIncome"],
            FamilyHomeType         = reader["FamilyHomeType"]         == DBNull.Value ? null : (string)reader["FamilyHomeType"],
            OwnsSummerHouse        = reader["OwnsSummerHouse"]        == DBNull.Value ? null : (bool?)(bool)reader["OwnsSummerHouse"],
            OwnsShop               = reader["OwnsShop"]               == DBNull.Value ? null : (bool?)(bool)reader["OwnsShop"],
            OwnsRentalHouse        = reader["OwnsRentalHouse"]        == DBNull.Value ? null : (bool?)(bool)reader["OwnsRentalHouse"],
            OwnsCar                = reader["OwnsCar"]                == DBNull.Value ? null : (bool?)(bool)reader["OwnsCar"],
            SiblingsPrimary        = reader["SiblingsPrimary"]        == DBNull.Value ? null : (int?)(int)reader["SiblingsPrimary"],
            SiblingsHighSchool     = reader["SiblingsHighSchool"]     == DBNull.Value ? null : (int?)(int)reader["SiblingsHighSchool"],
            SiblingsUniInCity      = reader["SiblingsUniInCity"]      == DBNull.Value ? null : (int?)(int)reader["SiblingsUniInCity"],
            SiblingsUniOutCity     = reader["SiblingsUniOutCity"]     == DBNull.Value ? null : (int?)(int)reader["SiblingsUniOutCity"],
            ReceivesScholarship    = reader["ReceivesScholarship"]    == DBNull.Value ? null : (bool?)(bool)reader["ReceivesScholarship"],
            ScholarshipAmount      = reader["ScholarshipAmount"]      == DBNull.Value ? null : (decimal?)(decimal)reader["ScholarshipAmount"],
            ReceivesLoan           = reader["ReceivesLoan"]           == DBNull.Value ? null : (bool?)(bool)reader["ReceivesLoan"],
            LoanAmount             = reader["LoanAmount"]             == DBNull.Value ? null : (decimal?)(decimal)reader["LoanAmount"],
            IsWorking              = reader["IsWorking"]              == DBNull.Value ? null : (bool?)(bool)reader["IsWorking"],
            WorkIncome             = reader["WorkIncome"]             == DBNull.Value ? null : (decimal?)(decimal)reader["WorkIncome"]
        };

        private const string _fileColumns = @"
            StudentFileId, StudentId, FilledAt, UpdatedAt,
            BloodType, BirthPlace, BirthDate, Nationality, PhotoPath,
            HousingType, HousingTypeDescription, Address, FamilyAddress, FamilyPhone,
            PreviousSchool, HasDisability, DisabilityRate, DisabilityDescription,
            HasChronicIllness, ChronicIllnessDetails, IsMartyrOrVeteranChild, StayedInCareHome,
            Gano, ParentsStatus, IsMotherDisabled, IsFatherDisabled, IsSiblingDisabled,
            FamilyMonthlyIncome, FamilyHomeType, OwnsSummerHouse, OwnsShop, OwnsRentalHouse,
            OwnsCar, SiblingsPrimary, SiblingsHighSchool, SiblingsUniInCity, SiblingsUniOutCity,
            ReceivesScholarship, ScholarshipAmount, ReceivesLoan, LoanAmount, IsWorking, WorkIncome";

        public static clsStudentFile? GetStudentFileByStudentId(int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    $"SELECT {_fileColumns} FROM StudentFiles WHERE StudentId = @StudentId", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapStudentFile(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentFileData.GetStudentFileByStudentId] {ex}");
                return null;
            }
        }

        public static clsStudentFormInfo? GetStudentFormInfo(int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT
                        Student.StudentId,
                        Student.FirstName + N' ' + Student.LastName AS FullName,
                        Student.StudentNumber,
                        Student.NationalId,
                        Department.DepartmentName,
                        Faculty.FacultyName,
                        Student.EducationType,
                        CASE
                            WHEN Student.IsInternational = 1 AND NationalityCountry.CountryName IS NOT NULL
                                THEN NationalityCountry.CountryName
                            WHEN Student.IsInternational = 1
                                THEN N'Uluslararası'
                            ELSE N'T.C.'
                        END AS NationalityLabel,
                        Student.MobilePhone,
                        Student.Email,
                        StudentFile.StudentFileId,
                        StudentFile.FilledAt,
                        StudentFile.UpdatedAt,
                        StudentFile.BloodType,
                        StudentFile.BirthPlace,
                        StudentFile.BirthDate,
                        StudentFile.HousingType,
                        StudentFile.HousingTypeDescription,
                        StudentFile.Address,
                        StudentFile.FamilyAddress,
                        StudentFile.FamilyPhone,
                        StudentFile.PreviousSchool,
                        StudentFile.HasDisability,
                        StudentFile.DisabilityRate,
                        StudentFile.DisabilityDescription,
                        StudentFile.HasChronicIllness,
                        StudentFile.ChronicIllnessDetails,
                        StudentFile.IsMartyrOrVeteranChild,
                        StudentFile.StayedInCareHome,
                        StudentFile.Gano,
                        StudentFile.ParentsStatus,
                        StudentFile.IsMotherDisabled,
                        StudentFile.IsFatherDisabled,
                        StudentFile.IsSiblingDisabled,
                        StudentFile.FamilyMonthlyIncome,
                        StudentFile.FamilyHomeType,
                        StudentFile.OwnsSummerHouse,
                        StudentFile.OwnsShop,
                        StudentFile.OwnsRentalHouse,
                        StudentFile.OwnsCar,
                        StudentFile.SiblingsPrimary,
                        StudentFile.SiblingsHighSchool,
                        StudentFile.SiblingsUniInCity,
                        StudentFile.SiblingsUniOutCity,
                        StudentFile.ReceivesScholarship,
                        StudentFile.ScholarshipAmount,
                        StudentFile.ReceivesLoan,
                        StudentFile.LoanAmount,
                        StudentFile.IsWorking,
                        StudentFile.WorkIncome
                    FROM   Students    AS Student
                    INNER JOIN Departments AS Department ON Student.DepartmentId     = Department.DepartmentId
                    INNER JOIN Faculties   AS Faculty    ON Department.FacultyId     = Faculty.FacultyId
                    LEFT  JOIN StudentFiles AS StudentFile ON StudentFile.StudentId  = Student.StudentId
                    LEFT  JOIN Countries AS NationalityCountry ON Student.NationalityCountryId = NationalityCountry.CountryId
                    WHERE Student.StudentId = @StudentId", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;

                clsStudentFormInfo info = new()
                {
                    StudentId        = (int)reader["StudentId"],
                    FullName         = (string)reader["FullName"],
                    StudentNumber    = (string)reader["StudentNumber"],
                    NationalId       = (string)reader["NationalId"],
                    DepartmentName   = (string)reader["DepartmentName"],
                    FacultyName      = (string)reader["FacultyName"],
                    EducationType    = (string)reader["EducationType"],
                    NationalityLabel = (string)reader["NationalityLabel"],
                    MobilePhone      = reader["MobilePhone"]    == DBNull.Value ? null : (string)reader["MobilePhone"],
                    Email            = reader["Email"]          == DBNull.Value ? null : (string)reader["Email"],
                    StudentFileId    = reader["StudentFileId"]  == DBNull.Value ? 0    : (int)reader["StudentFileId"],
                    FilledAt         = reader["FilledAt"]       == DBNull.Value ? null : (DateTime?)reader["FilledAt"],
                    UpdatedAt        = reader["UpdatedAt"]      == DBNull.Value ? null : (DateTime?)reader["UpdatedAt"],
                    BloodType              = reader["BloodType"]              == DBNull.Value ? null : (string)reader["BloodType"],
                    BirthPlace             = reader["BirthPlace"]             == DBNull.Value ? null : (string)reader["BirthPlace"],
                    BirthDate              = reader["BirthDate"]              == DBNull.Value ? null : DateOnly.FromDateTime((DateTime)reader["BirthDate"]),
                    HousingType            = reader["HousingType"]            == DBNull.Value ? null : (string)reader["HousingType"],
                    HousingTypeDescription = reader["HousingTypeDescription"] == DBNull.Value ? null : (string)reader["HousingTypeDescription"],
                    Address                = reader["Address"]                == DBNull.Value ? null : (string)reader["Address"],
                    FamilyAddress          = reader["FamilyAddress"]          == DBNull.Value ? null : (string)reader["FamilyAddress"],
                    FamilyPhone            = reader["FamilyPhone"]            == DBNull.Value ? null : (string)reader["FamilyPhone"],
                    PreviousSchool         = reader["PreviousSchool"]         == DBNull.Value ? null : (string)reader["PreviousSchool"],
                    Gano                   = reader["Gano"]                   == DBNull.Value ? null : (decimal?)(decimal)reader["Gano"],
                    HasDisability          = reader["HasDisability"]          == DBNull.Value ? null : (bool?)(bool)reader["HasDisability"],
                    DisabilityRate         = reader["DisabilityRate"]         == DBNull.Value ? null : (int?)(int)reader["DisabilityRate"],
                    DisabilityDescription  = reader["DisabilityDescription"]  == DBNull.Value ? null : (string)reader["DisabilityDescription"],
                    HasChronicIllness      = reader["HasChronicIllness"]      == DBNull.Value ? null : (bool?)(bool)reader["HasChronicIllness"],
                    ChronicIllnessDetails  = reader["ChronicIllnessDetails"]  == DBNull.Value ? null : (string)reader["ChronicIllnessDetails"],
                    IsMartyrOrVeteranChild = reader["IsMartyrOrVeteranChild"] == DBNull.Value ? null : (bool?)(bool)reader["IsMartyrOrVeteranChild"],
                    StayedInCareHome       = reader["StayedInCareHome"]       == DBNull.Value ? null : (bool?)(bool)reader["StayedInCareHome"],
                    ParentsStatus          = reader["ParentsStatus"]          == DBNull.Value ? null : (string)reader["ParentsStatus"],
                    IsMotherDisabled       = reader["IsMotherDisabled"]       == DBNull.Value ? null : (bool?)(bool)reader["IsMotherDisabled"],
                    IsFatherDisabled       = reader["IsFatherDisabled"]       == DBNull.Value ? null : (bool?)(bool)reader["IsFatherDisabled"],
                    IsSiblingDisabled      = reader["IsSiblingDisabled"]      == DBNull.Value ? null : (bool?)(bool)reader["IsSiblingDisabled"],
                    FamilyMonthlyIncome    = reader["FamilyMonthlyIncome"]    == DBNull.Value ? null : (decimal?)(decimal)reader["FamilyMonthlyIncome"],
                    FamilyHomeType         = reader["FamilyHomeType"]         == DBNull.Value ? null : (string)reader["FamilyHomeType"],
                    OwnsSummerHouse        = reader["OwnsSummerHouse"]        == DBNull.Value ? null : (bool?)(bool)reader["OwnsSummerHouse"],
                    OwnsShop               = reader["OwnsShop"]               == DBNull.Value ? null : (bool?)(bool)reader["OwnsShop"],
                    OwnsRentalHouse        = reader["OwnsRentalHouse"]        == DBNull.Value ? null : (bool?)(bool)reader["OwnsRentalHouse"],
                    OwnsCar                = reader["OwnsCar"]                == DBNull.Value ? null : (bool?)(bool)reader["OwnsCar"],
                    SiblingsPrimary        = reader["SiblingsPrimary"]        == DBNull.Value ? null : (int?)(int)reader["SiblingsPrimary"],
                    SiblingsHighSchool     = reader["SiblingsHighSchool"]     == DBNull.Value ? null : (int?)(int)reader["SiblingsHighSchool"],
                    SiblingsUniInCity      = reader["SiblingsUniInCity"]      == DBNull.Value ? null : (int?)(int)reader["SiblingsUniInCity"],
                    SiblingsUniOutCity     = reader["SiblingsUniOutCity"]     == DBNull.Value ? null : (int?)(int)reader["SiblingsUniOutCity"],
                    ReceivesScholarship    = reader["ReceivesScholarship"]    == DBNull.Value ? null : (bool?)(bool)reader["ReceivesScholarship"],
                    ScholarshipAmount      = reader["ScholarshipAmount"]      == DBNull.Value ? null : (decimal?)(decimal)reader["ScholarshipAmount"],
                    ReceivesLoan           = reader["ReceivesLoan"]           == DBNull.Value ? null : (bool?)(bool)reader["ReceivesLoan"],
                    LoanAmount             = reader["LoanAmount"]             == DBNull.Value ? null : (decimal?)(decimal)reader["LoanAmount"],
                    IsWorking              = reader["IsWorking"]              == DBNull.Value ? null : (bool?)(bool)reader["IsWorking"],
                    WorkIncome             = reader["WorkIncome"]             == DBNull.Value ? null : (decimal?)(decimal)reader["WorkIncome"]
                };
                return info;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentFileData.GetStudentFormInfo] {ex}");
                return null;
            }
        }

        public static bool SaveStudentForm(
            int              studentId,
            string?          mobilePhone,
            string?          email,
            clsStudentFile   studentFile,
            clsParent        mother,
            clsParent        father)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmdStudent = new(@"
                        UPDATE Students
                        SET MobilePhone = @MobilePhone,
                            Email       = @Email
                        WHERE StudentId = @StudentId", conn, transaction);
                    cmdStudent.Parameters.AddWithValue("@MobilePhone", (object?)mobilePhone ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@Email",       (object?)email       ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@StudentId",   studentId);
                    cmdStudent.ExecuteNonQuery();

                    int studentFileId;
                    if (studentFile.StudentFileId <= 0)
                    {
                        using SqlCommand cmdInsertFile = new(@"
                            INSERT INTO StudentFiles
                                (StudentId, FilledAt, UpdatedAt,
                                 BloodType, BirthPlace, BirthDate,
                                 HousingType, HousingTypeDescription, Address, FamilyAddress, FamilyPhone,
                                 PreviousSchool, HasDisability, DisabilityRate, DisabilityDescription,
                                 HasChronicIllness, ChronicIllnessDetails, IsMartyrOrVeteranChild, StayedInCareHome,
                                 Gano, ParentsStatus, IsMotherDisabled, IsFatherDisabled, IsSiblingDisabled,
                                 FamilyMonthlyIncome, FamilyHomeType, OwnsSummerHouse, OwnsShop, OwnsRentalHouse,
                                 OwnsCar, SiblingsPrimary, SiblingsHighSchool, SiblingsUniInCity, SiblingsUniOutCity,
                                 ReceivesScholarship, ScholarshipAmount, ReceivesLoan, LoanAmount, IsWorking, WorkIncome)
                            VALUES
                                (@StudentId, SYSDATETIME(), SYSDATETIME(),
                                 @BloodType, @BirthPlace, @BirthDate,
                                 @HousingType, @HousingTypeDescription, @Address, @FamilyAddress, @FamilyPhone,
                                 @PreviousSchool, @HasDisability, @DisabilityRate, @DisabilityDescription,
                                 @HasChronicIllness, @ChronicIllnessDetails, @IsMartyrOrVeteranChild, @StayedInCareHome,
                                 @Gano, @ParentsStatus, @IsMotherDisabled, @IsFatherDisabled, @IsSiblingDisabled,
                                 @FamilyMonthlyIncome, @FamilyHomeType, @OwnsSummerHouse, @OwnsShop, @OwnsRentalHouse,
                                 @OwnsCar, @SiblingsPrimary, @SiblingsHighSchool, @SiblingsUniInCity, @SiblingsUniOutCity,
                                 @ReceivesScholarship, @ScholarshipAmount, @ReceivesLoan, @LoanAmount, @IsWorking, @WorkIncome);
                            SELECT SCOPE_IDENTITY();", conn, transaction);
                        _AddStudentFileParams(cmdInsertFile, studentId, studentFile);
                        object? result = cmdInsertFile.ExecuteScalar();
                        if (result is null)
                        {
                            transaction.Rollback();
                            return false;
                        }
                        studentFileId = Convert.ToInt32(result);
                    }
                    else
                    {
                        studentFileId = studentFile.StudentFileId;
                        using SqlCommand cmdUpdateFile = new(@"
                            UPDATE StudentFiles
                            SET UpdatedAt               = SYSDATETIME(),
                                BloodType               = @BloodType,
                                BirthPlace              = @BirthPlace,
                                BirthDate               = @BirthDate,
                                HousingType             = @HousingType,
                                HousingTypeDescription  = @HousingTypeDescription,
                                Address                 = @Address,
                                FamilyAddress           = @FamilyAddress,
                                FamilyPhone             = @FamilyPhone,
                                PreviousSchool          = @PreviousSchool,
                                HasDisability           = @HasDisability,
                                DisabilityRate          = @DisabilityRate,
                                DisabilityDescription   = @DisabilityDescription,
                                HasChronicIllness       = @HasChronicIllness,
                                ChronicIllnessDetails   = @ChronicIllnessDetails,
                                IsMartyrOrVeteranChild  = @IsMartyrOrVeteranChild,
                                StayedInCareHome        = @StayedInCareHome,
                                Gano                    = @Gano,
                                ParentsStatus           = @ParentsStatus,
                                IsMotherDisabled        = @IsMotherDisabled,
                                IsFatherDisabled        = @IsFatherDisabled,
                                IsSiblingDisabled       = @IsSiblingDisabled,
                                FamilyMonthlyIncome     = @FamilyMonthlyIncome,
                                FamilyHomeType          = @FamilyHomeType,
                                OwnsSummerHouse         = @OwnsSummerHouse,
                                OwnsShop                = @OwnsShop,
                                OwnsRentalHouse         = @OwnsRentalHouse,
                                OwnsCar                 = @OwnsCar,
                                SiblingsPrimary         = @SiblingsPrimary,
                                SiblingsHighSchool      = @SiblingsHighSchool,
                                SiblingsUniInCity       = @SiblingsUniInCity,
                                SiblingsUniOutCity      = @SiblingsUniOutCity,
                                ReceivesScholarship     = @ReceivesScholarship,
                                ScholarshipAmount       = @ScholarshipAmount,
                                ReceivesLoan            = @ReceivesLoan,
                                LoanAmount              = @LoanAmount,
                                IsWorking               = @IsWorking,
                                WorkIncome              = @WorkIncome
                            WHERE StudentFileId = @StudentFileId", conn, transaction);
                        _AddStudentFileParams(cmdUpdateFile, studentId, studentFile);
                        cmdUpdateFile.Parameters.AddWithValue("@StudentFileId", studentFileId);
                        cmdUpdateFile.ExecuteNonQuery();
                    }

                    _UpsertParent(conn, transaction, studentFileId, mother);
                    _UpsertParent(conn, transaction, studentFileId, father);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsStudentFileData.SaveStudentForm inner] {ex}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentFileData.SaveStudentForm outer] {ex}");
                return false;
            }
        }

        private static void _AddStudentFileParams(SqlCommand cmd, int studentId, clsStudentFile file)
        {
            cmd.Parameters.AddWithValue("@StudentId",              studentId);
            cmd.Parameters.AddWithValue("@BloodType",              (object?)file.BloodType              ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BirthPlace",             (object?)file.BirthPlace             ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BirthDate",              file.BirthDate.HasValue
                                                                       ? (object)file.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
                                                                       : DBNull.Value);
            cmd.Parameters.AddWithValue("@HousingType",            (object?)file.HousingType            ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HousingTypeDescription", (object?)file.HousingTypeDescription ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address",                (object?)file.Address                ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FamilyAddress",          (object?)file.FamilyAddress          ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FamilyPhone",            (object?)file.FamilyPhone            ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PreviousSchool",         (object?)file.PreviousSchool         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HasDisability",          (object?)file.HasDisability          ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DisabilityRate",         (object?)file.DisabilityRate         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DisabilityDescription",  (object?)file.DisabilityDescription  ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HasChronicIllness",      (object?)file.HasChronicIllness      ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChronicIllnessDetails",  (object?)file.ChronicIllnessDetails  ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsMartyrOrVeteranChild", (object?)file.IsMartyrOrVeteranChild ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StayedInCareHome",       (object?)file.StayedInCareHome       ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Gano",                   (object?)file.Gano                   ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ParentsStatus",          (object?)file.ParentsStatus          ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsMotherDisabled",       (object?)file.IsMotherDisabled       ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsFatherDisabled",       (object?)file.IsFatherDisabled       ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsSiblingDisabled",      (object?)file.IsSiblingDisabled      ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FamilyMonthlyIncome",    (object?)file.FamilyMonthlyIncome    ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FamilyHomeType",         (object?)file.FamilyHomeType         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OwnsSummerHouse",        (object?)file.OwnsSummerHouse        ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OwnsShop",               (object?)file.OwnsShop               ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OwnsRentalHouse",        (object?)file.OwnsRentalHouse        ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OwnsCar",                (object?)file.OwnsCar                ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiblingsPrimary",        (object?)file.SiblingsPrimary        ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiblingsHighSchool",     (object?)file.SiblingsHighSchool     ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiblingsUniInCity",      (object?)file.SiblingsUniInCity      ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiblingsUniOutCity",     (object?)file.SiblingsUniOutCity     ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReceivesScholarship",    (object?)file.ReceivesScholarship    ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ScholarshipAmount",      (object?)file.ScholarshipAmount      ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReceivesLoan",           (object?)file.ReceivesLoan           ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LoanAmount",             (object?)file.LoanAmount             ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsWorking",              (object?)file.IsWorking              ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WorkIncome",             (object?)file.WorkIncome             ?? DBNull.Value);
        }

        private static void _UpsertParent(
            SqlConnection  conn,
            SqlTransaction transaction,
            int            studentFileId,
            clsParent      parent)
        {
            if (parent.ParentId <= 0)
            {
                using SqlCommand cmd = new(@"
                    INSERT INTO Parents
                        (StudentFileId, Relation, FirstName, LastName, Phone, Occupation,
                         HasSocialSecurity, Workplace, MonthlyIncome, EducationLevel, IsAlive, DeathYear)
                    VALUES
                        (@StudentFileId, @Relation, @FirstName, @LastName, @Phone, @Occupation,
                         @HasSocialSecurity, @Workplace, @MonthlyIncome, @EducationLevel, @IsAlive, @DeathYear)",
                    conn, transaction);
                _AddParentParams(cmd, studentFileId, parent);
                cmd.ExecuteNonQuery();
            }
            else
            {
                using SqlCommand cmd = new(@"
                    UPDATE Parents
                    SET FirstName         = @FirstName,
                        LastName          = @LastName,
                        Phone             = @Phone,
                        Occupation        = @Occupation,
                        HasSocialSecurity = @HasSocialSecurity,
                        Workplace         = @Workplace,
                        MonthlyIncome     = @MonthlyIncome,
                        EducationLevel    = @EducationLevel,
                        IsAlive           = @IsAlive,
                        DeathYear         = @DeathYear
                    WHERE ParentId = @ParentId",
                    conn, transaction);
                _AddParentParams(cmd, studentFileId, parent);
                cmd.Parameters.AddWithValue("@ParentId", parent.ParentId);
                cmd.ExecuteNonQuery();
            }
        }

        private static void _AddParentParams(SqlCommand cmd, int studentFileId, clsParent parent)
        {
            cmd.Parameters.AddWithValue("@StudentFileId",     studentFileId);
            cmd.Parameters.AddWithValue("@Relation",          parent.Relation);
            cmd.Parameters.AddWithValue("@FirstName",         (object?)parent.FirstName         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName",          (object?)parent.LastName          ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone",             (object?)parent.Phone             ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Occupation",        (object?)parent.Occupation        ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HasSocialSecurity", (object?)parent.HasSocialSecurity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Workplace",         (object?)parent.Workplace         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MonthlyIncome",     (object?)parent.MonthlyIncome     ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EducationLevel",    (object?)parent.EducationLevel    ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsAlive",           (object?)parent.IsAlive           ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DeathYear",         (object?)parent.DeathYear         ?? DBNull.Value);
        }
    }
}
