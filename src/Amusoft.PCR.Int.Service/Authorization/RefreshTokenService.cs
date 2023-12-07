using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Authorization;


	public interface IRefreshTokenManager
	{
		Task<bool> AddRefreshTokenAsync(ApplicationUser user, string refreshToken, DateTime validUntil);
		Task<RefreshToken> GetRefreshTokenAsync(ApplicationUser user, string refreshToken);
		Task<bool> InvalidateRefreshTokenAsync(ApplicationUser user, string refreshToken);
		Task<bool> DestroyRefreshTokenAsync(ApplicationUser user, string refreshToken);
		Task<bool> RemoveAllExpiredTokensAsync();
		Task<bool> RemoveAllAsync(ApplicationUser user);
	}

	public class RefreshTokenManager : IRefreshTokenManager
	{
		private readonly ILogger<RefreshTokenManager> _log;
		private readonly ApplicationDbContext _dbContext;

		public RefreshTokenManager(ILogger<RefreshTokenManager> log, ApplicationDbContext dbContext)
		{
			_log = log;
			_dbContext = dbContext;
		}


		public async Task<bool> AddRefreshTokenAsync(ApplicationUser user, string refreshToken, DateTime validUntil)
		{
			_log.LogDebug("Adding refresh token to user {Name}, valid until: {Date}", user.UserName, validUntil.ToString("F"));
			var entity = new RefreshToken();
			entity.IssuedAt = DateTime.UtcNow;
			entity.RefreshTokenId = refreshToken;
			entity.User = user;
			entity.ValidUntil = validUntil;

			_dbContext.RefreshTokens.Add(entity);

			var success = await _dbContext.SaveChangesAsync() > 0;
			if (!success)
			{
				_log.LogError("AddRefreshTokenAsync - Insert failed");
			}
			return success;
		}

		public Task<RefreshToken> GetRefreshTokenAsync(ApplicationUser user, string refreshToken)
		{
			var result = _dbContext.RefreshTokens.FirstOrDefaultAsync(d =>
				d.User == user && d.RefreshTokenId == refreshToken && d.ValidUntil > DateTime.UtcNow);

			return result;
		}

		public async Task<bool> InvalidateRefreshTokenAsync(ApplicationUser user, string refreshToken)
		{
			var entity = await _dbContext.RefreshTokens.FindAsync(user.Id, refreshToken);
			if (entity == null)
			{
				_log.LogCritical("Refresh token not found - this should never be happening");
				return false;
			}

			entity.IsUsed = true;
			_dbContext.Entry(entity).State = EntityState.Modified;
			var success = await _dbContext.SaveChangesAsync() > 0;
			if (!success)
			{
				_log.LogError("Failed to delete old refresh token {Token} for user {User}", refreshToken, user.UserName);
			}

			return success;
		}

		public async Task<bool> DestroyRefreshTokenAsync(ApplicationUser user, string refreshToken)
		{
			var entity = await _dbContext.RefreshTokens.FindAsync(user.Id, refreshToken);
			if (entity == null)
			{
				_log.LogCritical("Refresh token not found - this should never be happening");
				return false;
			}

			_dbContext.Remove(entity);
			var success = await _dbContext.SaveChangesAsync() > 0;
			if (!success)
			{
				_log.LogError("Failed to delete old refresh token {Token} for user {User}", refreshToken, user.UserName);
			}

			return success;
		}

		public async Task<bool> RemoveAllExpiredTokensAsync()
		{
			var entities = await _dbContext.RefreshTokens.Where(d => d.ValidUntil < DateTimeOffset.Now).ToListAsync();
			_dbContext.RemoveRange(entities);

			if (entities.Count == 0)
				return true;
			
			var changes = await _dbContext.SaveChangesAsync();
			return changes != entities.Count;
		}

		public async Task<bool> RemoveAllAsync(ApplicationUser user)
		{
			var entities = await _dbContext.RefreshTokens.Where(d => d.ValidUntil < DateTimeOffset.Now && d.User == user).ToListAsync();
			_dbContext.RemoveRange(entities);

			if (entities.Count == 0)
				return false;

			var changes = await _dbContext.SaveChangesAsync();
			return changes != entities.Count;
		}
	}