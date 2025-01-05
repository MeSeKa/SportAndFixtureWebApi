using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SportAndFixtureWebApi.Models;

public partial class Team
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

    public string? TeamName { get; set; }

    public string? City { get; set; }

	[JsonIgnore]
	public virtual ICollection<Fixture> FixtureAwayTeams { get; set; } = new List<Fixture>();
	[JsonIgnore]
	public virtual ICollection<Fixture> FixtureHomeTeams { get; set; } = new List<Fixture>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}


public class TeamDetailsDto
{
	public int Id { get; set; }
	public string TeamName { get; set; }
	public string City { get; set; }
	public int TotalMatches { get; set; }
	public int Wins { get; set; }
	public int Losses { get; set; }
	public int Draws { get; set; }
	public int NotPlayed { get; set; }
	public List<PlayerDto> Players { get; set; }
	public List<FixtureDto> Fixtures { get; set; }
}


public class PlayerDto
{
	public int PlayerId { get; set; }
	public string Name { get; set; }
	public string Position { get; set; }
}

public class FixtureDto
{
	public int FixtureId { get; set; }
	public string OpponentTeamName { get; set; }
	public DateOnly? Date { get; set; }
	public int? HomeTeamScore { get; set; }
	public int? AwayTeamScore { get; set; }
	public string Result { get; set; }
}

