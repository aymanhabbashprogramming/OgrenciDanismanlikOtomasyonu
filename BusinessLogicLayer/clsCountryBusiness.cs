using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsCountryBusiness : clsCountry
    {
        private clsCountryBusiness(clsCountry country)
        {
            CountryId   = country.CountryId;
            CountryName = country.CountryName;
            IsoCode     = country.IsoCode;
        }

        public static clsCountryBusiness? Find(int countryId)
        {
            clsCountry? country = clsCountryData.GetCountryInfoByID(countryId);
            return country is null ? null : new clsCountryBusiness(country);
        }

        public static List<clsCountry> GetAllCountries() =>
            clsCountryData.GetAllCountries();

        public static List<clsCountry> GetTurkiyeFirst()
        {
            List<clsCountry> countries = clsCountryData.GetAllCountries();
            int turkiyeIndex = countries.FindIndex(country => country.IsoCode == "TR");
            if (turkiyeIndex > 0)
            {
                clsCountry turkey = countries[turkiyeIndex];
                countries.RemoveAt(turkiyeIndex);
                countries.Insert(0, turkey);
            }
            return countries;
        }

        public static bool IsCountryExist(int countryId) =>
            clsCountryData.IsCountryExist(countryId);
    }
}
