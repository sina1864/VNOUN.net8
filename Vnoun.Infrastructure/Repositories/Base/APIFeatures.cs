using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Vnoun.Core.Entities;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Infrastructure.Repositories.Base;

public class APIFeatures<T>
{
    private IMongoQueryable<T> _query;
    private readonly Dictionary<string, string> _queryString;
    public APIFeatures(IMongoQueryable<T> query, string queryString)
    {
        _query = query;
        Dictionary<string, object>? queryDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(queryString ?? "{}");

        _queryString = queryDict.ToDictionary(k => k.Key, v => v.Value.ToString());
    }

    public APIFeatures<T> Filter()
    {
        var queryObj = new Dictionary<string, string>(_queryString);
        var excludedFields = new List<string> { "page", "sort", "limit", "fields", "skip" };

        excludedFields.ForEach(el => queryObj.Remove(el));

        var queryStr = JObject.FromObject(queryObj).ToString();

        var regex = new Regex(@"\b(gte|gt|lte|lt|in|elemMatch|eq|ne)\b");

        queryStr = regex.Replace(queryStr, match => $"${match}");

        var filter = Builders<T>.Filter.Empty;

        if (queryObj.ContainsKey("colors.price"))
        {
            var val = queryObj["colors.price"];

            var valObj = JObject.Parse(val);
            var gt = valObj["gt"];
            var lt = valObj["lt"];

            var gtvalue = gt.Value<int>();
            filter &= Builders<T>.Filter.ElemMatch("colors", Builders<Color>.Filter.Gt("price", gtvalue));
            var ltvalue = lt.Value<int>();
            filter &= Builders<T>.Filter.ElemMatch("colors", Builders<Color>.Filter.Lt("price", ltvalue));

            queryObj.Remove("colors.price");
        }

        if (queryObj.ContainsKey("color"))
        {
            var valObj = JObject.Parse(queryObj["color"]);
            var InValue = valObj["in"];

            if (InValue != null)
            {
                var InList = InValue.ToObject<List<string>>();
                filter &= Builders<T>.Filter.ElemMatch("colors", Builders<Color>.Filter.In("colorName", InList));
            }

            queryObj.Remove("color");
        }

        var parsedFilter = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(queryStr);

        if (parsedFilter.Contains("colors.price"))
        {
            parsedFilter.Remove("colors.price");
        }

        if (parsedFilter.Contains("color"))
        {
            parsedFilter.Remove("color");
        }

        foreach (var (key, value) in queryObj)
        {
            var properties = typeof(T).GetProperties();

            PropertyInfo property = null;

            foreach (var prop in properties)
            {
                var fieldAttr = prop.GetCustomAttributes(typeof(MongoDB.Entities.FieldAttribute), true).FirstOrDefault() as MongoDB.Entities.FieldAttribute;

                if (fieldAttr != null && fieldAttr.ElementName == key)
                {
                    property = prop;
                    break;
                }
            }

            if (property == null)
            {
                continue;
            }

            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (!value.Contains("{"))
                {
                    filter &= Builders<T>.Filter.ElemMatch(key, Builders<T>.Filter.Regex("$", new BsonRegularExpression(value, "i")));
                }
                else
                {
                    var valObj = JObject.Parse(value);
                    var InValue = valObj["in"];
                    var GtValue = valObj["gt"];
                    var LtValue = valObj["lt"];

                    if (InValue != null)
                    {
                        var InList = InValue.ToObject<List<string>>();
                        filter &= Builders<T>.Filter.AnyIn(key, InList);
                    }

                    if (GtValue != null)
                    {
                        var gtvalue = GtValue.Value<int>();
                        filter &= Builders<T>.Filter.AnyGt(key, gtvalue);
                    }

                    if (LtValue != null)
                    {
                        var ltvalue = LtValue.Value<int>();
                        filter &= Builders<T>.Filter.AnyLt(key, ltvalue);
                    }
                }
                parsedFilter.Remove(key);
            }
            else if (!value.Contains("{"))
            {
                filter &= Builders<T>.Filter.Regex(key, new BsonRegularExpression(value, "i"));
                parsedFilter.Remove(key);
            }
        }

        filter &= Builders<T>.Filter.And(filter, new BsonDocumentFilterDefinition<T>(parsedFilter));
        _query = _query.Where(c => filter.Inject());

        return this;
    }

    public APIFeatures<T> Sort()
    {
        if (_queryString.ContainsKey("sort"))
        {
            var sortBy = _queryString["sort"].Replace("-", "");

            if (sortBy == "colors.price")
            {
                var query = _query as IMongoQueryable<Product>;
                query = query.OrderByDescending(p => p.Colors.Max(c => c.Price));

                _query = query as IMongoQueryable<T>;

                return this;
            }

            ParameterExpression pe = Expression.Parameter(typeof(T), "t");
            MemberExpression me = Expression.Property(pe, sortBy);
            Expression conversion = Expression.Convert(me, typeof(object));
            Expression<Func<T, object>> orderExpression = Expression.Lambda<Func<T, object>>(conversion, new[] { pe });

            if (_queryString["sort"].StartsWith("-"))
                _query = _query.OrderByDescending(orderExpression);
            else
                _query = _query.OrderBy(orderExpression);

            return this;
        }
        else
        {
            return this;
        }
    }

    public APIFeatures<T> Paginate()
    {
        var limit = _queryString.ContainsKey("limit") ? Convert.ToInt32(_queryString["limit"]) : 100;
        var skip = _queryString.ContainsKey("skip") ? Convert.ToInt32(_queryString["skip"]) : 0;

        _query = _query.Skip(skip).Take(limit);

        return this;
    }

    public async Task<List<T>> ToListAsync()
    {
        return await _query.ToListAsync();
    }

    public IMongoQueryable<T> GetQuery()
    {
        return _query;
    }
}
