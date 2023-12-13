using Microsoft.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class QuoteController
{
	private readonly ApplicationDbContext _context;

	public QuoteController(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<NutritionTipsAndQuotes> GetQuote()
	{
		var randomQuote = await _context.NutritionTipsAndQuotes
			.OrderBy(x => Guid.NewGuid())
			.FirstOrDefaultAsync();

		return randomQuote ?? new NutritionTipsAndQuotes { QuoteText = "" };
	}
}
