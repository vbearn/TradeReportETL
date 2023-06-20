using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Pipeline.Modules.Transform.Models;
using TradeReportETL.Pipeline.Modules.Transform.Services;
using TradeReportETL.Pipeline.Modules.Transform.Services.ApiClient;
using TradeReportETL.Shared.Models;
using Xunit;

namespace TradeReportETL.Pipeline.UnitTests
{
    public sealed class TransactionTransformServiceTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<TransactionTransformService>> _loggerStub;
        private readonly CancellationToken _cancellationToken;

        public TransactionTransformServiceTests()
        {
            _fixture = new Fixture();

            _cancellationToken = _fixture.Build<CancellationToken>().Create();

            _loggerStub = new Mock<ILogger<TransactionTransformService>>();
        }

        [Fact]
        public async Task TransformBatchTransactions_WithOneTransaction_ShouldEnrichCorrectly()
        {

            // Arrange

            var leiModel = _fixture.Build<LeiModel>().Create();
            ICollection<LeiModel> leiModels = new List<LeiModel> { leiModel };

            var tradeReportTransactionModel = _fixture.Build<TradeReportTransactionModel>()
                .With(t => t.Lei, leiModel.Attributes.Lei)
                .With(t => t.LegalName, leiModel.Attributes.Entity.LegalName.Name)
                .With(t => t.Bic , leiModel.Attributes.Bic.First())
                .Create();
            var tradeReportTransactionModels = new List<TradeReportTransactionModel> { tradeReportTransactionModel };

            var gleifApiClientStub = new Mock<IGleifApiClient>();
            gleifApiClientStub
                .Setup(m => m.GetLeiRecordsBatchAsync(new List<string> { tradeReportTransactionModel.Lei }))
                .Returns(Task.FromResult(leiModels));


            var transactionCost = _fixture.Build<decimal>().Create();

            var transactionCostApiClientStub = new Mock<ITransactionCostApiClient>();
            transactionCostApiClientStub
                .Setup(m => m.CalculateTransactionCost(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(Task.FromResult(transactionCost));

            var _sut = new TransactionTransformService(
                gleifApiClientStub.Object,
                _loggerStub.Object,
                transactionCostApiClientStub.Object
                );




            // Act

            var result = await _sut.TransformBatchTransactions(tradeReportTransactionModels, _cancellationToken);

            // Assert

            result.Should().AllBeOfType<TradeReportTransactionModel>();
            result.Should().HaveCount(1);
            result.First().LegalName.Should().Be(leiModel.Attributes.Entity.LegalName.Name);
            result.First().Bic.Should().Be(leiModel.Attributes.Bic.First());
        }

    }


}
