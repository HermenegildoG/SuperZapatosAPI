using DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperZapatosAPI.Dtos;
using System.Reflection.Metadata.Ecma335;

namespace SuperZapatosAPI.Controllers
{
    [Route("services/[controller]")]
    [ApiController]
    public class articlesController : ControllerBase
    {

        private SuperZapatosContext _context;

        public articlesController(SuperZapatosContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetArticles()
        {
            var articles = _context.Articles
        .Include(article => article.Stores)
        .ToList();

            var result = new
            {
                articles = articles.Select(article => new
                {
                    article.id,
                    article.name,
                    article.description,
                    article.price,
                    article.total_in_shelf,
                    article.total_in_vault,
                    article.store_id,
                    store_name = article.Stores?.name
                }).ToList(),
                success = true,
                total_elements = articles.Count
            };

            return Ok(result);
        }

        [HttpGet("/services/articles/stores/{storeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetArticlesById(string storeId)
        {
            try
            {
                if (!int.TryParse(storeId, out int parsedStoreId))
                {
                    return BadRequest(new
                    {
                        error_msg = "Bad Request",
                        error_code = 400,
                        success = false
                    });
                }

                var articles = _context.Articles
                    .Where(article => article.store_id == parsedStoreId)
                    .Include(article => article.Stores)
                    .ToList();

                if (articles.Count == 0)
                {
                    return NotFound(new
                    {
                        error_msg = "Not Found",
                        error_code = 404,
                        success = false
                    });
                }

                var result = new
                {
                    articles = articles.Select(article => new
                    {
                        article.id,
                        article.name,
                        article.description,
                        article.price,
                        article.total_in_shelf,
                        article.total_in_vault,
                        article.store_id,
                        store_name = article.Stores?.name
                    }),
                    success = true,
                    total_elements = articles.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error_msg = "Internal Server Error",
                    error_code = 500,
                    success = false
                });
            }
        }

        [HttpPost("/services/articles/createarticle")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateArticle([FromBody] CreateArticleDto article)
        {
            if (article == null)
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request",
                    error_code = 400,
                    success = false
                });
            }
            var newArticle = new Articles
            {
                // Asigna propiedades de la nueva tienda basadas en el modelo de entrada
                name = article.name,
                description = article.description, 
                price = article.price,
                total_in_shelf = article.total_in_shelf,
                total_in_vault = article.total_in_vault,
                store_id = article.store_id,
            };

            _context.Articles.Add(newArticle);
            _context.SaveChanges();

            var result = new
            {
                store = newArticle,
                success = true
            };

            return CreatedAtAction(nameof(GetArticles), result);
        }

        [HttpDelete("/services/articles/deletearticle/{articleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteArticle(string articleId)
        {
            if (!int.TryParse(articleId, out int parsedArticleId))
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: El id debe ser un número entero.",
                    error_code = 400,
                    success = false
                });
            }
            var articleToDelete = _context.Articles.Find(parsedArticleId);

            if (articleToDelete == null)
            {
                return NotFound(new
                {
                    error_msg = "Not Found",
                    error_code = 404,
                    success = false
                });
            }
            _context.Articles.Remove(articleToDelete);
            _context.SaveChanges();

            var result = new
            {
                success = true
            };

            return Ok(result);
        }

        [HttpPut("/services/articles/updatearticle/{articleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateArticle(string articleId, [FromBody] CreateArticleDto articleUpdate)
        {
            if (!int.TryParse(articleId, out int parsedArticleId))
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: El id debe ser un número entero.",
                    error_code = 400,
                    success = false
                });
            }

            // Validar si existe el artículo a actualizar
            var existingArticle = _context.Articles.Find(parsedArticleId);

            if (existingArticle == null)
            {
                return NotFound(new
                {
                    error_msg = "Not Found: El artículo a actualizar no existe.",
                    error_code = 404,
                    success = false
                });
            }

            // Validar si existe el store_id proporcionado
            var existingStore = _context.Stores.Find(articleUpdate.store_id);

            if (existingStore == null)
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: El store_id proporcionado no es válido.",
                    error_code = 400,
                    success = false
                });
            }

            // Validación básica del modelo de actualización
            if (articleUpdate == null || string.IsNullOrEmpty(articleUpdate.name) || string.IsNullOrEmpty(articleUpdate.description))
            {
                return BadRequest(new
                {
                    error_msg = "Bad Request: Se requieren datos válidos para la actualización.",
                    error_code = 400,
                    success = false
                });
            }

            // Actualizar el artículo con los nuevos valores
            existingArticle.name = articleUpdate.name;
            existingArticle.description = articleUpdate.description;
            existingArticle.price = articleUpdate.price;
            existingArticle.total_in_shelf = articleUpdate.total_in_shelf;
            existingArticle.total_in_vault = articleUpdate.total_in_vault;
            existingArticle.store_id = articleUpdate.store_id;

            _context.SaveChanges();

            var result = new
            {
                success = true
            };

            return Ok(result);
        }



    }
}
