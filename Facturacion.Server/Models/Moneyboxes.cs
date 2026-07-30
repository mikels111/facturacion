using Facturacion.Server.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Facturacion.Server.Models
{
    public class Moneyboxes 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal value { get; set; }
        public int percentage { get; set; }
        public static async Task<decimal> GetGlobalAmount(AppDbContext appDbContext)
        {
            decimal result = 0;
            try
            {
                var moneyboxes = await appDbContext.Moneyboxes.ToListAsync();

                foreach (var mb in moneyboxes)
                {
                    result += mb.value;

                }
                result = Math.Round(result, 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }
        public static async Task<List<Moneyboxes>> GetMoneyboxes(AppDbContext appDbContext)
        {
            List<Moneyboxes> data = new List<Moneyboxes>();
            try
            {
                data = await appDbContext.Moneyboxes.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return data;
        }
    }
}


