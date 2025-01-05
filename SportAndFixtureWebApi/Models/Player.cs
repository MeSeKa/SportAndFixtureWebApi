using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using SportAndFixtureWebApi.Models;

namespace SportAndFixtureWebApi.Models;

public partial class Player
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

    public string? FullName { get; set; }

    public int? TeamId { get; set; }

    public string? Position { get; set; }

    public virtual Team? Team { get; set; }
}
