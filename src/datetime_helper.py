"""
A module containing utility functions to calculate due times for specific events in a Discord bot.

This module provides two functions to calculate due times for events:

1. calculate_buff_due_time: 
    Calculates the buff due time based on the 12am Central Time of the next day.
2. calculate_daily_doggo_time: 
    Calculates the daily doggo time based on the 10am Central Time of the next day.

Both functions take into consideration daylight saving time and return the due time in UTC.
"""
from datetime import datetime, timedelta
import pytz


def calculate_buff_due_time() -> datetime:
    """
    Calculate the buff due time in UTC.

    This function calculates the buff due time based on the 12am Central Time of the next day,
    taking into consideration daylight saving time. The returned datetime is in UTC.

    Returns:
        datetime: The buff due time in UTC.
    """
    central = pytz.timezone("US/Central")
    now_central = datetime.now(central)

    tomorrow_central = now_central + timedelta(days=1)
    tomorrow_12am_central = tomorrow_central.replace(
        hour=0, minute=0, second=0, microsecond=0
    )

    tomorrow_12am_utc = tomorrow_12am_central.astimezone(pytz.UTC)

    if datetime.now(pytz.UTC) > tomorrow_12am_utc:
        return tomorrow_12am_utc + timedelta(days=1)
    return tomorrow_12am_utc


def calculate_daily_doggo_time() -> datetime:
    """
    Calculate the daily doggo time in UTC.

    This function calculates the daily doggo time based on the 10am Central Time of the next day,
    taking into consideration daylight saving time. The returned datetime is in UTC.

    Returns:
        datetime: The daily doggo time in UTC.
    """
    central = pytz.timezone("US/Central")
    now_central = datetime.now(central)

    tomorrow_central = now_central + timedelta(days=1)
    tomorrow_10am_central = tomorrow_central.replace(
        hour=10, minute=0, second=0, microsecond=0
    )

    tomorrow_10am_utc = tomorrow_10am_central.astimezone(pytz.UTC)

    if datetime.now(pytz.UTC) > tomorrow_10am_utc:
        return tomorrow_10am_utc + timedelta(days=1)
    return tomorrow_10am_utc
