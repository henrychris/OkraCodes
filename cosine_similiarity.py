import math
import re
from collections import Counter

# Regular expression for splitting text into words
WORD = re.compile(r"\w+")


import math


def get_cosine_similarity(vec1: dict, vec2: dict) -> float:
    """
    Calculates the cosine similarity between two vectors.

    Args:
        vec1 (dict): The first vector represented as a dictionary of word frequencies.
        vec2 (dict): The second vector represented as a dictionary of word frequencies.

    Returns:
        float: The cosine similarity between the two vectors.
    """
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


from collections import Counter
import re

WORD = re.compile(r"\w+")


def text_to_vector(text: str) -> dict:
    """
    Convert a given text into a dictionary of word counts.

    Args:
        text (str): The text to be converted.

    Returns:
        dict: A dictionary where the keys are words in the text and the values are the number of times each word appears.
    """
    words = WORD.findall(text)
    return Counter(words)


def calculate_cosine_similarity(text1: str, text2: str) -> float:
    """
    Calculates the cosine similarity between two text strings.

    Args:
        text1 (str): The first text string.
        text2 (str): The second text string.

    Returns:
        float: The cosine similarity between the two text strings.
    """
    vector1 = text_to_vector(text1)
    vector2 = text_to_vector(text2)

    cosine = get_cosine_similarity(vector1, vector2)

    return cosine
