using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Genealogy.Models;
using Genealogy.Data;
using System.Linq;
using Newtonsoft.Json;
using Yandex.Checkout.V3;
using System;

public class PurchaseManageService : BackgroundService
{
    private readonly ILogger<PurchaseManageService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly GenealogyContext _genealogyContext;
    private Client client = new Yandex.Checkout.V3.Client(shopId: "739084", secretKey: "test_5LLiubI6pAXxCt-13sfn9WymESZgeE9Z30BrZIB3BAQ");

    private readonly int timeout = 10;

    public PurchaseManageService(ILogger<PurchaseManageService> logger, IServiceProvider serviceProvider, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = scopeFactory;
        _genealogyContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<GenealogyContext>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
            _logger.LogDebug($" PurchaseManageService background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            AsyncClient asyncClient = client.MakeAsync();
            _logger.LogDebug($"PurchaseManageService task doing background work.");

            BusinessObjectFilter filter = new BusinessObjectFilter()
            {
                MetatypeId = MetatypeData.Purchase.Id
            };

            var purchases = _genealogyContext.BusinessObjects.Where(bo => bo.MetatypeId == MetatypeData.Purchase.Id).ToList();
            var list = purchases;

            foreach (var purchase in list)
            {
                var purchaseProps = JsonConvert.DeserializeObject<CustomProps.Purchase>(purchase.Data);

                if (purchaseProps.status == PurchaseStatus.Succeeded)
                {
                    continue;
                }

                var response = await asyncClient.GetPaymentAsync(purchaseProps.paymentId);

                switch (purchaseProps.status)
                {
                    case PurchaseStatus.Pending:
                        var time1 = DateTime.Now;
                        var time2 = purchase.StartDate;
                        var span = time1.Subtract(time2).TotalMinutes;

                        if (DateTime.Now.Subtract(purchase.StartDate).TotalMinutes > timeout)
                        {
                            await removePurchase(purchase, @"Timeout {timeout} minutes.");
                        }
                        break;
                }

                switch (response.Status)
                {
                    case PaymentStatus.Succeeded:
                        purchaseProps.status = PurchaseStatus.Succeeded;
                        purchase.Data = JsonConvert.SerializeObject(purchaseProps);

                        await updatePurchase(purchase, "Payment successed.");
                        await productAction(Guid.Parse(purchaseProps.productId), purchase.UserId);

                        break;

                    case PaymentStatus.Canceled:
                        await removePurchase(purchase, "Canceled.");
                        break;
                }

                _logger.LogDebug($"{purchase.Id} {purchase.Title}");
            };

            await Task.Delay(5000, stoppingToken);
        }

        _logger.LogDebug($"PurchaseManageService background task is stopping.");
    }

    private async Task<int> removePurchase(BusinessObject purchase, string reason)
    {
        _logger.LogDebug($"PurchaseManageService removes purchase {purchase.Id}. Reason: {reason}");
        try
        {
            _genealogyContext.BusinessObjects.Remove(purchase);
            return await _genealogyContext.SaveChangesAsync();
        }
        catch (ApplicationException e)
        {
            _logger.LogError(e.ToString());
            throw e;
        }
    }
    private async Task<int> updatePurchase(BusinessObject purchase, string reason)
    {
        _logger.LogDebug($"PurchaseManageService updates purchase {purchase.Id}. Reason: {reason}");
        try
        {
            _genealogyContext.BusinessObjects.Update(purchase);
            return await _genealogyContext.SaveChangesAsync();
        }
        catch (ApplicationException e)
        {
            _logger.LogError(e.ToString());
            throw e;
        }
    }

    private async Task<int> productAction(Guid productId, Guid userId)
    {
        if (productId == ProductData.Subscribe.Id)
        {
            var subscribeMetatype = _genealogyContext.Metatypes.Where(metatype => metatype.Id == MetatypeData.Subscribe.Id).FirstOrDefault();
            var subscribe = new BusinessObject()
            {
                Id = Guid.NewGuid(),
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now.AddMonths(1),
                UserId = userId,
                MetatypeId = ProductData.Subscribe.Id,
                Metatype = subscribeMetatype,
                IsRemoved = false,
                Name = "SUBSCRIBLE",
                Title = "Подписка"
            };
            try
            {
                _genealogyContext.BusinessObjects.Add(subscribe);
                return await _genealogyContext.SaveChangesAsync();
            }
            catch (ApplicationException e)
            {
                _logger.LogError(e.ToString());
                throw e;
            }
        }

        throw new ApplicationException();
    }
}