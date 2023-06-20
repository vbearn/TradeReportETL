using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Shared.Events;
using TradeReportETL.Shared.Models;

namespace TradeReportETL.Shared.Services
{
    public class RedisLockService
    {
        private readonly RedLockFactory _redlockFactory;
        public RedLockFactory RedlockFactory => _redlockFactory;

        public RedisLockService(IConfiguration configuration)
        {
            var endPoints = new List<RedLockEndPoint>
            {
                new DnsEndPoint(configuration.GetValue<string>("Redis:Host"), configuration.GetValue<int>("Redis:Port")),
            };

            _redlockFactory = RedLockFactory.Create(endPoints);
        }


    }
}