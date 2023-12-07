using Microsoft.EntityFrameworkCore;
using _14112023crudcodefirst.Models;

namespace _14112023crudcodefirst.Models
{
    public class EmployeeDbContext:DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; } = default!;
        public DbSet<person> person { get; set; } = default!;
        public DbSet<_14112023crudcodefirst.Models.StudentModel> StudentModel { get; set; } = default!;
        //public DbSet<StudentModel> StudentModel { get; set; } = default!;
        //public DbSet<UploadFileModel> UploadfileModel { get; set; } = default!;


    }
}
