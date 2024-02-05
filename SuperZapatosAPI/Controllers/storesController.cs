using DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperZapatosAPI.Dtos;

namespace SuperZapatosAPI.Controllers
{
    [Route("services/[controller]")]
    [ApiController]
    public class storesController : ControllerBase
    {
        private SuperZapatosContext _context;

        public storesController(SuperZapatosContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetStores()
        {
            var stores = _context.Stores.ToList();

            var result = new
            {
                stores = stores,
                success = true,
                total_elements = stores.Count
            };

            return Ok(result);
        }

        [HttpPost("/services/stores/createstore")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateStore([FromBody] CreateStoreDto store)
        {
            if (store == null)
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request",
                    error_code = 400,
                    success = false
                });
            }


            var newStore = new Stores
            {
                name = store.name,
                adress = store.adress
            };

            _context.Stores.Add(newStore);
            _context.SaveChanges();

            var result = new
            {
                store = newStore,
                success = true
            };

            return CreatedAtAction(nameof(GetStores), result);
        }

        [HttpDelete("/services/stores/deletestore/{storeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteStore(string storeId)
        {
            if (!int.TryParse(storeId, out int parsedStoreId))
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: El id debe ser un número entero.",
                    error_code = 400,
                    success = false
                });
            }

            var storeToDelete = _context.Stores.Find(parsedStoreId);

            if (storeToDelete == null)
            {
                return NotFound(new
                {
                    error_msg = "Not Found",
                    error_code = 404,
                    success = false
                });
            }

            // Verificar si hay artículos asociados a la tienda
            var articlesInStore = _context.Articles.Any(article => article.store_id == parsedStoreId);

            if (articlesInStore)
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: No se puede eliminar la tienda porque tiene artículos asociados.",
                    error_code = 400,
                    success = false
                });
            }

            _context.Stores.Remove(storeToDelete);
            _context.SaveChanges();

            var result = new
            {
                success = true
            };

            return Ok(result);
        }

        [HttpPut("/services/stores/updatestore/{storeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateStore(string storeId, [FromBody] CreateStoreDto storeUpdate)
        {
            if (!int.TryParse(storeId, out int parsedStoreId))
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: El id debe ser un número entero.",
                    error_code = 400,
                    success = false
                });
            }

            var existingStore = _context.Stores.Find(parsedStoreId);

            if (existingStore == null)
            {
                return NotFound(new
                {
                    error_msg = "Not Found",
                    error_code = 404,
                    success = false
                });
            }

            // Validación básica del modelo de actualización
            if (storeUpdate == null || string.IsNullOrEmpty(storeUpdate.name) || string.IsNullOrEmpty(storeUpdate.adress))
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: Se requieren datos válidos para la actualización.",
                    error_code = 400,
                    success = false
                });
            }

            // Actualizar las propiedades de la tienda existente con los datos del modelo de actualización
            existingStore.name = storeUpdate.name;
            existingStore.adress = storeUpdate.adress;

            _context.SaveChanges();

            var result = new
            {
                success = true
            };

            return Ok(result);
        }




    }
}
