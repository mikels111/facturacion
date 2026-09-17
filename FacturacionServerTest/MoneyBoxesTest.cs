using Facturacion.Server.Data;
using Facturacion.Server.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FacturacionServerTest
{
    public class MoneyBoxesTest
    {
        private readonly SqliteConnection _connection;
        readonly DbContextOptions<AppDbContext> _contextOptions;
        public MoneyBoxesTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 2. Configurar las opciones de DbContext
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;


            // Crear el esquema en la base de datos en memoria
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task MoneyBoxes_GetGlobalAmount_ReturnsCorrectValue()
        {
            //arrange
            using (var context = new AppDbContext(_contextOptions))
            {
                var moneybox = new Moneyboxes { Name = "Caja 1", Description = "Descripción de la caja 1", Id = 1, percentage = 10, value = 100 };
                context.Moneyboxes.Add(moneybox);
                await context.SaveChangesAsync();
            }

            Decimal value = 0;
            //act and assert
            using (var context = new AppDbContext(_contextOptions))
            {

                value = await Moneyboxes.GetGlobalAmount(context);
                context.Database.EnsureDeleted();

            }
            Assert.NotNull(value);
            _connection.Dispose();
            _connection.Close();
        }
        [Fact]
        public async Task Moneyboxes_GetMoneyboxes_ReturnsList()
        {
            //arrange
            using (var context = new AppDbContext(_contextOptions))
            {
                var moneybox = new Moneyboxes { Name = "Caja 1", Description = "Descripción de la caja 1", Id = 1, percentage = 10, value = 100 };
                context.Moneyboxes.Add(moneybox);
                await context.SaveChangesAsync();
            }

            List<Moneyboxes> data = new List<Moneyboxes>();
            //act and assert
            using (var context = new AppDbContext(_contextOptions))
            {
                data = await Moneyboxes.GetMoneyboxes(context);
                context.Database.EnsureDeleted();
            }
            Assert.NotNull(data);
            Assert.NotEmpty(data);


            _connection.Dispose();
            _connection.Close();

        }


    }
}

