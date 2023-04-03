"""
Tests for the datetime_helper module.

This module contains unit tests for the datetime_helper functions,
including calculate_buff_due_time and calculate_daily_doggo_time.
"""

from datetime import datetime, timedelta
import unittest

import pytz

from src.datetime_helper import calculate_buff_due_time, calculate_daily_doggo_time


class TestDateTimeHelper(unittest.IsolatedAsyncioTestCase):
    """
    A test class for the datetime_helper module.

    This class contains unit tests for the calculate_buff_due_time and
    calculate_daily_doggo_time functions.
    """

    async def test_calculate_buff_due_time(self):
        """
        Test the calculate_buff_due_time function.

        This method tests the calculate_buff_due_time function by checking if it
        correctly calculates the buff due time in UTC based on 12am Central Time
        of the next day, taking into consideration daylight saving time.
        """
        due_time = calculate_buff_due_time()

        central = pytz.timezone("US/Central")
        now_central = datetime.now(central)
        tomorrow_central = now_central + timedelta(days=1)
        tomorrow_12am_central = central.localize(
            datetime(
                tomorrow_central.year,
                tomorrow_central.month,
                tomorrow_central.day,
                0,
                0,
            )
        )
        tomorrow_12am_utc = tomorrow_12am_central.astimezone(pytz.UTC)

        if datetime.now(pytz.UTC) > tomorrow_12am_utc:
            expected_due_time = tomorrow_12am_utc + timedelta(days=1)
        else:
            expected_due_time = tomorrow_12am_utc

        self.assertEqual(due_time, expected_due_time)

    async def test_calculate_daily_doggo_time(self):
        """
        Test the calculate_daily_doggo_time function.

        This method tests the calculate_daily_doggo_time function by checking if it
        correctly calculates the daily doggo time in UTC based on 10am Central Time
        of the next day, taking into consideration daylight saving time.
        """
        due_time = calculate_daily_doggo_time()

        central = pytz.timezone("US/Central")
        now_central = datetime.now(central)
        tomorrow_central = now_central + timedelta(days=1)
        tomorrow_10am_central = central.localize(
            datetime(
                tomorrow_central.year,
                tomorrow_central.month,
                tomorrow_central.day,
                10,
                0,
            )
        )
        tomorrow_10am_utc = tomorrow_10am_central.astimezone(pytz.UTC)

        if datetime.now(pytz.UTC) > tomorrow_10am_utc:
            expected_due_time = tomorrow_10am_utc + timedelta(days=1)
        else:
            expected_due_time = tomorrow_10am_utc

        self.assertEqual(due_time, expected_due_time)
