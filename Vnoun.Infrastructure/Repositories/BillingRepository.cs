using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Core.Repositories.Dtos.Responses.billings;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class BillingRepository : Repository<Billing>, IBillingRepository
{
    public async Task<AdminStatsResultDto> GetAdminStatsAsync()
    {
        var filter = Builders<Billing>.Filter.Eq(c => c.PaymentStatus, "COMPLETED");

        var billings = await DB.Collection<Billing>().Find(filter).ToListAsync();

        var orders = billings.Count;

        var paid = new Dictionary<string, decimal>();

        foreach (var billing in billings)
        {
            if (paid.ContainsKey(billing.Currency.ToLower()))
            {
                paid[billing.Currency.ToLower()] += billing.Balance;
            }
            else
            {
                paid.Add(billing.Currency.ToLower(), billing.Balance);
            }
        }

        filter = Builders<Billing>.Filter.Eq(c => c.PaymentStatus, "succeeded");

        var billings2 = await DB.Collection<Billing>().Find(filter).ToListAsync();

        orders += billings2.Count;

        foreach (var billing in billings2)
        {
            if (paid.ContainsKey(billing.Currency.ToLower()))
            {
                paid[billing.Currency.ToLower()] += billing.Balance;
            }
            else
            {
                paid.Add(billing.Currency.ToLower(), billing.Balance);
            }
        }

        var todayEarnings = new Dictionary<string, decimal>();

        filter = Builders<Billing>.Filter.Gte(c => c.CreatedAt, DateTime.Now.Date);

        var billings3 = await DB.Collection<Billing>().Find(filter).ToListAsync();
        billings3 = billings3.Where(c => c.PaymentStatus == "succeeded").ToList();

        foreach (var billing in billings3)
        {
            if (todayEarnings.ContainsKey(billing.Currency.ToLower()))
            {
                todayEarnings[billing.Currency.ToLower()] += billing.Balance;
            }
            else
            {
                todayEarnings.Add(billing.Currency.ToLower(), billing.Balance);
            }
        }

        filter = Builders<Billing>.Filter.Gte(c => c.CreatedAt, DateTime.Now.Date);

        var billings4 = await DB.Collection<Billing>().Find(filter).ToListAsync();
        billings4 = billings4.Where(c => c.PaymentStatus == "succeeded").ToList();

        foreach (var billing in billings4)
        {
            if (todayEarnings.ContainsKey(billing.Currency.ToLower()))
            {
                todayEarnings[billing.Currency.ToLower()] += billing.Balance;
            }
            else
            {
                todayEarnings.Add(billing.Currency.ToLower(), billing.Balance);
            }
        }

        var TotalProducts = await DB.Collection<Product>().Find(Builders<Product>.Filter.Empty).CountDocumentsAsync();

        return new()
        {
            Orders = orders,
            Paid = paid,
            RefundedAmount = 0,
            TodayEarnings = todayEarnings,
            TotalProducts = TotalProducts
        };
    }

    public async Task<List<Billing>> GetBillingsWithIds(List<string> ids)
    {
        var filter = Builders<Billing>.Filter.In(x => x.ID, ids);

        return await DB.Collection<Billing>().Find(filter).ToListAsync();
    }

    public async Task<List<Billing>> GetMyBillingsAsync(string userId, string? queryString)
    {
        if (queryString != null)
        {
            IMongoQueryable<Billing> query = DB.Collection<Billing>().AsQueryable();

            APIFeatures<Billing> features = new(query, queryString);

            var billigs = await features.Filter().Sort().Paginate().GetQuery().Where(c => c.UserId.ToString() == userId).ToListAsync();

            var filterUserf = Builders<User>.Filter.Eq(c => c.ID, userId);
            var userf = await DB.Collection<User>().Find(filterUserf).FirstOrDefaultAsync();

            foreach (var billing in billigs)
            {
                billing.User = userf;
            }

            return billigs;
        }

        var filter = Builders<Billing>.Filter.Eq(c => c.UserId, ObjectId.Parse(userId));

        var billigsReturn = await DB.Collection<Billing>().Find(filter).ToListAsync();

        var filterUser = Builders<User>.Filter.Eq(c => c.ID, userId);
        var user = await DB.Collection<User>().Find(filterUser).FirstOrDefaultAsync();

        foreach (var billing in billigsReturn)
        {
            billing.User = user;
        }

        return billigsReturn;
    }

    public async Task<MyStatsResultDto> GetMyStatsAsync(string userId)
    {
        var filter = Builders<Billing>.Filter.Eq(c => c.UserId, ObjectId.Parse(userId));

        var billings = await DB.Collection<Billing>().Find(filter).ToListAsync();

        billings = billings.Where(b => b.PaymentStatus == "COMPLETED").ToList();

        var orders = billings.Count;

        var paid = new Dictionary<string, decimal>();

        foreach (var billing in billings)
        {
            if (paid.ContainsKey(billing.Currency.ToLower()))
            {
                paid[billing.Currency.ToLower()] += billing.Balance;
            }
            else
            {
                paid.Add(billing.Currency.ToLower(), billing.Balance);
            }
        }

        filter = Builders<Billing>.Filter.Eq(c => c.UserId, ObjectId.Parse(userId));
        var billings2 = await DB.Collection<Billing>().Find(filter).ToListAsync();
        billings2 = billings2.Where(c => c.PaymentStatus == "succeeded").ToList();

        orders += billings2.Count;

        foreach (var billing in billings2)
        {
            if (paid.ContainsKey(billing.Currency.ToLower()))
            {
                paid[billing.Currency.ToLower()] += billing.Balance;
            }
            else
            {
                paid.Add(billing.Currency.ToLower(), billing.Balance);
            }
        }

        return new()
        {
            Orders = orders,
            Paid = paid,
            RefundedAmount = 0
        };
    }
}