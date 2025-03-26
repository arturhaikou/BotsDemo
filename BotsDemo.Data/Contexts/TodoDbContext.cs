using BotsDemo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BotsDemo.Data.Contexts
{
    public class TodoDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    }
}
