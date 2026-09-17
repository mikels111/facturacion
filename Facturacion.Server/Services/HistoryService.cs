using Facturacion.Server.Data;
using Facturacion.Server.Models;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Facturacion.Server.Services
{
    public class HistoryService
    {

        public static async Task<List<object>> GetHistory(AppDbContext appDbContext)
        {

            List<History> historyList = new List<History>();
            var formatHistory = Enumerable.Empty<object>();
            try
            {
                historyList = await History.GetHistory(appDbContext);
                formatHistory = historyList.Select(p => new
                {
                    p.IdHistory,
                    p.HistoryTypeId,
                    p.Moneybox,
                    Amount = p.Amount.ToString("F2", new CultureInfo("es-ES")),
                    p.Description,
                    p.Date,
                    MoneyBoxPreviousAmount = p.MoneyBoxPreviousAmount.ToString("F2", new CultureInfo("es-ES")),
                    GlobalPreviousAmount = p.GlobalPreviousAmount.ToString("F2", new CultureInfo("es-ES"))

                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return formatHistory.ToList();



        }
    }
}
