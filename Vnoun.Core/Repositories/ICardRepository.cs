using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Core.Repositories;
public interface ICardRepository : IRepository<Card>
{
    Task DeleteCardsForUser(string userId);
    Task<List<Card>> GetCardsForUser(string userId, string query = null);
    Task DeleteCardsWithIds(List<string> cardIds, string userId);
    Task<List<Card>> GetCardsWithIds(List<string> cardIds);
    Task<decimal> GetAmountForCards(List<Card> cards);
    Task<List<Order>> getCartDataAndUpdateQuantityOfProducts(List<Card> cards, string userId);
    Task DeleteWithIds(List<string> cardIds);
}