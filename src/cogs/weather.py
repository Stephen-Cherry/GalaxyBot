"""File holding the Cog for weather commands."""
from typing import List
import discord
from discord import Option
from discord.ext import commands
import aiohttp

from src.constants.constants import WEATHER_API_BASE_URL, WEATHER_API_KEY
from src.models.weather import Weather


class WeatherCog(commands.Cog):
    """Weather Commands Cog."""

    def __init__(self, bot: discord.Bot) -> None:
        self.bot = bot

    async def fetch_weather_data(self, location: str) -> Weather:
        """Fetches weather data from the API."""
        url = (
            f"{WEATHER_API_BASE_URL}forecast.json?key={WEATHER_API_KEY}"
            f"&q={location}&days=3&aqi=no&alerts=no"
        )
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as response:
                return await response.json()

    weather = discord.SlashCommandGroup("weather", "Commands for weather.")

    @weather.command(
        name="current",
        description="Get the current weather",
    )
    async def current(
        self,
        ctx: discord.ApplicationContext,
        location: Option(
            str,
            ("City with state/province and country or zip/postal code."),
            required=True,
        ),
        hide: bool = False,
    ):
        """Get the current weather."""
        data: Weather = await self.fetch_weather_data(location)
        weather_now = data["current"]
        embed = discord.Embed(
            title=(
                f"Current weather in {data['location']['name']}, "
                f"{data['location']['region']}, "
                f"{data['location']['country']}"
            ),
            description=weather_now["condition"]["text"],
        )
        embed.set_thumbnail(url=f"https:{weather_now['condition']['icon']}")
        embed.add_field(name="Humidity", value=f"{weather_now['humidity']}%")
        embed.add_field(
            name="Temperature",
            value=f"{weather_now['temp_f']}°F ({weather_now['temp_c']}°C)",
        )
        embed.add_field(
            name="Feels Like",
            value=f"{weather_now['feelslike_f']}°F ({weather_now['feelslike_c']}°C)",
        )
        embed.add_field(
            name="Wind",
            value=f"{weather_now['wind_mph']} mph ({weather_now['wind_kph']} kph)",
        )
        embed.add_field(
            name="Gust",
            value=f"{weather_now['gust_mph']} mph ({weather_now['gust_kph']} kph)",
        )
        embed.add_field(
            name="Visibility",
            value=f"{weather_now['vis_miles']} miles ({weather_now['vis_km']} km)",
        )

        await ctx.send_response(embed=embed, ephemeral=hide)

    @weather.command(
        name="three_day",
        description="Get the weather forecast for the next 3 days",
    )
    async def three_day(
        self,
        ctx: discord.ApplicationContext,
        location: Option(
            str,
            ("City with state/province and country or zip/postal code."),
            required=True,
        ),
        hide: bool = False,
    ):
        """Get the weather forecast for the next 3 days."""
        data: Weather = await self.fetch_weather_data(location)
        embeds: List[discord.Embed] = []

        for forecast_day in data["forecast"]["forecastday"]:
            condition = forecast_day["day"]["condition"]["text"]
            high_temp_c = forecast_day["day"]["maxtemp_c"]
            low_temp_c = forecast_day["day"]["mintemp_c"]
            high_temp_f = forecast_day["day"]["maxtemp_f"]
            low_temp_f = forecast_day["day"]["mintemp_f"]

            embed = discord.Embed(
                title=(
                    f"{forecast_day['date']} forecast in {data['location']['name']}, "
                    f"{data['location']['region']}, "
                    f"{data['location']['country']}"
                ),
                description=f"Condition: {condition}",
            )
            embed.set_thumbnail(
                url=f"https:{forecast_day['day']['condition']['icon']}"
            )
            embed.add_field(
                name="High Temperature",
                value=f"{high_temp_f}°F ({high_temp_c}°C)",
            )
            embed.add_field(
                name="Low Temperature",
                value=f"{low_temp_f}°F ({low_temp_c}°C)",
            )

            embeds.append(embed)
        await ctx.send_response(embeds=embeds, ephemeral=hide)


def setup(bot: discord.Bot) -> None:
    """Adds cog to the bot."""
    bot.add_cog(WeatherCog(bot))
