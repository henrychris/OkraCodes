import math
import re
from collections import Counter

# Regular expression for splitting text into words
WORD = re.compile(r"\w+")


def get_cosine_similarity(vec1, vec2):
    # Calculate the intersection of words in the two vectors
    intersection = set(vec1.keys()) & set(vec2.keys())

    # Calculate the numerator of the cosine similarity
    numerator = sum(vec1[word] * vec2[word] for word in intersection)

    # Calculate the sum of squares for each vector
    sum1 = sum(vec1[word] ** 2 for word in vec1)
    sum2 = sum(vec2[word] ** 2 for word in vec2)

    # Calculate the denominator of the cosine similarity
    denominator = math.sqrt(sum1) * math.sqrt(sum2)

    # Avoid division by zero
    if not denominator:
        return 0.0
    else:
        return float(numerator) / denominator


def text_to_vector(text):
    # Split the text into words and count their occurrences
    words = WORD.findall(text)
    return Counter(words)


def calculate_cosine_similarity(text1: str, text2: str) -> float:
    vector1 = text_to_vector(text1)
    vector2 = text_to_vector(text2)

    cosine = get_cosine_similarity(vector1, vector2)

    return cosine
