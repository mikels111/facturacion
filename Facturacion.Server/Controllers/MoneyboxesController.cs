using Facturacion.Server.Data;
using Facturacion.Server.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using static Facturacion.Server.Models.History;

namespace Facturacion.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoneyboxesController : ControllerBase
    {
        private readonly AppDbContext _AppDbContext;
        public MoneyboxesController(AppDbContext appDbContext)
        {
            _AppDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<List<object>> Get()
        {
            List<Moneyboxes> data = new List<Moneyboxes>();
            var formatMoneyBoxes = Enumerable.Empty<object>();
            try
            {
                data = await Moneyboxes.GetMoneyboxes(_AppDbContext);

                formatMoneyBoxes = data.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    ValueString = p.value.ToString("F2", new CultureInfo("es-ES"))
                }).ToList();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return [.. formatMoneyBoxes];
            //return Ok(data);
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Moneyboxes> patch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var moneybox = await _AppDbContext.Moneyboxes.FirstOrDefaultAsync(m => m.Id == id);
                if (moneybox == null)
                    return NotFound();
                patch.ApplyTo(moneybox);
                _AppDbContext.Update(moneybox);
                var result = _AppDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok();
        }
        [Route("Deposit")]
        [HttpPost]
        public async Task<IActionResult> Deposit([FromBody] History.HistoryDTO depositData)
        {
            //por hacer --> que reciba el array como parametro para modificar solo los que se le indiquen
            int result = 0;
            try
            {
                if (depositData.Amount <= 0)
                    return BadRequest();
                History history = new History
                {
                    HistoryTypeId = 1,
                    Moneybox = "Global",
                    Amount = depositData.Amount,
                    Description = depositData.Description ?? "Ingreso",
                    Date = DateTime.Now,
                    GlobalPreviousAmount = await Moneyboxes.GetGlobalAmount(_AppDbContext),
                    MoneyBoxId = 0

                };
                //decimal porcentajeNecesidades = Math.Round(50 * depositData.Amount / 100, 2);
                //decimal porcentajeResto = Math.Round(10 * depositData.Amount / 100, 2);

                //var ids = new[] { 1, 2, 3, 4, 5, 6 };
                //var moneyboxes = await _AppDbContext.Moneyboxes.Where(mb => ids.Contains(mb.Id)).ToListAsync();
                var moneyboxes = await _AppDbContext.Moneyboxes.ToListAsync();
                foreach (var mb in moneyboxes)
                {
                    mb.value += Math.Round(mb.percentage * depositData.Amount / 100, 2);
                }


                result = await _AppDbContext.SaveChangesAsync();
                if (result > 0)
                {
                    await History.CreateHistory(_AppDbContext, history);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(result);
        }
        [Route("SingleDeposit/{moneyboxId:int:min(1)}")]
        [HttpPost]
        public async Task<IActionResult> SingleDeposit([FromRoute] int moneyboxId, [FromBody] History.HistoryDTO depositData)
        {
            //por hacer --> que reciba el array como parametro para modificar solo los que se le indiquen
            int result = 0;
            try
            {
                //if (amount <= 0)
                //    return BadRequest();
                var moneybox = await _AppDbContext.Moneyboxes.Where(mb => mb.Id == moneyboxId).FirstOrDefaultAsync();
                History history = new History
                {
                    HistoryTypeId = 1,
                    Amount = depositData.Amount,
                    Moneybox = moneybox?.Name,
                    Description = depositData.Description ?? "",
                    Date = DateTime.Now,
                    MoneyBoxPreviousAmount = moneybox?.value ?? 0,
                    GlobalPreviousAmount = await Moneyboxes.GetGlobalAmount(_AppDbContext),
                    MoneyBoxId= moneyboxId

                };
                if (moneybox == null)
                    return NotFound();
                decimal total = Math.Round(moneybox.value + depositData.Amount, 2);
                //if (total <= 0)
                //    return BadRequest();
                moneybox.value = total;
                result = await _AppDbContext.SaveChangesAsync();
                if (result > 0)
                {
                    await History.CreateHistory(_AppDbContext, history);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(result);
        }
        [HttpPost("Withdraw/{moneyboxId:int:min(1)}")]
        public async Task<IActionResult> Withdraw([FromRoute] int moneyboxId, [FromBody] History.HistoryDTO depositData)
        {
            int result = 0;
            try
            {
                //if (amount <= 0)
                //    return BadRequest();
                var moneybox = await _AppDbContext.Moneyboxes.Where(mb => mb.Id == moneyboxId).FirstOrDefaultAsync();
                History history = new History
                {
                    HistoryTypeId = 2,
                    Amount = depositData.Amount,
                    Moneybox = moneybox?.Name,
                    Description = depositData.Description ?? "",
                    Date = DateTime.Now,
                    MoneyBoxPreviousAmount = moneybox?.value ?? 0,
                    GlobalPreviousAmount = await Moneyboxes.GetGlobalAmount(_AppDbContext),
                    MoneyBoxId = moneyboxId

                };
                if (moneybox == null)
                    return NotFound();
                decimal total = Math.Round(moneybox.value - depositData.Amount, 2);
                //if (total < 0)
                //    return BadRequest();
                moneybox.value = total;
                result = await _AppDbContext.SaveChangesAsync();
                if (result > 0)
                {
                    await History.CreateHistory(_AppDbContext, history);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(result);
        }

        [Route("GetGlobalAmount")]
        [HttpGet]
        public async Task<IActionResult> GetGlobalAmount()
        {
            decimal result = 0;
            string precioConComa = "";
            try
            {
                result = await Facturacion.Server.Models.Moneyboxes.GetGlobalAmount(_AppDbContext);
                precioConComa = result.ToString("F2", new CultureInfo("es-ES"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(precioConComa);
        }

        [Route("GetHistory")]
        [HttpGet]
        public async Task<List<object>> GetHistory()
        {
            List<History> data = new List<History>();
            var formatHistory = Enumerable.Empty<object>();
            try
            {
                data = await _AppDbContext.History.ToListAsync();
                formatHistory = data.Select(p => new
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

            return [.. formatHistory];
        }

        [Route("DepositTest")]
        [HttpPost]
        public async Task<IActionResult> DepositTest([FromBody] History.HistoryDTO depositData)
        {
            //por hacer --> que reciba el array como parametro para modificar solo los que se le indiquen
            int result = 0;
            try
            {
                if (depositData.Amount <= 0)
                    return BadRequest();

                decimal porcentajeNecesidades = Math.Round(50 * depositData.Amount / 100, 2);
                decimal porcentajeResto = Math.Round(10 * depositData.Amount / 100, 2);

                var ids = new[] { 1, 2, 3, 4, 5, 6 };
                //var moneyboxes = await _AppDbContext.Moneyboxes.Where(mb => ids.Contains(mb.Id)).ToListAsync();
                //foreach (var mb in moneyboxes)
                //{
                //    if (mb.Id == 2)
                //        mb.value += porcentajeNecesidades;
                //    else
                //        mb.value += porcentajeResto;
                //}
                var moneyboxes = await _AppDbContext.Moneyboxes.ToListAsync();
                foreach (var mb in moneyboxes)
                {
                    //Console.WriteLine($"Moneybox ID: {mb.Id}, Name: {mb.Name}, Value: {mb.value}, percentage: {mb.percentage}");
                    mb.value += Math.Round(mb.percentage * depositData.Amount / 100, 2);
                    Console.WriteLine($"Moneybox ID: {mb.Id}, Name: {mb.Name}, Value: {mb.value}, percentage: {mb.percentage}");

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(result);
        }


        [Route("GetHistoryForEachBox")]
        [HttpGet]
        public async Task<IActionResult> GetHistoryForEachBox()
        {
            //por hacer --> que reciba el array como parametro para modificar solo los que se le indiquen
            int result = 0;
            List<HistoryChartDTO> historyChart = new List<HistoryChartDTO>(4);
            List<HistoryDTO> historyData1 = new List<HistoryDTO>();
            try
            {
                historyChart.AddRange(new List<HistoryChartDTO>()
                {
                    new HistoryChartDTO(){
                        Name = DateOnly.FromDateTime(DateTime.UtcNow).ToString() +" - "+ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)).ToString(),
                        DateStart = DateTime.UtcNow.AddDays(-7),
                        DateEnd = DateTime.UtcNow
                    },
                    new HistoryChartDTO()
                    {
                        Name = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)).ToString() + " - "+DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)).ToString(),
                        DateStart = DateTime.UtcNow.AddDays(-14),
                        DateEnd = DateTime.UtcNow.AddDays(-7)
                    },
                    new HistoryChartDTO()
                    {
                        Name = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)).ToString() + " - "+ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-21)).ToString(),
                        DateStart = DateTime.UtcNow.AddDays(-21),
                        DateEnd = DateTime.UtcNow.AddDays(-14)
                    },
                    new HistoryChartDTO()
                    {
                        Name = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-21)).ToString() + " - " + DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-28)).ToString(),
                        DateStart = DateTime.UtcNow.AddDays(-28),
                        DateEnd = DateTime.UtcNow.AddDays(-21)
                    }
                });
                foreach (var item in historyChart)
                {
                    historyData1= await History.GetHistoryForEachBox(_AppDbContext, item.DateStart, item.DateEnd);
                    item.Inversion = (decimal)historyData1.Where(h => h.Description == "1").Select(h => h.Amount).FirstOrDefault();
                    item.GastosBasicos =(decimal) historyData1.Where(h => h.Description == "2").Select(h => h.Amount).FirstOrDefault();
                    item.Ocio = (decimal)historyData1.Where(h => h.Description == "3").Select(h => h.Amount).FirstOrDefault();
                    item.GastosGrandes = (decimal)historyData1.Where(h => h.Description == "4").Select(h => h.Amount).FirstOrDefault();
                    item.Donacion = (decimal)historyData1.Where(h => h.Description == "5").Select(h => h.Amount).FirstOrDefault();
                    
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(historyChart);
        }


    }
}
