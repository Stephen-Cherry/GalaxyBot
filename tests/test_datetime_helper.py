"""
Tests for the datetime_helper module.

This module contains unit tests for the datetime_helper functions,
including calculate_buff_due_time and calculate_daily_doggo_time.
"""

from datetime import datetime
import unittest

import pytz

from src.datetime_helper import (
    calculate_buff_due_time,
    calculate_daily_doggo_time,
    calculate_daily_quote_time,
    calculate_due_time,
)


class TestDateTimeHelper(unittest.IsolatedAsyncioTestCase):
    """
    A test class for the datetime_helper module.

    This class contains unit tests for the calculate_buff_due_time and
    calculate_daily_doggo_time functions.
    """

    async def test_calculate_due_time(self):
        """
        Test the calculate_due_time function.

        This method tests the calculate_due_time function by checking if it
        correctly calculates the buff due time in UTC based on 5am Central Time
        of the next day, taking into consideration daylight saving time.
        """
        # Test with a specific hour
        hour = 5
        due_time = calculate_due_time(hour)
        self.assertIsInstance(due_time, datetime)

        # Test if due time is in the future
        self.assertGreater(due_time, datetime.now(pytz.UTC))

        # Test if due time has the correct hour in Central Time
        central = pytz.timezone("US/Central")
        due_time_central = due_time.astimezone(central)
        self.assertEqual(due_time_central.hour, hour)

    async def test_calculate_buff_due_time(self):
        """
        Test the calculate_buff_due_time function.

        This method tests the calculate_buff_due_time function by checking if it
        correctly calculates the buff due time in UTC based on 12am Central Time
        of the next day, taking into consideration daylight saving time.
        """
        due_time = calculate_buff_due_time()
        self.assertIsInstance(due_time, datetime)
        self.assertGreater(due_time, datetime.now(pytz.UTC))

    async def test_calculate_daily_doggo_time(self):
        """
        Test the calculate_daily_doggo_time function.

        This method tests the calculate_daily_doggo_time function by checking if it
        correctly calculates the daily doggo time in UTC based on 10am Central Time
        of the next day, taking into consideration daylight saving time.
        """
        due_time = calculate_daily_doggo_time()
        self.assertIsInstance(due_time, datetime)
        self.assertGreater(due_time, datetime.now(pytz.UTC))

    async def test_calculate_daily_quote_time(self):
        """
        Test the calculate_daily_quote_time function.

        This method tests the calculate_daily_quote_time function by checking if it
        correctly calculates the daily doggo time in UTC based on 9am Central Time
        of the next day, taking into consideration daylight saving time.
        """
        due_time = calculate_daily_quote_time()
        self.assertIsInstance(due_time, datetime)
        self.assertGreater(due_time, datetime.now(pytz.UTC))


if __name__ == "__main__":
    unittest.main()
