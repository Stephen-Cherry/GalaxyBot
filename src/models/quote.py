"""
A module defining a TypedDict for the response format of the weight loss json data.
"""
from typing import TypedDict


class Quote(TypedDict):
    """
    A TypedDict class representing the response format of the weight loss json data.

    Attributes:
        quote (str): The quote.
        author (str): The author of the quote.
        id (int): The ID of the quote.
    """

    quote: str
    author: str
    id: int
