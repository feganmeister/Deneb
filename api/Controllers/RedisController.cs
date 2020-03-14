using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<RedisController> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase database;

        public RedisController(ILogger<RedisController> logger, ConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            database = _redis.GetDatabase();
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var rng = new Random();
            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray();
            database.StringSet(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), JsonSerializer.Serialize(forecast.First()));
            var endpoints = _redis.GetEndPoints();
            foreach(var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                //await server.FlushDatabaseAsync(); // must set ",allowAdmin=true" in ConnectionMultiplexer
                var keys = server.Keys(pattern: "*");
                foreach (var key in keys)
                {
                    var value = database.StringGet(key);
                    Debug.WriteLine($"{key}={value}");
                }
            }   
            return forecast;
        }

        [HttpGet]
        [Route("GetByKey")]
        public async Task<string> GetByKey(int id)
        {
            var value = database.StringGet(id.ToString());
            return value;
        }

    }

}