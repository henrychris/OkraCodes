// Example sentences
#region test

// using System.Diagnostics;

// List<Dictionary<string, string>> data_list = new()
// {
// 			new Dictionary<string, string> {{"text", "PAYCOM (OPAY)"}, {"text2", "OPAY"}},
// 			new Dictionary<string, string> {{"text", "Alat"}, {"text2", "ALAT by WEMA"}},
// 			new Dictionary<string, string> {{"text", "Bowen Microfinance Bank"}, {"text2", "Bowen Microfinance Bank"}},
// 			new Dictionary<string, string> {{"text", "Ecobank Nigeria"}, {"text2", "Ecobank Nigeria"}},
// 			new Dictionary<string, string> {{"text", "Eyowo"}, {"text2", "EYOWO MICROFINANCE BANK"}},
// 			new Dictionary<string, string> {{"text", "Hasal Microfinance Bank"}, {"text2", "Hasal Microfinance Bank"}},
// 			new Dictionary<string, string> {{"text", "CEMCS Microfinance Bank"}, {"text2", "Guaranty Trust Bank"}},
// 			new Dictionary<string, string> {{"text", "Access Bank (Diamond)"}, {"text2", "Diamond bank"}},
// 			new Dictionary<string, string> {{"text", "Parkway - ReadyCash"}, {"text2", "ReadyCash (Parkway)"}}
// 		};

// foreach (var data in data_list)
// {

// 	string sentence1 = data["text"];
// 	string sentence2 = data["text2"];

// 	// Measure execution time using Stopwatch
// 	Stopwatch stopwatch = Stopwatch.StartNew();
// 	double finalSimilarity = CosineSimilarityCalculator
// 		.CalculateFinalSimilarity(sentence1, sentence2);

// 	Console.WriteLine("Sentence 1: " + sentence1);
// 	Console.WriteLine("Sentence 2: " + sentence2);
// 	Console.WriteLine("\nFinal Similarity: " + finalSimilarity + "\n");
// 	Console.WriteLine("Execution Time: " + stopwatch.ElapsedMilliseconds + " ms\n");
// }

#endregion
using Newtonsoft.Json;

namespace CosineSim
{
	public class Program
	{
		private static readonly HttpClient httpClient = new();
		private static Dictionary<string, string> providerDictionary = new();
		private static readonly int MatchThreshold = 75;

		public static async Task Main(string[] args)
		{
			// Step 1: Connect to the database and get banks with OCode as null
			var databaseBanksWithNullOCode = new List<Bank>
			{

			};

			// Step 2: Send an API request to get the list of banks from the provider
			string providerApiUrl = "your_provider_api_url";
			// todo: deserialise this from stream.
			string providerApiResponse = await httpClient.GetStringAsync(providerApiUrl) ?? throw new Exception("API response is null");

			// Step 3: Create a dictionary from the provider's list
			// You can parse the JSON response into the dictionary
			providerDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(providerApiResponse)
						?? throw new Exception("Provider dictionary is null");

			// Step 3: Sort the provider dictionary alphabetically
			providerDictionary = providerDictionary.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

			// Step 4: Iterate over the list and check for similarity
			foreach (var bankEntry in databaseBanksWithNullOCode)
			{
				string databaseBankName = bankEntry.Name;

				// return a class with both the match and the similarity
				var matchResults = FindSimilarMatch(databaseBankName, providerDictionary);

				if (matchResults != null)
				{
					Console.WriteLine($"Current Bank Name: {databaseBankName}");
					Console.WriteLine($"Match Found: {matchResults.ProviderBankName}");

					// Step 5: Automatically save if match is 100%
					if (IsPerfectMatch(matchResults.Similarity))
					{
						// Save automatically
						SaveMatchedData(databaseBankName, matchResults.ProviderId);
						continue;
					}

					// Step 6: Prompt user to manually provide an ID
					ManuallyProvideID(bankEntry);
				}

				Console.WriteLine($"Current Bank Name: {databaseBankName}");
				ManuallyProvideID(bankEntry);
			}
		}


		private static MatchResults? FindSimilarMatch(string databaseBankName, Dictionary<string, string> providerDictionary)
		{
			double bestSimilarity = 0.0;
			string? bestMatchingName = null;
			string? bestProviderId = null;
			string? keyToRemove = null;

			foreach (var kvp in providerDictionary)
			{
				string providerBankName = kvp.Key;
				string providerId = kvp.Value;

				var similarity = CosineSimilarityCalculator.CalculateFinalSimilarity(databaseBankName, providerBankName);

				if (PassesMatchThreshold(similarity))
				{
					if (similarity > bestSimilarity)
					{
						bestSimilarity = similarity;
						bestMatchingName = providerBankName;
						bestProviderId = providerId;

						// Store the key to remove
						keyToRemove = kvp.Key;
					}
				}
			}

			if (keyToRemove != null)
			{
				// Remove the key from the dictionary
				providerDictionary.Remove(keyToRemove);
			}

			if (bestMatchingName is not null && bestProviderId is not null)
			{
				return new MatchResults(bestSimilarity, bestProviderId, bestMatchingName);
			}

			return null; // No suitable match found
		}

		private static void ManuallyProvideID(Bank bankEntry)
		{
			Console.WriteLine("Do you want to manually provide an ID for this match? (YES/NO): ");
			string userInput = Console.ReadLine();

			if (userInput?.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
			{
				Console.WriteLine("Enter the ID (or leave empty for NULL): ");
				string userProvidedId = Console.ReadLine();

				SaveMatchedData(bankEntry.Id, userProvidedId);
			}
			else
			{
				SaveMatchedData(bankEntry.Id, null); // Save as NULL
			}
		}

		private static bool IsPerfectMatch(double match)
		{
			return match > 90;
		}

		private static bool PassesMatchThreshold(double match)
		{
			return match > MatchThreshold;
		}

		// if ID is null, save it like that
		private static void SaveMatchedData(string databaseBankId, string? providerId)
		{
			// Implement logic to save the matched data to your database
		}
	}

	public class MatchResults
	{
		public MatchResults(double similarity, string providerId, string providerBankName)
		{
			Similarity = similarity;
			ProviderId = providerId;
			ProviderBankName = providerBankName;
		}

		public double Similarity { get; set; }
		public string ProviderBankName { get; set; }
		public string ProviderId { get; set; }

	}
	public class Bank
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
}