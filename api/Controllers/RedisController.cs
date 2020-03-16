using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

using API.Models;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {

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
        public async Task<IActionResult> Get()
        {
            var item1 = new RedisItem1
            {
                RedisType = "RedisItem1",
                Test1 = "testing 1"
            };
            var item2 = new RedisItem2
            {
                RedisType = "RedisItem2",
                Test2 = "testing 2"
            };
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await database.StringSetAsync((now - 100).ToString(), JsonSerializer.Serialize(item1));
            await database.StringSetAsync((now - 200).ToString(), JsonSerializer.Serialize(item2));
            var endpoints = _redis.GetEndPoints();
            foreach(var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                //await server.FlushDatabaseAsync(); // must set ",allowAdmin=true" in ConnectionMultiplexer
                var keys = server.Keys(pattern: "*");
                foreach (var key in keys)
                {
                    var value = await database.StringGetAsync(key);
                    Debug.WriteLine($"{key}={value}");
                }
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetByKey")]
        public async Task<IActionResult> GetByKey(string id)
        {
            var value = await database.StringGetAsync(id);
            if (value.Length() <= 0) return NotFound();
            var redisType = new ResolveRedisType(value);
            Debug.WriteLine($"redisType={redisType.RedisType}");
            return Ok(value);
        }

    }

}