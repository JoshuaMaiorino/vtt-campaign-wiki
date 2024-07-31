﻿using System.Linq.Expressions;
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

        public virtual async Task<T> GetByIdAsync( int id )
        {
            return await _dbSet.FindAsync( id );
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync( Expression<Func<T, bool>> filter = null )
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where( filter );
            }

            PropertyInfo positionProperty = typeof( T ).GetProperty( "Position" );
            if (positionProperty != null && positionProperty.PropertyType == typeof( decimal ))
            {
                query = query.OrderBy( e => (double)EF.Property<decimal>( e, "Position" ) );
            }

            return await query
                .ToListAsync();
        }

        public virtual async Task AddAsync( T entity )
        {
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

        public virtual async Task UpdateAsync( T entity )
        {
            var exisitingEntity = await GetByIdAsync( entity.Id );

            if (exisitingEntity == null)
            {
                return;
            }

            _dbSet.Entry( exisitingEntity ).CurrentValues.SetValues( entity );

            if (entity is ItemBaseEntity entityBase)
            {
                var currentPlayer = PlayerProvider.GetCurrentPlayer();
                if (currentPlayer != null && entityBase.AuthorId != currentPlayer.Id)
                {
                    throw new UnauthorizedAccessException( "Only the author can update this entity." );
                }

                if (exisitingEntity is ItemBaseEntity existingEntityBase && entityBase.Image != null )
                {
                    existingEntityBase.Image = entityBase.Image;
                }
            }
            
            await _context.SaveChangesAsync();
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
    }
}
