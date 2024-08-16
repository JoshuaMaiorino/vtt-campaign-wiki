using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using vtt_campaign_wiki.Server.Data;
using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Player.Services;

namespace vtt_campaign_wiki.Server.Features.Shared.Services
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class, IEntity
    {
        protected readonly VttCampaignWikiDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase( VttCampaignWikiDbContext context )
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual Task<T> GetByIdAsync( int id )
        {
            return GetByIdAsync( id, _dbSet );
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync( Expression<Func<T, bool>> filter = null )
        {
            var (items, itemsLength) = await GetAllAsync( _dbSet, null, filter );
            return items;
        }

        public virtual Task<(IEnumerable<T> Items, int ItemsLength)> GetAllAsync( PaginationParameter options, Expression<Func<T, bool>> filter = null )
        {
            return GetAllAsync( _dbSet, options, filter );
        }

        public virtual Task AddAsync( T entity )
        {
            SanitizeEntity( entity );
            return AddAsync( entity, null );
        }

        public virtual Task UpdateAsync( T entity )
        {
            SanitizeEntity( entity );
            return UpdateAsync( entity, null );
        }

        public virtual async Task DeleteAsync( int id )
        {
            var entity = await GetByIdAsync( id );
            if (entity != null)
            {
                if (entity is ItemBaseEntity entityBase)
                {
                    var currentPlayer = PlayerProvider.GetCurrentPlayer();
                    if (currentPlayer != null && entityBase.AuthorId != currentPlayer.Id)
                    {
                        throw new UnauthorizedAccessException( "Only the author can delete this entity." );
                    }
                }

                _dbSet.Remove( entity );
                await _context.SaveChangesAsync();
            }
        }

        protected virtual async Task<T> GetByIdAsync( int id, IQueryable<T>query)
        {
            return await query.FirstOrDefaultAsync( e => e.Id == id );
        }
        protected virtual async Task<(IEnumerable<T> Items, int ItemsLength)> GetAllAsync( IQueryable<T> query, PaginationParameter options = null, Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
            {
                query = query.Where( filter );
            }

            if (options == null)
            {
                options = new PaginationParameter();
            }

            if (!string.IsNullOrEmpty( options?.Search ))
            {
                query = ApplySearch( query, options.Search );
            }

            if (options.SortBy != null)
            {
                query = ApplySort( query, options.SortBy.ToList() );
            }

            var itemLength = await query.CountAsync();

            if (options.Page.HasValue && options.ItemsPerPage.HasValue)
            {
                int skip = (options.Page.Value - 1) * options.ItemsPerPage.Value;
                query = query.Skip( skip ).Take( options.ItemsPerPage.Value );
            }

            var items = await query.ToListAsync();

            return (items, itemLength);
        }

        protected virtual async Task UpdateAsync( T entity, Action<T> preUpdateAction )
        {
            var existingEntity = await GetByIdAsync( entity.Id );

            if (existingEntity == null)
            {
                return;
            }

            // Invoke the delegate if provided
            preUpdateAction?.Invoke( existingEntity );

            // Update scalar properties
            _dbSet.Entry( existingEntity ).CurrentValues.SetValues( entity );

            // Additional logic for ItemBaseEntity
            if (entity is ItemBaseEntity entityBase 
                && existingEntity is ItemBaseEntity existingEntityBase 
                && entityBase.Image != null)
            {
                existingEntityBase.Image = entityBase.Image;
            }

            await _context.SaveChangesAsync();
        }

        protected virtual async Task AddAsync( T entity, Action<T> preAddAction )
        {
            // Invoke the delegate if provided
            preAddAction?.Invoke( entity );

            if (entity is ItemBaseEntity entityBase)
            {
                var currentPlayer = PlayerProvider.GetCurrentPlayer();
                if (currentPlayer != null)
                {
                    entityBase.Author = currentPlayer;
                    entityBase.AuthorId = currentPlayer.Id;
                }
            }

            await _dbSet.AddAsync( entity );
            await _context.SaveChangesAsync();
        }

        public override bool Equals( object? obj )
        {
            return obj is RepositoryBase<T> @base &&
                   EqualityComparer<VttCampaignWikiDbContext>.Default.Equals( _context, @base._context ) &&
                   EqualityComparer<DbSet<T>>.Default.Equals( _dbSet, @base._dbSet );
        }

        protected async Task LoadChildrenRecursively( CampaignItemEntity parent )
        {
            // Load the children of the current parent
            await _context.Entry( parent ).Collection( x => x.Children ).LoadAsync();

            // Sort the children by Position
            parent.Children = parent.Children.OrderBy( c => (double)c.Position ).ToList();

            // Recursively load and sort the children of each child
            foreach (var child in parent.Children)
            {
                await LoadChildrenRecursively( child );
            }
        }
        protected virtual IQueryable<T> ApplySearch( IQueryable<T> query, string search )
        {
            // Get the type of T
            Type type = typeof( T );

            // Try to get the "Title" property
            PropertyInfo titleProperty = type.GetProperty( "Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase );

            if (titleProperty != null && titleProperty.PropertyType == typeof( string ))
            {
                // Create a parameter expression representing a parameter of type T
                ParameterExpression param = Expression.Parameter( type, "x" );

                // Create an expression representing accessing the "Title" property
                MemberExpression propertyAccess = Expression.Property( param, titleProperty );

                // Create an expression representing the search string
                ConstantExpression searchExpression = Expression.Constant( search );

                // Create an expression representing the call to "Contains" method
                MethodInfo containsMethod = typeof( string ).GetMethod( "Contains", new[] { typeof( string ) } );
                MethodCallExpression containsCall = Expression.Call( propertyAccess, containsMethod, searchExpression );

                // Create a lambda expression representing the where clause
                Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>( containsCall, param );

                // Apply the where clause to the query
                query = query.Where( lambda );
            }

            return query;
        }
        protected virtual IQueryable<T> ApplySort( IQueryable<T> query, List<SortItem> sortBy )
        {
            if (sortBy != null && sortBy.Any())
            {
                bool isFirstOrderBy = true;

                foreach (var sortItem in sortBy)
                {
                    var parameter = Expression.Parameter( typeof( T ), "e" );
                    var property = Expression.Property( parameter, sortItem.Key );
                    var lambda = Expression.Lambda( property, parameter );

                    string methodName;

                    if (isFirstOrderBy)
                    {
                        methodName = sortItem.Order == SortOrder.Descending ? "OrderByDescending" : "OrderBy";
                        isFirstOrderBy = false;
                    }
                    else
                    {
                        methodName = sortItem.Order == SortOrder.Descending ? "ThenByDescending" : "ThenBy";
                    }

                    query = typeof( Queryable ).GetMethods()
                        .First( method => method.Name == methodName && method.GetParameters().Length == 2 )
                        .MakeGenericMethod( typeof( T ), property.Type )
                        .Invoke( null, new object[] { query, lambda } ) as IQueryable<T>;
                }
            }
            else
            {
                // Default sorting by "Position" property if it exists
                PropertyInfo positionProperty = typeof( T ).GetProperty( "Position" );
                if (positionProperty != null && positionProperty.PropertyType == typeof( decimal ))
                {
                    query = query.OrderBy( e => (double) EF.Property<decimal>( e, "Position" ) );
                }
            }

            return query;
        }
        protected virtual void SanitizeEntity( T entity )
        {
            var entityType = entity.GetType();

            foreach (var property in entityType.GetProperties( BindingFlags.Public | BindingFlags.Instance ))
            {
                if (property.PropertyType == typeof( string ))
                {
                    var value = property.GetValue( entity ) as string;
                    if (!string.IsNullOrEmpty( value ) && value.Trim() == "<p><br></p>")
                    {
                        property.SetValue( entity, string.Empty );
                    }
                }
            }
        }

    }
}
