"""
Tests for the DailyInspirationService cog.

This module contains unit tests for the DailyInspirationService cog, focusing on the
methods related to the daily quote task, including before_daily_quote_task.
"""
import unittest
from unittest.mock import AsyncMock, MagicMock, patch
from datetime import datetime, timedelta
import discord
import pytz

from src.cogs.daily_inspiration_service import DailyInspirationService


class TestDailyInspirationQuote(unittest.IsolatedAsyncioTestCase):
    """
    A test class for the DailyInspirationService cog.

    This class contains unit tests for the methods related to the daily quote task,
    including before_daily_quote_task.
    """

    @patch.dict(
        "src.cogs.daily_inspiration_service.os.environ",
        {
            "DEVELOPMENT_WEIGHT_LOSS_CHANNEL": "1234567890",
        },
    )
    async def asyncSetUp(self):
        """
        Set up the test environment for the TestDailyInspirationQuote class.

        This method is run before each test method and initializes the bot and
        DailyInspirationService instances for testing purposes.
        """
        # pylint: disable=W0201
        self.bot = MagicMock(spec=discord.Bot)
        self.daily_inspiration_service = DailyInspirationService(self.bot)

    @patch("src.cogs.daily_inspiration_service.asyncio")
    async def test_before_daily_quote_task(self, mock_asyncio):
        """
        Test the before_daily_quote_task method of the DailyInspirationService cog.

        This method tests the before_daily_quote_task method by simulating scenarios
        when the quote due time is in the future or has already passed, and checks if
        the correct behavior is implemented in each case.
        """
        # Test when quote due time is in the future
        self.daily_inspiration_service.quote_due_time = datetime.now(
            pytz.UTC
        ) + timedelta(minutes=5)
        mock_asyncio.sleep = AsyncMock()

        await self.daily_inspiration_service.before_daily_quote_task()
        unittest.TestCase().assertAlmostEqual(
            300, mock_asyncio.sleep.call_args[0][0], places=2
        )

        # Test when quote due time has already passed
        mock_asyncio.sleep.reset_mock()
        self.daily_inspiration_service.quote_due_time = datetime.now(
            pytz.UTC
        ) - timedelta(minutes=5)

        await self.daily_inspiration_service.before_daily_quote_task()
        mock_asyncio.sleep.assert_not_called()


if __name__ == "__main__":
    unittest.main()
