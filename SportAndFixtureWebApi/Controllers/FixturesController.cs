using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportAndFixtureWebApi.Models;

namespace SportAndFixtureWebApi.Controllers
{
	[AllowAnonymous]
	[Route("api/[controller]")]
	[ApiController]
	public class FixturesController : ControllerBase
	{
		private readonly SportAndFixtureDbContext _context;

		public FixturesController(SportAndFixtureDbContext context)
		{
			_context = context;
		}

		// GET: api/Fixtures
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Fixture>>> GetFixtures()
		{
			var fixtures = await _context.Fixtures
										 .Include(e => e.HomeTeam)
										 .Include(e => e.AwayTeam)
										 .ToListAsync();

			var options = new JsonSerializerOptions
			{
				ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
				WriteIndented = true
			};

			return new JsonResult(fixtures, options);
		}


		// GET: api/Fixtures/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Fixture>> GetFixture(int id)
		{
			var fixture = await _context.Fixtures.FindAsync(id);

			if (fixture == null)
			{
				return NotFound();
			}

			return fixture;
		}

		// PUT: api/Fixtures/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutFixture(int id, Fixture fixture)
		{
			if (id != fixture.Id)
			{
				return BadRequest();
			}

			_context.Entry(fixture).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FixtureExists(id))
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

		// POST: api/Fixtures
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Fixture>> PostFixture(Fixture fixture)
		{
			// AwayTeam ve HomeTeam objelerini veritabanından yükle
			fixture.AwayTeam = await _context.Teams.FindAsync(fixture.AwayTeamId);
			fixture.HomeTeam = await _context.Teams.FindAsync(fixture.HomeTeamId);

			if (fixture.AwayTeam == null || fixture.HomeTeam == null)
			{
				return BadRequest("One or both teams do not exist.");
			}

			_context.Fixtures.Add(fixture);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (FixtureExists(fixture.Id))
				{
					return Conflict();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetFixture", new { id = fixture.Id }, fixture);
		}


		// DELETE: api/Fixtures/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFixture(int id)
		{
			var fixture = await _context.Fixtures.FindAsync(id);
			if (fixture == null)
			{
				return NotFound();
			}

			_context.Fixtures.Remove(fixture);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool FixtureExists(int id)
		{
			return _context.Fixtures.Any(e => e.Id == id);
		}
	}
}
