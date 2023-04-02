"""
Tests for the Buff Reminder Service module.

This test module contains test cases for the BuffReminderService cog and related
functions, such as calculate_buff_due_time and get_buff_channel_id.
"""
import unittest
from unittest.mock import AsyncMock, MagicMock, patch
from datetime import datetime, timedelta
import discord
import pytz

from src.cogs.buff_reminder_service import (
    BuffReminderService,
    calculate_buff_due_time,
    get_buff_channel_id,
)


class TestBuffReminderService(unittest.IsolatedAsyncioTestCase):
    """
    Test cases for the BuffReminderService cog.
    """

    @patch.dict(
        "src.cogs.buff_reminder_service.os.environ",
        {
            "is_production": "false",
            "DEVELOPMENT_BUFF_CHANNEL": "1234",
        },
    )
    async def asyncSetUp(self):
        """
        Set up the test environment by initializing the bot and the BuffReminderService instance.
        """
        # pylint: disable=W0201
        self.bot = MagicMock(spec=discord.Bot)
        self.buff_reminder_service = BuffReminderService(self.bot)

    async def test_on_message(self):
        """
        Test the on_message method of the BuffReminderService cog.

        This method tests the on_message method by checking if the response is sent
        correctly based on the message content and the cooldown period status.
        """
        # Test with valid message and cooldown period passed
        message = MagicMock(spec=discord.Message)
        message.content = "Hello there! :BuffCat:"
        message.channel.send = AsyncMock()
        self.buff_reminder_service.cooldown = datetime.utcnow() - timedelta(hours=1)
        await self.buff_reminder_service.on_message(message)
        message.channel.send.assert_called_once_with("Praise be to the Buff Cat!")

        # Test with valid message and cooldown period not passed
        message.channel.send.reset_mock()
        self.buff_reminder_service.cooldown = datetime.utcnow() + timedelta(hours=1)
        await self.buff_reminder_service.on_message(message)
        message.channel.send.assert_not_called()

        # Test with invalid message
        message.content = "No BuffCat here!"
        await self.buff_reminder_service.on_message(message)
        message.channel.send.assert_not_called()

    async def test_buff_reminder_task(self):
        """
        Test the buff_reminder_task method of the BuffReminderService cog.

        This method tests the buff_reminder_task by ensuring the task sends the reminder
        message correctly and handles different channel types.
        """
        # Test with valid TextChannel
        text_channel = MagicMock(spec=discord.TextChannel)
        text_channel.send = AsyncMock()
        self.bot.get_channel.return_value = text_channel
        # pylint: disable=W0212
        self.buff_reminder_service._buff_channel_id = 12345
        self.buff_reminder_service.buff_due_time = datetime.utcnow() - timedelta(
            minutes=1
        )

        await self.buff_reminder_service.buff_reminder_task()
        text_channel.send.assert_called_once_with(
            "@here, I have not seen the Buff Cat lately.  "
            "Please honor me with its presence if the buffs have been updated."
        )

        # Test with invalid channel type
        invalid_channel = MagicMock(spec=discord.VoiceChannel)
        self.bot.get_channel.return_value = invalid_channel

        with self.assertRaises(ValueError):
            await self.buff_reminder_service.buff_reminder_task()

    @patch("src.cogs.buff_reminder_service.asyncio")
    async def test_before_buff_reminder_task(self, mock_asyncio):
        """
        Test the before_buff_reminder_task method of the BuffReminderService cog.

        This method tests the before_buff_reminder_task by checking if it correctly
        waits for the remaining time until the buff due time before starting the
        buff reminder task loop.
        """
        # Test when buff due time is in the future
        self.buff_reminder_service.buff_due_time = datetime.now(pytz.UTC) + timedelta(
            minutes=5
        )
        mock_asyncio.sleep = AsyncMock()

        await self.buff_reminder_service.before_buff_reminder_task()
        unittest.TestCase().assertAlmostEqual(
            300, mock_asyncio.sleep.call_args[0][0], places=2
        )

        # Test when buff due time has already passed
        mock_asyncio.sleep.reset_mock()
        self.buff_reminder_service.buff_due_time = datetime.now(pytz.UTC) - timedelta(
            minutes=5
        )

        await self.buff_reminder_service.before_buff_reminder_task()
        mock_asyncio.sleep.assert_not_called()

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


class TestGetBuffChannelId(unittest.TestCase):
    """
    Test cases for the get_buff_channel_id function.
    """

    @patch(
        "src.cogs.buff_reminder_service.os.environ",
        {"is_production": "true", "PRODUCTION_BUFF_CHANNEL": "12345"},
    )
    def test_get_buff_channel_id_production(self):
        """
        Test the get_buff_channel_id function in production mode.

        This method tests the get_buff_channel_id function by checking if it
        correctly retrieves the Buff Channel ID from environment variables when
        the bot is running in production mode.
        """
        channel_id = get_buff_channel_id()
        self.assertEqual(channel_id, 12345)

    @patch(
        "src.cogs.buff_reminder_service.os.environ",
        {"is_production": "false", "DEVELOPMENT_BUFF_CHANNEL": "67890"},
    )
    def test_get_buff_channel_id_development(self):
        """
        Test the get_buff_channel_id function in development mode.

        This method tests the get_buff_channel_id function by checking if it
        correctly retrieves the Buff Channel ID from environment variables when
        the bot is running in development mode.
        """
        channel_id = get_buff_channel_id()
        self.assertEqual(channel_id, 67890)

    @patch("src.cogs.buff_reminder_service.os.environ", {"is_production": "true"})
    def test_get_buff_channel_id_missing(self):
        """
        Test the get_buff_channel_id function when the Buff Channel ID is missing.

        This method tests the get_buff_channel_id function by checking if it raises
        a ValueError when the Buff Channel ID is not found in environment variables.
        """
        with self.assertRaises(ValueError):
            get_buff_channel_id()


if __name__ == "__main__":
    unittest.main()
