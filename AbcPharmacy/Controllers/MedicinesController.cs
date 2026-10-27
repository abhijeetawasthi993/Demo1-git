using AbcPharmacy.Data;
using AbcPharmacy.Models;
using Microsoft.AspNetCore.Mvc;

namespace AbcPharmacy.Controllers
{
    [Route("api/medicines")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string name)
        {
            List<Medicine> lst = JsonData.GetMedicines();

            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower().Trim();
                lst = lst.Where(x => x.FullName.ToLower().Contains(name) || x.Brand.ToLower().Contains(name)).ToList();
            }

            return Ok(lst);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Medicine m = JsonData.GetMedicines().Where(x => x.Id == id).FirstOrDefault();
            if (m == null)
                return NotFound("Medicine not found");
            return Ok(m);
        }

        [HttpPost]
        public IActionResult Save(Medicine obj)
        {
            if (obj == null)
                return BadRequest("No data");

            if (string.IsNullOrWhiteSpace(obj.FullName))
                return BadRequest("Please enter medicine name");

            if (string.IsNullOrWhiteSpace(obj.Brand))
                return BadRequest("Please enter brand");

            if (obj.ExpiryDate == DateTime.MinValue)
                return BadRequest("Please enter expiry date");

            if (obj.Quantity < 0)
                return BadRequest("Quantity is invalid");

            if (obj.Price <= 0)
                return BadRequest("Price is invalid");

            obj.Price = Math.Round(obj.Price, 2);
            obj.FullName = obj.FullName.Trim();
            obj.Brand = obj.Brand.Trim();
            if (obj.Notes == null)
                obj.Notes = "";
            else
                obj.Notes = obj.Notes.Trim();

            List<Medicine> lst = JsonData.GetMedicines();
            int nextId = 1;
            if (lst.Count > 0)
                nextId = lst.Max(x => x.Id) + 1;

            obj.Id = nextId;
            lst.Add(obj);
            JsonData.SaveMedicines(lst);

            return Ok(obj);
        }
    }
}
