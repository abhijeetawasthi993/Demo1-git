using System.Text.Json;
using AbcPharmacy.Models;

namespace AbcPharmacy.Data
{
    public class JsonData
    {
        static object objLock = new object();
        static JsonSerializerOptions jopt = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        static string MedFile()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "medicines.json");
        }

        static string SaleFile()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "sales.json");
        }

        static void CheckFolder()
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(MedFile()))
                File.WriteAllText(MedFile(), "[]");

            if (!File.Exists(SaleFile()))
                File.WriteAllText(SaleFile(), "[]");
        }

        public static List<Medicine> GetMedicines()
        {
            lock (objLock)
            {
                CheckFolder();
                string json = File.ReadAllText(MedFile());
                List<Medicine> lst = JsonSerializer.Deserialize<List<Medicine>>(json, jopt);
                if (lst == null)
                    lst = new List<Medicine>();
                return lst;
            }
        }

        public static void SaveMedicines(List<Medicine> lst)
        {
            lock (objLock)
            {
                CheckFolder();
                string json = JsonSerializer.Serialize(lst, jopt);
                File.WriteAllText(MedFile(), json);
            }
        }

        public static List<Sale> GetSales()
        {
            lock (objLock)
            {
                CheckFolder();
                string json = File.ReadAllText(SaleFile());
                List<Sale> lst = JsonSerializer.Deserialize<List<Sale>>(json, jopt);
                if (lst == null)
                    lst = new List<Sale>();
                return lst;
            }
        }

        public static void SaveSales(List<Sale> lst)
        {
            lock (objLock)
            {
                CheckFolder();
                string json = JsonSerializer.Serialize(lst, jopt);
                File.WriteAllText(SaleFile(), json);
            }
        }
    }
}
