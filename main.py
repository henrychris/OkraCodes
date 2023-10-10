import re
from collections import Counter
from cosine_similiarity import *


def preprocess_string(text: str) -> str:
    """
    Preprocesses a given string by converting it to lowercase, removing non-alphanumeric characters,
    removing the word "bank", removing parentheses, and removing the words "of", "by", and "and".

    Args:
            text (str): The string to preprocess.

    Returns:
            str: The preprocessed string.
    """
    # Convert text to lowercase
    text = text.lower()

    # Remove non-alphanumeric characters
    text = re.sub(r"\W+", " ", text)

    # Remove the word "bank"
    text = text.replace("bank", "")

    # Remove parentheses
    text = text.replace("(", "")
    text = text.replace(")", "")

    return text


def calculate_word_match(sentence1: str, sentence2: str) -> int:
    # Tokenize both sentences
    words1 = set(WORD.findall(sentence1))
    words2 = set(WORD.findall(sentence2))

    # Calculate the number of matching words
    matching_words = len(words1.intersection(words2))

    return matching_words


def calculate_final_similarity(sentence1: str, sentence2: str) -> float:
    # Calculate cosine similarity
    cosine_similarity = get_cosine_similarity(
        text_to_vector(sentence1), text_to_vector(sentence2)
    )

    # Calculate word match score
    word_match_score = calculate_word_match(sentence1, sentence2)

    # Calculate final similarity as the average of cosine similarity and word match score
    final_similarity = (cosine_similarity + word_match_score) / 2.0

    # Ensure that the final similarity score is not greater than 100
    return min(final_similarity * 100, 100)


def main():
    # Example sentences
    data_list = [
        {"text": "PAYCOM (OPAY)", "text2": "OPAY"},
        {"text": "Alat", "text2": "ALAT by WEMA"},
        {"text": "Bowen Microfinance Bank", "text2": "Bowen Microfinance Bank"},
        {"text": "Ecobank Nigeria", "text2": "Ecobank Nigeria"},
        {"text": "Eyowo", "text2": "EYOWO MICROFINANCE BANK"},
        {"text": "Hasal Microfinance Bank", "text2": "Hasal Microfinance Bank"},
        {"text": "CEMCS Microfinance Bank", "text2": "Guaranty Trust Bank"},
        {"text": "Access Bank (Diamond)", "text2": "Diamond bank"},
        {"text": "Parkway - ReadyCash", "text2": "ReadyCash (Parkway)"},
    ]
    for data in data_list:
        sentence1 = data["text"]
        sentence2 = data["text2"]

        final_similarity = calculate_final_similarity(
            preprocess_string(sentence1), preprocess_string(sentence2)
        )

        # convert final_similarity to a percentage
        print("Sentence 1:", sentence1)
        print("Sentence 2:", sentence2)
        print("\nFinal Similarity:", final_similarity)
        print("\n")


# ! use 75 as a threshold
main()