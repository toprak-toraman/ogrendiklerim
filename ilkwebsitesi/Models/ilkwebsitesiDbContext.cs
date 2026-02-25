using ilkwebsitesi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ilkwebsitesiDbContext : IdentityDbContext<User>
{
    public ilkwebsitesiDbContext(DbContextOptions<ilkwebsitesiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }
}
