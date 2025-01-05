using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportAndFixtureWebApi.Models;

public partial class Fixture
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

    public int HomeTeamId { get; set; }

    public int? HomeTeamScore { get; set; }

    public int AwayTeamId { get; set; }

    public int? AwayTeamScore { get; set; }

    public DateOnly? MatchDate { get; set; }

    public virtual Team? AwayTeam { get; set; } = null!;

    public virtual Team? HomeTeam { get; set; } = null!;
}
