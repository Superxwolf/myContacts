using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myContacts.Data;
using myContacts.Models;
using System.Security.Claims;

namespace myContacts.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactsAPIController : ControllerBase
    {
        ContactsContext contactContext;

        public ContactsAPIController(ContactsContext contactsContext)
        {
            this.contactContext = contactsContext;
        }

        /// <summary>
        /// Obtains list of all contacts
        /// </summary>
        /// <returns>An array of ContactModel</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<ContactModel>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ListContacts()
        {
            var contacts = await contactContext.Contacts
                .OrderBy(contact => contact.Name)
                .ToListAsync();

            return Ok(contacts);
        }

        /// <summary>
        /// Returns a filtered list of contacts
        /// </summary>
        /// <param name="search">Search term for the contacts</param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<ContactModel>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SearchContacts(string search)
        {
            var filteredContacts = await contactContext.Contacts
                .Where(contact => contact.ContactID.ToString().Contains(search) || contact.Name.ToLower().Contains(search))
                .OrderBy(contact => contact.Name)
                .ToListAsync();

            return Ok(filteredContacts);
        }

        /// <summary>
        /// Returns a specific contact
        /// </summary>
        /// <param name="id">ContactID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ContactModel), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetContact(int id)
        {
            var contact = await contactContext.Contacts.FindAsync(id);

            if(contact == null)
                return NotFound();

            return Ok(contact);
        }

        /// <summary>
        /// Update a contact
        /// </summary>
        /// <param name="updateContact">The contact, with the updated information</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateContact(UpdateContactRequestModel updateContact)
        {
            var username = User.Identity.Name;
            var contact = await contactContext.Contacts.FindAsync(updateContact.ContactID);

            if(contact == null)
                return NotFound();
            
            contactContext.Contacts.Update(contact);

            contact.Name = updateContact.Name;
            contact.Phone = updateContact.Phone;
            contact.eMail = updateContact.eMail;
            contact.Fax = updateContact.Fax;
            contact.Notes = updateContact.Notes;
            contact.LastUpdateDate = DateTime.Now;
            contact.LastUpdateUserName = username;

            var (isValid, message) = contact.IsValid();

            if (!isValid)
            {
                return BadRequest(new { errors = message });
            }

            await contactContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="updateContact">Contact's information</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateContact(CreateContactRequestModel updateContact)
        {
            var username = User.Identity.Name;

            var contact = new ContactModel
            {
                Name = updateContact.Name,
                Phone = updateContact.Phone,
                eMail = updateContact.eMail,
                Fax = updateContact.Fax,
                Notes = updateContact.Notes,
                LastUpdateDate = DateTime.Now,
                LastUpdateUserName = username
            };

            var (isValid, message) = contact.IsValid();

            if (!isValid)
            {
                return BadRequest(new {errors = message});
            }

            contactContext.Add(contact);
            await contactContext.SaveChangesAsync();
            return Ok();
        }

        /*
        // DELETE api/contacts/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await contactContext.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            contactContext.Contacts.Remove(contact);
            await contactContext.SaveChangesAsync();
            return Ok();
        }
        */

        
    }
}