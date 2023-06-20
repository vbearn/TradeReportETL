//using Microsoft.Extensions.Options;
//using MongoDB.Driver;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using TradeReportETL.Shared.Events;
//using TradeReportETL.Shared.Models;

//namespace TradeReportETL.Shared.Services
//{
//    public class TradeReportDatabaseService : ITradeReportDatabaseService
//    {
//        private readonly IMongoCollection<TradeReportModel> _tradeReports;
//        private readonly IMongoCollection<TradeReportTransactionModel> _tradeReportTransactions;

//        public TradeReportDatabaseService(IOptions<TradeReportDatabaseSettings> settings)
//        {
//            var mongoClient = new MongoClient(settings.Value.ConnectionString);

//            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);

//            _tradeReports = database.GetCollection<TradeReportModel>(settings.Value.TradeReportsCollectionName);
//            _tradeReportTransactions = database.GetCollection<TradeReportTransactionModel>(settings.Value.TradeReportTransactionsCollectionName);
//        }

//        public TradeReportModel CreateTradeReport(TradeReportModel tradeReport)
//        {
//            _tradeReports.InsertOne(tradeReport);
//            return tradeReport;
//        }

//        public TradeReportModel GetTradeReport(string id)
//        {
//            return _tradeReports.Find(x => x.Id == id).FirstOrDefault();
//        }

//        public void RemoveTradeReport(string id)
//        {
//            _tradeReports.DeleteOne(x => x.Id == id);
//        }

//        public void UpdateTradeReport(string id, TradeReportModel tradeReport)
//        {
//            _tradeReports.ReplaceOne(x => x.Id == id, tradeReport);
//        }


//        public TradeReportTransactionModel CreateTradeReportTransaction(TradeReportTransactionModel tradeReportTransaction)
//        {
//            _tradeReportTransactions.InsertOne(tradeReportTransaction);
//            return tradeReportTransaction;
//        }

//        public TradeReportTransactionModel GetTradeReportTransaction(string id)
//        {
//            return _tradeReportTransactions.Find(x => x.Id == id).FirstOrDefault();
//        }

//        public void RemoveTradeReportTransaction(string id)
//        {
//            _tradeReportTransactions.DeleteOne(x => x.Id == id);
//        }

//        public void UpdateTradeReportTransaction(string id, TradeReportTransactionModel tradeReportTransaction)
//        {
//            _tradeReportTransactions.ReplaceOne(x => x.Id == id, tradeReportTransaction);
//        }


//    }
//}