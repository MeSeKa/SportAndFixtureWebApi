using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportAndFixtureWebApi.Models;
using SportAndFixtureWebApi.Services;

namespace SportAndFixtureWebApi.Controllers
{
	[AllowAnonymous]
	[Route("api/[controller]")]
	[ApiController]
	public class TeamsController : ControllerBase
	{
		private readonly SportAndFixtureDbContext _context;
		private readonly TeamService _teamService;

		public TeamsController(SportAndFixtureDbContext context, TeamService teamService)
		{
			_context = context;
			_teamService = teamService;
		}

		// GET: api/Teams
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
		{
			return await _context.Teams.ToListAsync();
		}

		// GET: api/Teams/5
		[HttpGet("{id}")]
		public async Task<ActionResult<TeamDetailsDto>> GetTeam(int id)
		{
			var team = await _context.Teams
				.Include(t => t.FixtureHomeTeams)
				.ThenInclude(f => f.AwayTeam)
				.Include(t => t.FixtureAwayTeams)
				.ThenInclude(f => f.HomeTeam)
				.Include(t => t.Players)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (team == null)
			{
				return NotFound();
			}

			// Ev sahibi ve deplasman maçlarını birleştir
			var allFixtures = team.FixtureHomeTeams
				.Concat(team.FixtureAwayTeams)
				.OrderBy(f => f.MatchDate)
				.ToList();

			var totalMatches = allFixtures.Count;
			var notPlayed = allFixtures.Count(f => f.HomeTeamScore == -1 || f.AwayTeamScore == -1);
			var draws = allFixtures.Count(f => f.HomeTeamScore == f.AwayTeamScore && f.HomeTeamScore != -1);
			var wins = allFixtures.Count(f => IsWin(f, team.Id));
			var losses = totalMatches - wins - draws - notPlayed;

			// Detayları bir DTO'ya dönüştür
			var teamDetails = new TeamDetailsDto
			{
				Id = team.Id,
				TeamName = team.TeamName,
				City = team.City,
				TotalMatches = totalMatches,
				Wins = wins,
				Losses = losses,
				Draws = draws,
				NotPlayed = notPlayed,
				Players = team.Players.Select(p => new PlayerDto
				{
					PlayerId = p.Id,
					Name = p.FullName,
					Position = p.Position
				}).ToList(),
				Fixtures = allFixtures.Select(f => new FixtureDto
				{
					FixtureId = f.Id,
					OpponentTeamName = f.HomeTeamId == team.Id ? f.AwayTeam?.TeamName : f.HomeTeam?.TeamName,
					Date = f.MatchDate,
					HomeTeamScore = f.HomeTeamScore,
					AwayTeamScore = f.AwayTeamScore,
					Result = GetResult(f, team.Id)
				}).ToList()
			};

			return Ok(teamDetails);
		}

		// Kazanan takımın belirlenmesi için yardımcı fonksiyon
		private bool IsWin(Fixture fixture, int teamId)
		{
			if (fixture.HomeTeamScore == -1 || fixture.AwayTeamScore == -1)
				return false; // Oynanmamış maçları kazanan olarak sayma
			if (fixture.HomeTeamId == teamId && fixture.HomeTeamScore > fixture.AwayTeamScore)
				return true;
			if (fixture.AwayTeamId == teamId && fixture.AwayTeamScore > fixture.HomeTeamScore)
				return true;
			return false;
		}

		// Maç sonucunun belirlenmesi için yardımcı fonksiyon
		private string GetResult(Fixture fixture, int teamId)
		{
			if (fixture.HomeTeamScore == -1 || fixture.AwayTeamScore == -1)
				return "Not Played";
			if (fixture.HomeTeamScore == fixture.AwayTeamScore)
				return "Draw";
			if (IsWin(fixture, teamId))
				return "Win";
			return "Loss";
		}





		// PUT: api/Teams/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutTeam(int id, Team team)
		{
			if (id != team.Id)
			{
				return BadRequest();
			}

			_context.Entry(team).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TeamExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Teams
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Team>> PostTeam([Bind("TeamName,City")] Team team)
		{

			_context.Teams.Add(team);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (TeamExists(team.Id))
				{
					return Conflict();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetTeam", new { id = team.Id }, team);
		}


		// DELETE: api/Teams/5
		// DELETE: api/Teams/5
		// DELETE: api/Teams/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTeam(int id)
		{
			// Takım silinip silinemeyeceğini kontrol et
			bool canDelete = await _teamService.CanDeleteTeamAsync(id);

			if (!canDelete)
			{
				return BadRequest(new { message = "Cannot delete team. It has associated players or fixtures." });
			}

			// Takımı sil
			bool deleted = await _teamService.DeleteTeamAsync(id);

			if (!deleted)
			{
				return NotFound("Team not found.");
			}

			return Ok("Team deleted successfully.");
		}


		private bool TeamExists(int id)
		{
			return _context.Teams.Any(e => e.Id == id);
		}
	}
}
