"""A module defining a TypedDict for the response format of the random dog
picture API."""

from typing import TypedDict


class DoggoResponse(TypedDict):
    """A TypedDict class representing the response format of the random dog
    picture API.

    Attributes:
        fileSizeBytes (int): The file size of the dog picture in bytes.
        url (str): The URL of the dog picture.
    """

    fileSizeBytes: int
    url: str
