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
Return ONLY a valid JSON object. No prose, no markdown tags.

Structure:
{{
  ""isResume"": boolean,
  ""headline"": ""string"",
  ""about"": ""string"",
  ""experienceYears"": ""string"",
  ""marketValue"": ""High/Mid/Low"",
  ""recommendation"": ""نصيحة احترافية بالعربي (2-3 جمل)"",
  ""targetTitle"": ""string"",
  ""skills"": [""string""],
  ""progress"": integer,
  ""missingSkillsWithImpact"": [
    {{ ""skillName"": ""string"", ""impactPercentage"": 5-12 }}
  ],
  ""roadmapData"": [ 
    {{
      ""skillName"": ""اسم المهارة الناقصة"",
      ""topResources"": [
        {{
          ""instructor"": ""اسم المدرب أو المنظمة"",
          ""courseTitle"": ""اسم الدورة أو التوثيق الرسمي"",
          ""platform"": ""YouTube/Udemy/Coursera/Official Docs"",
          ""url"": ""رابط مباشر مؤكد أو رابط بحث دقيق جداً""
        }}
      ]
    }}
  ]
}}

CRITICAL INSTRUCTIONS FOR 'url':
1. VERIFICATION: ONLY provide direct URLs if you are 100% certain they are active (e.g., official documentation like learn.microsoft.com or react.dev).
2. YOUTUBE FALLBACK: For YouTube courses, DO NOT invent playlist IDs. Instead, use a structured search URL that is GUARANTEED to work: 
   https://www.youtube.com/results?search_query=[Instructor]+[CourseName]+playlist
3. DIVERSITY: Provide 4 resources per skill. Top 2 MUST be the best available Arabic content. The other 2 can be top-tier English resources (like FreeCodeCamp or Official Docs).
4. NO DEAD LINKS: If a specific URL is uncertain, construct a precise search query for that resource on its respective platform.

Text to Analyze:
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

		// 1. فك تشفير الرد الأولي من جوجل
		var dynamicResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
		string aiJsonText = dynamicResponse.candidates[0].content.parts[0].text;

		// 2. تنظيف الـ Markdown tags (```json)
		aiJsonText = aiJsonText.Replace("```json", "").Replace("```", "").Trim();

		// 3. 🛡️ التعديل الجوهري: استخراج الـ JSON الصافي فقط 🛡️
		int startIndex = aiJsonText.IndexOf('{');
		int endIndex = aiJsonText.LastIndexOf('}');

		if (startIndex != -1 && endIndex != -1)
		{
			aiJsonText = aiJsonText.Substring(startIndex, (endIndex - startIndex) + 1);
		}
		else
		{
			throw new Exception("الذكاء الاصطناعي لم يرجع JSON صالح");
		}

		// 4. تحويل النص لكائن البرمجة (DTO)
		// استخدمنا Settings لضمان عدم الحساسية لحالة الأحرف
		var settings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore
		};

		return JsonConvert.DeserializeObject<AIAnalysisResult>(aiJsonText, settings)!;
	}
}

