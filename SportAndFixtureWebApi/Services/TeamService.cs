using SportAndFixtureWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace SportAndFixtureWebApi.Services
{
	public class TeamService
	{
		private readonly SportAndFixtureDbContext _context;

		public TeamService(SportAndFixtureDbContext context)
		{
			_context = context;
		}

		// Takımın silinip silinemeyeceğini kontrol eden metot
		public async Task<bool> CanDeleteTeamAsync(int teamId)
		{
			bool hasPlayers = await _context.Players.AnyAsync(p => p.TeamId == teamId);
			bool hasFixtures = await _context.Fixtures.AnyAsync(f => f.HomeTeamId == teamId || f.AwayTeamId == teamId);

			return !(hasPlayers || hasFixtures); // Oyuncu veya fixture yoksa true döner
		}

		// Takımı silen metot
		public async Task<bool> DeleteTeamAsync(int teamId)
		{
			var team = await _context.Teams.FindAsync(teamId);

			if (team == null)
			{
				return false; // Takım bulunamadı
			}

			_context.Teams.Remove(team);
			await _context.SaveChangesAsync();
			return true; // Başarıyla silindi
		}
	}
}
