using Facturacion.Server.Data;
using Facturacion.Server.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Facturacion.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoneyboxesController : ControllerBase
    {
        private readonly AppDbContext _AppDbContext = new AppDbContext();

        [HttpGet]
        public async Task<List<Facturacion.Server.Models.Moneyboxes>> Get()
        {
            List<Facturacion.Server.Models.Moneyboxes> data = new List<Facturacion.Server.Models.Moneyboxes>();
            try
            {
                data = await _AppDbContext.Moneyboxes.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return data;
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
        public async Task<IActionResult> Deposit([FromBody] float deposit)
        {
            //por hacer --> que reciba el array como parametro para modificar solo los que se le indiquen
            int result = 0;
            try
            {
                if (deposit <= 0)
                    return BadRequest();
                float porcentajeNecesidades = (float)Math.Round(50 * deposit / 100, 2);
                float porcentajeResto = (float)Math.Round(10 * deposit / 100, 2);

                var ids = new[] { 1, 2, 3, 4, 5, 6 };
                var moneyboxes = await _AppDbContext.Moneyboxes.Where(mb => ids.Contains(mb.Id)).ToListAsync();
                foreach (var mb in moneyboxes)
                {
                    if (mb.Id == 2)
                        mb.value += porcentajeNecesidades;
                    else
                        mb.value += porcentajeResto;
                }
                result = await _AppDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(result);
        }
        [Route("SingleDeposit/{moneyboxId:int:min(1)}")]
        [HttpPost]
        public async Task<IActionResult> SingleDeposit([FromRoute] int moneyboxId,[FromBody] float amount)
        {
            //por hacer --> que reciba el array como parametro para modificar solo los que se le indiquen
            int result = 0;
            try
            {
                //if (amount <= 0)
                //    return BadRequest();
                var moneybox = await _AppDbContext.Moneyboxes.Where(mb => mb.Id == moneyboxId).FirstOrDefaultAsync();
                if (moneybox == null)
                    return NotFound();
                float total = (float)Math.Round(moneybox.value + amount, 2);
                //if (total <= 0)
                //    return BadRequest();
                moneybox.value = total;
                result = await _AppDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(result);
        }
        [HttpPost("Withdraw/{moneyboxId:int:min(1)}")]
        public async Task<IActionResult> Withdraw([FromRoute] int moneyboxId, [FromBody] float amount)
        {
            int result = 0;
            try
            {
                //if (amount <= 0)
                //    return BadRequest();
                var moneybox = await _AppDbContext.Moneyboxes.Where(mb => mb.Id == moneyboxId).FirstOrDefaultAsync();
                if (moneybox == null)
                    return NotFound();
                float total = (float)Math.Round(moneybox.value - amount, 2);
                //if (total < 0)
                //    return BadRequest();
                moneybox.value = total;
                result = await _AppDbContext.SaveChangesAsync();
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
            float result = 0;
            try
            {
                var moneyboxes = await _AppDbContext.Moneyboxes.ToListAsync();
                foreach (var mb in moneyboxes)
                {
                    result += (float)mb.value;
                    
                }
                result = (float)Math.Round(result, 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok(result);
        }

    }
}
