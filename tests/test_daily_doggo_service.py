"""
Tests for the DailyDoggoService cog.

This module contains unit tests for the DailyDoggoService cog, focusing on the
methods related to the daily doggo task, including before_daily_doggo_task.
"""

import unittest
from unittest.mock import AsyncMock, MagicMock, patch
from datetime import datetime, timedelta
import discord
import pytz

from src.cogs.daily_doggo_service import DailyDoggoService


class TestDailyDoggo(unittest.IsolatedAsyncioTestCase):
    """
    A test class for the DailyDoggoService cog.

    This class contains unit tests for the methods related to the daily doggo task,
    including before_daily_doggo_task.
    """

    @patch.dict(
        "src.cogs.daily_doggo_service.os.environ",
        {
            "DEVELOPMENT_GENERAL_CHANNEL": "1234567890",
        },
    )
    async def asyncSetUp(self):
        """
        Set up the test environment for the TestDailyDoggo class.

        This method is run before each test method and initializes the bot and
        DailyDoggoService instances for testing purposes.
        """
        # pylint: disable=W0201
        self.bot = MagicMock(spec=discord.Bot)
        self.daily_doggo_service = DailyDoggoService(self.bot)

    @patch("src.cogs.daily_doggo_service.asyncio")
    async def test_before_daily_doggo_task(self, mock_asyncio):
        """
        Test the before_daily_doggo_task method of the DailyDoggoService cog.

        This method tests the before_daily_doggo_task method by simulating scenarios
        when the doggo due time is in the future or has already passed, and checks if
        the correct behavior is implemented in each case.
        """
        # Test when buff due time is in the future
        self.daily_doggo_service.doggo_due_time = datetime.now(pytz.UTC) + timedelta(
            minutes=5
        )
        mock_asyncio.sleep = AsyncMock()

        await self.daily_doggo_service.before_daily_doggo_task()
        unittest.TestCase().assertAlmostEqual(
            300, mock_asyncio.sleep.call_args[0][0], places=2
        )

        # Test when buff due time has already passed
        mock_asyncio.sleep.reset_mock()
        self.daily_doggo_service.doggo_due_time = datetime.now(pytz.UTC) - timedelta(
            minutes=5
        )

        await self.daily_doggo_service.before_daily_doggo_task()
        mock_asyncio.sleep.assert_not_called()


if __name__ == "__main__":
    unittest.main()
