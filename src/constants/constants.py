from datetime import time
import os

DAILY_INSPIRATION_TIME = time(14)
DAILY_BUFF_TIME = time(5)
WEATHER_API_BASE_URL = "http://api.weatherapi.com/v1/"
WEATHER_API_KEY = api_key = os.environ.get("WEATHER_API_KEY")
