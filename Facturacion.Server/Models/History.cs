using Facturacion.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Facturacion.Server.Models
{
    public class History
    {
        [Key]
        public int IdHistory { get; set; }
        public int HistoryTypeId { get; set; }//1=Ingreso 2=retiro
        public string Moneybox { get; set; }//Inversion,Necesidades,Ocio, Gastos Grandes, Donacion
        public int? MoneyBoxId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public decimal MoneyBoxPreviousAmount { get; set; } = 0;
        public decimal GlobalPreviousAmount { get; set; } = 0;

        public record struct HistoryDTO(decimal Amount, string? Description);
        //public record struct HistoryChartDTO(string Name, DateTime DateStart, DateTime DateEnd, decimal Inversion = 0, decimal GastosBasicos = 0, decimal Ocio = 0, decimal GastosGrandes = 0, decimal Donacion = 0);
        public class HistoryChartDTO
        {
            public string Name { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public decimal Inversion { get; set; } = 0;
            public decimal GastosBasicos { get; set; } = 0;
            public decimal Ocio { get; set; } = 0;
            public decimal GastosGrandes { get; set; } = 0;
            public decimal Donacion { get; set; } = 0;
        }
        public static async Task<int> CreateHistory(AppDbContext appDbContext, History history)
        {
            int result = 0;
            try
            {
                await appDbContext.History.AddAsync(history);
                result = await appDbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving interactions: {ex.Message}");
            }
            return result;
        }
        public static async Task<List<History>> GetHistory(AppDbContext appDbContext)
        {
            List<History> data = new List<History>();
            try
            {
                data = await appDbContext.History.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return data;
        }
        public static async Task<List<HistoryDTO>> GetHistoryForEachBox(AppDbContext appDbContext, DateTime dateStart, DateTime dateEnd)
        {
            //DateTime startDate, DateTime endDate
            List<History> data = new List<History>();
            List<HistoryDTO> resumenSemanal = new List<HistoryDTO>();
            try
            {
                //data = await appDbContext.History.ToListAsync();s
                resumenSemanal = await appDbContext.History
                .Where(h => h.Date >= dateStart && h.Date <= dateEnd)
                .Where(h => h.MoneyBoxId > 0)
                .GroupBy(h => h.MoneyBoxId)
                .Select(g => new HistoryDTO
                {
                    // Ordenamos el grupo por fecha y tomamos el 'MoneyBoxPreviousAmount' del registro más nuevo
                    Amount = g.OrderByDescending(x => x.Date)
                              .Select(x => x.Amount).Sum(),

                    // El ID del grupo es el MoneyBoxId, así que lo usamos directamente
                    Description = g.Key.ToString()
                }).ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return resumenSemanal;
        }

    }
}
