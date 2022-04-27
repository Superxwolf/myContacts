using myContacts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace myContacts.Data
{
    public class ContactsContext : DbContext
    {
        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options)
        {
        }

        public DbSet<ContactModel> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var trimEnd = new ValueConverter<string, string>(v => v.TrimEnd(), v => v.TrimEnd());

            modelBuilder.Entity<ContactModel>(entity =>
            {
                entity.ToTable("Contacts");
                entity.Property(e => e.Name).HasConversion(trimEnd);
                entity.Property(e => e.eMail).HasConversion(trimEnd);
                entity.Property(e => e.Phone).HasConversion(trimEnd);
                entity.Property(e => e.Fax).HasConversion(trimEnd);
                entity.Property(e => e.LastUpdateUserName).HasConversion(trimEnd);
            });
        }
    }
}
