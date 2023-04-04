"""
A module containing utility functions to calculate due times for specific events in a Discord bot.

This module provides a primary function to calculate due times for events, 
along with three wrapper functions:

calculate_due_time:
    Calculates the due time for a specific event based on the given hour in Central Time of the next 
    day.
calculate_buff_due_time:
    Calculates the buff due time based on the 12am Central Time of the next day.
calculate_daily_doggo_time:
    Calculates the daily doggo time based on the 10am Central Time of the next day.
calculate_daily_quote_task:
    Calculates the daily quote time based on the 9am Central Time of the next day.

All functions take into consideration daylight saving time and return the due time in UTC.
"""
from datetime import datetime, timedelta
import pytz


def calculate_due_time(hour: int) -> datetime:
    """
    Calculate the due time for a specific event in UTC.

    This function calculates the due time based on the specified hour in Central Time of the next
    day, taking into consideration daylight saving time. The returned datetime is in UTC.

    Args:
        hour (int): The hour of the event in Central Time.

    Returns:
        datetime: The due time for the event in UTC.
    """
    central = pytz.timezone("US/Central")
    now_central = datetime.now(central)

    tomorrow_central = now_central + timedelta(days=1)
    tomorrow_hour_central = tomorrow_central.replace(
        hour=hour, minute=0, second=0, microsecond=0
    )

    tomorrow_hour_utc = tomorrow_hour_central.astimezone(pytz.UTC)

    if datetime.now(pytz.UTC) > tomorrow_hour_utc:
        return tomorrow_hour_utc + timedelta(days=1)
    return tomorrow_hour_utc


def calculate_buff_due_time() -> datetime:
    """
    Calculate the buff due time in UTC.

    This function calculates the buff due time based on the 12am Central Time of the next day,
    taking into consideration daylight saving time. The returned datetime is in UTC.

    Returns:
        datetime: The buff due time in UTC.
    """
    return calculate_due_time(0)


def calculate_daily_doggo_time() -> datetime:
    """
    Calculate the daily doggo time in UTC.

    This function calculates the daily doggo time based on the 10am Central Time of the next day,
    taking into consideration daylight saving time. The returned datetime is in UTC.

    Returns:
        datetime: The daily doggo time in UTC.
    """
    return calculate_due_time(10)


def calculate_daily_quote_time() -> datetime:
    """
    Calculate the daily quote time in UTC.

    This function calculates the daily quote time based on the 9am Central Time of the next day,
    taking into consideration daylight saving time. The returned datetime is in UTC.

    Returns:
        datetime: The daily quote time in UTC.
    """
    return calculate_due_time(9)
