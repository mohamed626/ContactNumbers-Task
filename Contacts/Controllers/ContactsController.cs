using Contacts.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Dynamic.Core;

namespace Contacts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult LoadContactsData()
        {

            var pagesize = int.Parse(Request.Form["length"]);
            var skip = int.Parse(Request.Form["start"]);
            var searchValue = Request.Form["search[value]"];
            var sortColumn = Request.Form[string.Concat("columns[", Request.Form["order[0][column]"], "][name]")];
            var sortColumnDirection = Request.Form["order[0][dir]"];


            IQueryable<Contact> contacts = _context.Contacts.Where(m => string.IsNullOrEmpty(searchValue) ? true:
            (m.Name.Contains(searchValue) || m.Phone.Contains(searchValue) || m.Address.Contains(searchValue) || m.Notes.Contains(searchValue)) );

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                contacts = contacts.OrderBy(string.Concat(sortColumn, " ", sortColumnDirection));

            var data = contacts.Skip(skip).Take(pagesize).ToList();

            var recordsTotal = contacts.Count();

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data };

            return Ok(jsonData);

        }


        [HttpPost("AddEditContact")]
        [ValidateAntiForgeryToken]
        public IActionResult AddEditContact(Contact newContact)
        {
            try
            {
                if (newContact.Id == 0)
                {
                    _context.Contacts.Add(newContact);
                    _context.SaveChanges();
                    return Ok(newContact);

                }
                else
                {
                    var oldContact = _context.Contacts.Find(newContact.Id);
                    oldContact.Name = newContact.Name;
                    oldContact.Phone = newContact.Phone;
                    oldContact.Address = newContact.Address;
                    oldContact.Notes = newContact.Notes;

                    _context.Contacts.Update(oldContact);
                    _context.SaveChanges();

                    return Ok(oldContact);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("DeleteContact")]
        //[Route("api/Contacts/DeleteContact")]

        public IActionResult DeleteContact(int contactId)
        {
            try
            {
      
                var oldContact = _context.Contacts.Find(contactId);

                if (oldContact == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                _context.Contacts.Remove(oldContact);
                _context.SaveChanges();

                return Ok();
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
