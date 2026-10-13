using Fullstack.LoggingandHealthchecks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Fullstack.LoggingandHealthchecks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        private static readonly ConcurrentDictionary<int, Product> Products =
            new(new Dictionary<int, Product>
            {
                [1] = new Product
                {
                    Id = 1,
                    Name = "Mechanical Keyboard",
                    Price = 2499
                },
                [2] = new Product
                {
                    Id = 2,
                    Name = "Wireless Mouse",
                    Price = 999
                }
            });

        private static int _nextId = 2;

        /// <summary>
        /// Constructer
        /// </summary>
        /// <param name="logger"></param>
        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation(
                "Retrieving all products. RequestId: {RequestId}",
                HttpContext.TraceIdentifier);

            return Ok(Products.Values.OrderBy(p => p.Id));
        }
        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation(
                "Looking up product {ProductId}. RequestId: {RequestId}",
                id,
                HttpContext.TraceIdentifier);

            if (!Products.TryGetValue(id, out var product))
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found. RequestId: {RequestId}",
                    id,
                    HttpContext.TraceIdentifier);

                return NotFound(new
                {
                    message = $"Product {id} was not found."
                });
            }

            return Ok(product);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name) ||
                product.Price <= 0)
            {
                _logger.LogWarning(
                    "Invalid product creation request. RequestId: {RequestId}",
                    HttpContext.TraceIdentifier);

                return BadRequest(new
                {
                    message = "Product name and positive price are required."
                });
            }

            var id = Interlocked.Increment(ref _nextId);

            var created = new Product
            {
                Id = id,
                Name = product.Name,
                Price = product.Price
            };

            Products[id] = created;

            _logger.LogInformation(
                "Product {ProductId} created. Name: {ProductName}, Price: {Price}",
                created.Id,
                created.Name,
                created.Price);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }
        /// <summary>
        /// Simulate an error for testing purposes
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpGet("simulate-error")]
        public IActionResult SimulateError()
        {
            _logger.LogWarning(
                "The simulated error endpoint was requested. RequestId: {RequestId}",
                HttpContext.TraceIdentifier);

            throw new InvalidOperationException(
                "This is an intentional demo exception.");
        }
    }

}
