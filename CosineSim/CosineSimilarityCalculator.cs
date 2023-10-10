using System.Text.RegularExpressions;

public partial class CosineSimilarityCalculator
{
	public static double CalculateCosineSimilarity(string text1, string text2)
	{
		Dictionary<string, int> vector1 = TextToVector(text1);
		Dictionary<string, int> vector2 = TextToVector(text2);

		double cosineSimilarity = GetCosineSimilarity(vector1, vector2);

		return cosineSimilarity;
	}

	public static Dictionary<string, int> TextToVector(string text)
	{
		// Regular expression for splitting text into words
		var wordRegex = MyRegex();

		// Find all words in the text
		var words = wordRegex.Matches(text)
							.Cast<Match>()
							.Select(match => match.Value.ToLower())
							.ToList();

		// Create a dictionary of word counts
		var wordCountDictionary = new Dictionary<string, int>();
		foreach (var word in words)
		{
			if (wordCountDictionary.ContainsKey(word))
			{
				wordCountDictionary[word]++;
			}
			else
			{
				wordCountDictionary[word] = 1;
			}
		}

		return wordCountDictionary;
	}

	public static double GetCosineSimilarity(Dictionary<string, int> vec1, Dictionary<string, int> vec2)
	{
		// Calculate the intersection of words in the two vectors
		var intersection = vec1.Keys.Intersect(vec2.Keys);

		// Calculate the numerator of the cosine similarity
		double numerator = intersection.Sum(word => vec1[word] * vec2[word]);

		// Calculate the sum of squares for each vector
		double sum1 = vec1.Values.Sum(value => Math.Pow(value, 2));
		double sum2 = vec2.Values.Sum(value => Math.Pow(value, 2));

		// Calculate the denominator of the cosine similarity
		double denominator = Math.Sqrt(sum1) * Math.Sqrt(sum2);

		// Avoid division by zero
		if (denominator == 0)
		{
			return 0.0;
		}
		else
		{
			return numerator / denominator;
		}
	}

	public static string PreprocessString(string text)
	{
		// Convert text to lowercase
		text = text.ToLower();

		// Remove non-alphanumeric characters
		text = Regex.Replace(text, @"\W+", " ");

		// Remove the word "bank"
		text = text.Replace("bank", "");

		// Remove parentheses
		text = text.Replace("(", "");
		text = text.Replace(")", "");

		return text;
	}


	public static int CalculateWordMatch(string sentence1, string sentence2)
	{
		// Regular expression for splitting text into words
		var wordRegex = MyRegex();

		// Tokenize both sentences
		var words1 = new HashSet<string>(wordRegex.Matches(sentence1)
												  .Cast<Match>()
												  .Select(match => match.Value.ToLower()));

		var words2 = new HashSet<string>(wordRegex.Matches(sentence2)
												  .Cast<Match>()
												  .Select(match => match.Value.ToLower()));

		// Calculate the number of matching words
		int matchingWords = words1.Intersect(words2).Count();

		return matchingWords;
	}

	// use 0.75 as threshold.
	// if a match is found, have the console prompt you: choose Y or N to save or skip.
	// if a match isnt found, input the code manually.
	// those that aren't matched should be left blank
	public static double CalculateFinalSimilarity(string sentence1, string sentence2)
	{
		// preprocess strings
		sentence1 = PreprocessString(sentence1);
		sentence2 = PreprocessString(sentence2);

		// Calculate cosine similarity
		double cosineSimilarity = GetCosineSimilarity(TextToVector(sentence1), TextToVector(sentence2));

		// if the cosine similarity is too low, it wont match anyway
		if (cosineSimilarity < 0.4)
		{
			return 0;
		}

		// Calculate word match score
		int wordMatchScore = CalculateWordMatch(sentence1, sentence2);

		// Calculate final similarity as the average of cosine similarity and word match score
		double finalSimilarity = (cosineSimilarity + wordMatchScore) / 2.0;

		// Ensure that the final similarity score is not greater than 100
		return Math.Min(finalSimilarity * 100, 100);
	}

	[GeneratedRegex("\\w+")]
	private static partial Regex MyRegex();
}