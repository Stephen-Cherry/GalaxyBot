"""A module defining a TypedDict for the response format of the random dog
and cat APIs."""

from __future__ import annotations
from typing import TypedDict


class Doggo(TypedDict):
    """A TypedDict class representing the response format of the random dog.

    Attributes:
        url (str): The URL of the dog picture.
    """

    url: str


class Cat(TypedDict):
    """A TypedDict class representing the response format of a random
    cat

    Attributes:
        url (str): The URL of the cat picture.
    """

    url: str
