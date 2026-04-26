using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.AI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

public class GeminiService : IGeminiService
{
	private readonly HttpClient _httpClient;
	private readonly string _apiKey;

	// التعديل الأساسي: استخدمنا نفس الرابط اللي ضبط معك بالظبط
	const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

	public GeminiService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_apiKey = configuration.GetValue<string>("Gemini:ApiKey")
				  ?? throw new Exception("Gemini API Key missing");

		// إضافة الهيدر بنفس الصيغة اللي بالصورة
		if (!_httpClient.DefaultRequestHeaders.Contains("X-goog-api-key"))
		{
			_httpClient.DefaultRequestHeaders.Add("X-goog-api-key", _apiKey);
		}
	}

	public async Task<AIAnalysisResult> AnalyzeResumeAsync(string resumeText)
	{
		var prompt = $@"Analyze the following text and determine if it is a professional resume or CV. 
Your response must be ONLY a valid JSON object. Do not include markdown tags like ```json or any introductory text.

Required JSON Structure and Rules:
1. isResume: Boolean (true if the text is a resume/CV, false otherwise).

If isResume is false, return ONLY: {{ ""isResume"": false }}.

If isResume is true, return:
2. headline: Create a professional headline including the job title and top 3 technical skills separated by a vertical bar ' | ' (e.g., ""Full-stack Developer | .NET Core | React | SQL Server"").
3. about: A professional summary paragraph (3-4 sentences) based on the resume content.
4. experienceYears: Total years of experience as a string (e.g., ""2 Years"" or ""Entry Level"").
5. marketValue: Evaluate the candidate's skills and experience and return one of these values: ""High"", ""Mid"", or ""Low"".
6. recommendation: Provide 2-3 sentences of career advice or missing skills to improve their profile.
7. targetTitle: The most suitable job title the user should apply for (e.g., ""Junior .NET Developer"").
8. skills: A JSON array of strings containing the top 10 technical skills found in the resume.
9. progress: An integer (0-100) representing the profile readiness for the target title.
10. missingSkillsWithImpact: A JSON array of objects. Each object has 'skillName' and 'impactPercentage' (between 5-12). These are skills the user needs to reach the targetTitle.
Resume Text to Analyze:
{resumeText}";
		var requestBody = new
		{
			contents = new[]
			{
				new { parts = new[] { new { text = prompt } } }
			}
		};

		var jsonRequest = JsonConvert.SerializeObject(requestBody);
		var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync(ApiUrl, content);
		var responseString = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
		{
			throw new Exception($"Gemini Error: {response.StatusCode} - {responseString}");
		}

		// معالجة الرد
		var dynamicResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
		string aiJsonText = dynamicResponse.candidates[0].content.parts[0].text;

		// تنظيف النص من علامات الكود إذا وجدت
		aiJsonText = aiJsonText.Replace("```json", "").Replace("```", "").Trim();

		return JsonConvert.DeserializeObject<AIAnalysisResult>(aiJsonText)!;
	}

}