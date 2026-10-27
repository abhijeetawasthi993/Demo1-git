using AbcPharmacy.Data;
using AbcPharmacy.Models;
using Microsoft.AspNetCore.Mvc;

namespace AbcPharmacy.Controllers
{
    [Route("api/sales")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            List<Sale> lst = JsonData.GetSales();
            lst.Reverse();
            return Ok(lst);
        }

        [HttpPost]
        public IActionResult Save(Sale obj)
        {
            if (obj == null || obj.MedicineId == 0)
                return BadRequest("Please select medicine");

            if (obj.QtySold <= 0)
                return BadRequest("Qty should be more than 0");

            List<Medicine> meds = JsonData.GetMedicines();
            Medicine m = null;
            for (int i = 0; i < meds.Count; i++)
            {
                if (meds[i].Id == obj.MedicineId)
                {
                    m = meds[i];
                    break;
                }
            }

            if (m == null)
                return BadRequest("Medicine not found");

            if (m.Quantity < obj.QtySold)
                return BadRequest("Only " + m.Quantity + " left in stock");

            m.Quantity = m.Quantity - obj.QtySold;
            JsonData.SaveMedicines(meds);

            List<Sale> sales = JsonData.GetSales();
            int nextId = 1;
            if (sales.Count > 0)
                nextId = sales.Max(x => x.Id) + 1;

            Sale s = new Sale();
            s.Id = nextId;
            s.MedicineId = m.Id;
            s.MedicineName = m.FullName;
            s.QtySold = obj.QtySold;
            s.UnitPrice = m.Price;
            s.Amount = Math.Round(m.Price * obj.QtySold, 2);
            s.SoldOn = DateTime.Now;
            sales.Add(s);
            JsonData.SaveSales(sales);

            return Ok(s);
        }
    }
}
