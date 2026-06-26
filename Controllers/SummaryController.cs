using Microsoft.AspNetCore.Mvc;
using MediSummaryAI.Models;
using OpenAI.Chat;

namespace MediSummaryAI.Controllers;

public class SummaryController : Controller
{
    private readonly ChatClient _client;

    public SummaryController(IConfiguration config)
    {
        string apiKey = config["OPENAI_API_KEY"] 
            ?? throw new Exception("OpenAI API key is missing.");

        _client = new ChatClient(model: "gpt-4.1-mini", apiKey: apiKey);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new SummaryRequest());
    }

    [HttpPost]
    public async Task<IActionResult> Index(SummaryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DocumentText))
        {
            ModelState.AddModelError("", "Please enter text to summarize.");
            return View(request);
        }

        ChatCompletion completion = await _client.CompleteChatAsync(
            $"Summarize this document in simple terms. Include key points and action items:\n\n{request.DocumentText}"
        );

        request.SummaryResult = completion.Content[0].Text;

        return View(request);
    }
}