using MongoDB.Driver;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace RestroMenu.Helpers
{
    public class DbHelper
    {
        public enum Direction
        {
            Ascending,
            Descending
        };

        private readonly IMongoDatabase _database;
        private readonly Dictionary<Type, string> _collectionNames;

        public DbHelper(IMongoDatabase database)
        {
            _database = database;
            _collectionNames = new Dictionary<Type, string>()
            {
               { typeof(User),  "users" }
            };
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>()
            where TDocument : BaseModel
        {
            string collectionName = _collectionNames[typeof(TDocument)];

            return _database.GetCollection<TDocument>(collectionName);
        }

        public FilterDefinition<TDocument> LikeFilter<TDocument>(Expression<Func<TDocument, object>> field, string value)
        {
            return Builders<TDocument>.Filter.Regex(field, new BsonRegularExpression(value, "i"));
        }

        public SortDefinition<TDocument> GetSortDefinition<TDocument>(Expression<Func<TDocument, object>> field, Direction direction)
        {
            if (direction == Direction.Ascending)
            {
                return Builders<TDocument>.Sort.Ascending(field);
            }
            else
            {
                return Builders<TDocument>.Sort.Descending(field);
            }
        }

        public bool IdExists<TDocument>(string id, FilterDefinition<TDocument> filter)
            where TDocument : BaseModel
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            return RecordExists(Builders<TDocument>.Filter.Eq(x => x.Id, id) & filter);
        }

        public async Task<bool> IdExistsAsync<TDocument>(string id, FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
            where TDocument : BaseModel
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            return await RecordExistsAsync(Builders<TDocument>.Filter.Eq(x => x.Id, id) & filter, cancellationToken);
        }

        public bool RecordExists<TDocument>(FilterDefinition<TDocument> filter)
            where TDocument : BaseModel
        {
            var collection = GetCollection<TDocument>();

            return collection.Find(filter).Project(x => x.Id).FirstOrDefault() is not null;
        }

        public async Task<bool> RecordExistsAsync<TDocument>(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
            where TDocument : BaseModel
        {
            var collection = GetCollection<TDocument>();

            return await collection.Find(filter).Project(x => x.Id).FirstOrDefaultAsync(cancellationToken) is not null;
        }

        public TNewProjection? FindById<TDocument, TNewProjection>(
            string id,
            Expression<Func<TDocument, TNewProjection>> projection,
            FilterDefinition<TDocument> filter)
            where TDocument : BaseModel
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return default;
            }

            var collection = GetCollection<TDocument>();

            return collection
                .Find(Builders<TDocument>.Filter.Eq(x => x.Id, id) & filter)
                .Project(projection)
                .FirstOrDefault();
        }

        public async Task<TNewProjection?> FindByIdAsync<TDocument, TNewProjection>(
            string id,
            Expression<Func<TDocument, TNewProjection>> projection,
            FilterDefinition<TDocument> filter,
            CancellationToken cancellationToken = default)
            where TDocument : BaseModel
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return default;
            }

            var collection = GetCollection<TDocument>();

            return await collection
                .Find(Builders<TDocument>.Filter.Eq(x => x.Id, id) & filter)
                .Project(projection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TNewProjection>> FindByIdsAsync<TDocument, TNewProjection>(
            IEnumerable<string> ids,
            Expression<Func<TDocument, TNewProjection>> projection,
            FilterDefinition<TDocument> filter,
            CancellationToken cancellationToken = default)
            where TDocument : BaseModel
        {
            ids = ids.Where(x => ObjectId.TryParse(x, out _)).Distinct();

            if (!ids.Any())
            {
                return new List<TNewProjection>();
            }

            var collection = GetCollection<TDocument>();

            return await collection
                .Find(Builders<TDocument>.Filter.In(x => x.Id, ids) & filter)
                .Project(projection)
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<object, TNewProjection>> GetParentModels<TModel, TDocument, TNewProjection>(
            IEnumerable<TModel> models,
            Func<TModel, object> localField,
            Expression<Func<TDocument, object>> foreignField,
            Expression<Func<TDocument, TNewProjection>> projection,
            Func<TNewProjection, object> keySelector,
            FilterDefinition<TDocument> filter,
            CancellationToken cancellationToken = default)
            where TDocument : BaseModel
        {
            var ids = models.Select(localField).Where(x => x is not null).Distinct();

            var collection = GetCollection<TDocument>();

            var list = await collection
                .Find(Builders<TDocument>.Filter.In(foreignField, ids) & filter)
                .Project(projection)
                .ToListAsync(cancellationToken);

            return list.ToDictionary(keySelector, x => x);
        }

        public async Task AddManyToMany<TDocument, TRelatedDocument>(
            string id,
            IEnumerable<string> relatedIds,
            Expression<Func<TDocument, IEnumerable<string>>> field,
            Expression<Func<TRelatedDocument, IEnumerable<string>>> relatedField)
            where TDocument : BaseModel
            where TRelatedDocument : BaseModel
        {
            var currentRelatedIds = await FindByIdAsync(
                id,
                field,
                Builders<TDocument>.Filter.Empty
            );

            if (currentRelatedIds is null)
            {
                return;
            }

            relatedIds = relatedIds.Distinct();

            IEnumerable<string> added = relatedIds.Except(currentRelatedIds);

            if (!added.Any())
            {
                return;
            }

            var collection = GetCollection<TDocument>();
            var relatedCollection = GetCollection<TRelatedDocument>();

            await collection.UpdateOneAsync(Builders<TDocument>.Filter.Eq(x => x.Id, id),
                Builders<TDocument>.Update.AddToSetEach(field, added));

            await relatedCollection.UpdateManyAsync(Builders<TRelatedDocument>.Filter.In(x => x.Id, added),
                Builders<TRelatedDocument>.Update.AddToSet(relatedField, id));
        }

        public async Task RemoveManyToMany<TDocument, TRelatedDocument>(
            string id,
            IEnumerable<string> relatedIds,
            Expression<Func<TDocument, IEnumerable<string>>> field,
            Expression<Func<TRelatedDocument, IEnumerable<string>>> relatedField)
            where TDocument : BaseModel
            where TRelatedDocument : BaseModel
        {
            var currentRelatedIds = await FindByIdAsync(
                id,
                field,
                Builders<TDocument>.Filter.Empty
            );

            if (currentRelatedIds is null)
            {
                return;
            }

            relatedIds = relatedIds.Distinct();

            IEnumerable<string> removed = relatedIds.Intersect(currentRelatedIds);

            if (!removed.Any())
            {
                return;
            }

            var collection = GetCollection<TDocument>();
            var relatedCollection = GetCollection<TRelatedDocument>();

            await collection.UpdateOneAsync(Builders<TDocument>.Filter.Eq(x => x.Id, id),
                Builders<TDocument>.Update.PullAll(field, removed));

            await relatedCollection.UpdateManyAsync(Builders<TRelatedDocument>.Filter.In(x => x.Id, removed),
                Builders<TRelatedDocument>.Update.Pull(relatedField, id));
        }

        public async Task RemoveAllManyToMany<TDocument, TRelatedDocument>(
            string id,
            Expression<Func<TDocument, IEnumerable<string>>> field,
            Expression<Func<TRelatedDocument, IEnumerable<string>>> relatedField)
            where TDocument : BaseModel
            where TRelatedDocument : BaseModel
        {
            var currentRelatedIds = await FindByIdAsync(
                id,
                field,
                Builders<TDocument>.Filter.Empty
            );

            if (currentRelatedIds is null)
            {
                return;
            }

            if (!currentRelatedIds.Any())
            {
                return;
            }

            var collection = GetCollection<TDocument>();
            var relatedCollection = GetCollection<TRelatedDocument>();

            await collection.UpdateOneAsync(Builders<TDocument>.Filter.Eq(x => x.Id, id),
                Builders<TDocument>.Update.Set(field, new string[] { }));

            await relatedCollection.UpdateManyAsync(Builders<TRelatedDocument>.Filter.In(x => x.Id, currentRelatedIds),
                Builders<TRelatedDocument>.Update.Pull(relatedField, id));
        }

        public async Task SetManyToMany<TDocument, TRelatedDocument>(
            string id,
            IEnumerable<string> relatedIds,
            Expression<Func<TDocument, IEnumerable<string>>> field,
            Expression<Func<TRelatedDocument, IEnumerable<string>>> relatedField)
            where TDocument : BaseModel
            where TRelatedDocument : BaseModel
        {
            var currentRelatedIds = await FindByIdAsync(
                id,
                field,
                Builders<TDocument>.Filter.Empty
            );

            if (currentRelatedIds is null)
            {
                return;
            }

            relatedIds = relatedIds.Distinct();

            IEnumerable<string> added = relatedIds.Except(currentRelatedIds);
            IEnumerable<string> removed = currentRelatedIds.Except(relatedIds);

            if (!added.Any() && !removed.Any())
            {
                return;
            }

            var collection = GetCollection<TDocument>();
            var relatedCollection = GetCollection<TRelatedDocument>();

            await collection.UpdateOneAsync(Builders<TDocument>.Filter.Eq(x => x.Id, id),
                Builders<TDocument>.Update.Set(field, relatedIds));

            if (added.Any())
            {
                await relatedCollection.UpdateManyAsync(Builders<TRelatedDocument>.Filter.In(x => x.Id, added),
                Builders<TRelatedDocument>.Update.AddToSet(relatedField, id));
            }

            if (removed.Any())
            {
                await relatedCollection.UpdateManyAsync(Builders<TRelatedDocument>.Filter.In(x => x.Id, removed),
                Builders<TRelatedDocument>.Update.Pull(relatedField, id));
            }
        }
    }
}
