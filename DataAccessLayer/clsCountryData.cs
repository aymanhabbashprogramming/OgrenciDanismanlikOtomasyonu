using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsCountryData
    {
        private static clsCountry _MapCountry(SqlDataReader reader) => new clsCountry
        {
            CountryId   = (int)reader["CountryId"],
            CountryName = (string)reader["CountryName"],
            IsoCode     = ((string)reader["IsoCode"]).Trim()
        };

        public static clsCountry? GetCountryInfoByID(int countryId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT CountryId, CountryName, IsoCode FROM Countries WHERE CountryId = @CountryId", conn);
                cmd.Parameters.AddWithValue("@CountryId", countryId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapCountry(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsCountryData.GetCountryInfoByID] {ex}");
                return null;
            }
        }

        public static List<clsCountry> GetAllCountries()
        {
            List<clsCountry> countryList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT CountryId, CountryName, IsoCode FROM Countries ORDER BY CountryName", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    countryList.Add(_MapCountry(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsCountryData.GetAllCountries] {ex}");
            }
            return countryList;
        }

        public static bool IsCountryExist(int countryId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Countries WHERE CountryId = @CountryId", conn);
                cmd.Parameters.AddWithValue("@CountryId", countryId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsCountryData.IsCountryExist] {ex}");
                return false;
            }
        }
    }
}
