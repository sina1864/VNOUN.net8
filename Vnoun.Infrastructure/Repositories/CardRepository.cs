using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class CardRepository : Repository<Card>, ICardRepository
{
    public async Task DeleteCardsForUser(string userId)
    {
        var filter = Builders<Card>.Filter.Eq(x => x.UserId, userId);
        var cards = await DB.Collection<Card>().Find(filter).ToListAsync();

        foreach (var card in cards)
        {
            var product = await DB.Find<Product>().OneAsync(card.ProductId);
            product.card_adds--;

            await product.SaveAsync();
            await card.DeleteAsync();
        }
    }

    public async Task DeleteCardsWithIds(List<string> cardIds, string userId)
    {
        var filter = Builders<Card>.Filter.In(x => x.ID, cardIds) & Builders<Card>.Filter.Eq(x => x.UserId, userId);
        var cards = await DB.Collection<Card>().Find(filter).ToListAsync();

        foreach (var card in cards)
        {
            var product = await DB.Find<Product>().OneAsync(card.ProductId);
            product.card_adds--;

            await product.SaveAsync();
            await card.DeleteAsync();
        }
    }

    public async Task DeleteWithIds(List<string> cardIds)
    {
        var filter = Builders<Card>.Filter.In(x => x.ID, cardIds);
        await DB.Collection<Card>().DeleteManyAsync(filter);
    }

    public async Task<decimal> GetAmountForCards(List<Card> cards)
    {
        decimal amount = 0;

        foreach (var card in cards)
        {
            if (card?.Product?.Colors == null)
            {
                continue;
            }

            var item = card.Product.Colors.Find(x => x.ColorCode == card.Color);
            double? price = item.Price;

            if (item.PriceDiscount > 0)
            {
                price -= (price * item.PriceDiscount);
            }
            else if (card.Product.GlobalDiscount > 0)
            {
                price -= (price * card.Product.GlobalDiscount);
            }

            amount += (decimal)price * (decimal)(card.NumberOfOrders);
        }

        return amount;
    }

    public Task<List<Card>> GetCardsForUser(string userId, string? queryString = null)
    {
        if (queryString != null)
        {
            IMongoQueryable<Card> query = DB.Collection<Card>().AsQueryable();
            APIFeatures<Card> features = new(query, queryString);

            return features.Filter().Sort().Paginate().GetQuery().Where(x => x.UserId == userId).ToListAsync();
        }

        var filter = Builders<Card>.Filter.Eq(x => x.UserId, userId);
        return DB.Collection<Card>().Find(filter).ToListAsync();
    }

    public async Task<List<Card>> GetCardsWithIds(List<string> cardIds)
    {
        var filter = Builders<Card>.Filter.In(x => x.ID, cardIds);
        var cards = await DB.Collection<Card>().Find(filter).ToListAsync();

        for (int i = 0; i < cards.Count; i++)
        {
            var product = await DB.Find<Product>().OneAsync(cards[i].ProductId);
            cards[i].Product = product;
        }

        return cards;
    }

    public async Task<List<Core.Entities.Order>> getCartDataAndUpdateQuantityOfProducts(List<Card> cards, string userId)
    {
        List<Core.Entities.Order> orders = new();

        foreach (var card in cards)
        {
            var order = new Core.Entities.Order
            {
                ID = ObjectId.GenerateNewId().ToString(),
                Name = card.Product.Name,
                Image = card.Product.Colors.Find(x => x.ColorCode == card.Color).Images[0].MediumImage,
                Color = card.Color,
                Size = card.Size,
                NumberOfOrders = card.NumberOfOrders,
                ProductId = ObjectId.Parse(card.ProductId),
            };

            var productColor = card.Product.Colors.Find(x => x.ColorCode == card.Color);
            productColor.Quantity -= card.NumberOfOrders;
            await productColor.SaveAsync();

            var product = await DB.Find<Product>().OneAsync(card.ProductId);

            product.Buyers ??= new();

            if (!product.Buyers.Contains(userId))
            {
                product.BuyersId.Add(ObjectId.Parse(userId));
            }

            await product.SaveAsync();

            orders.Add(order);
        }

        return orders;
    }
}