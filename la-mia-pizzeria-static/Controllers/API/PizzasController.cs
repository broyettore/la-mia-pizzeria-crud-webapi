using Humanizer;
using la_mia_pizzeria_static.Database;
using la_mia_pizzeria_static.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace la_mia_pizzeria_static.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {

        private readonly PizzaContext _myDb;

        public PizzasController(PizzaContext myDb)
        {
            _myDb = myDb;
        }

        [HttpGet]
        public IActionResult GetPizzas()
        {
            List<Pizza> pizzas = _myDb.Pizzas.Include(p => p.Category)
                .Include(p => p.Ingredients)
                .ToList();

            return Ok(pizzas);
        }

        [HttpGet]
        public IActionResult GetPizzasByName(string? name)
        {
            if (name == null)
            {
                return BadRequest(new { Message = "No parameter inserted" });
            }

            List<Pizza> foundPizzas = _myDb.Pizzas.Where(p => p.Name.ToLower().Contains(name.ToLower())).ToList();

            return Ok(foundPizzas);
        }

        [HttpGet("{id}")]
        public IActionResult GetPizzaById(int id)
        {
            Pizza? foundPizza = _myDb.Pizzas.Where(p => p.PizzaId == id).Include(p => p.Category)
                .Include(p => p.Ingredients).FirstOrDefault();

            if (foundPizza != null)
            {
                return Ok(foundPizza);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Pizza newPizza)
        {
            try
            {
                _myDb.Pizzas.Add(newPizza);
                _myDb.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Modify(int id, [FromBody] Pizza updatedPizza)
        {
            Pizza? pizzaToUpdate = _myDb.Pizzas.Where(p => p.PizzaId == id).FirstOrDefault();

            if(pizzaToUpdate != null) 
            {
                pizzaToUpdate.Name = updatedPizza.Name;
                pizzaToUpdate.ImgPath = updatedPizza.ImgPath;
                pizzaToUpdate.Price = updatedPizza.Price;
                pizzaToUpdate.Description = updatedPizza.Description;
                pizzaToUpdate.CategoryId = updatedPizza.CategoryId;

                _myDb.SaveChanges();

                return Ok();
            } else
            {
                return NotFound();
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Pizza? pizzaToDelete = _myDb.Pizzas.Where(p => p.PizzaId == id).FirstOrDefault();

            if(pizzaToDelete == null) 
            {
                return NotFound();
            } else
            {
                _myDb.Pizzas.Remove(pizzaToDelete);
                _myDb.SaveChanges();
                return Ok();
            }
        }
    }
}
