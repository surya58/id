import os
import httpx
import json
import logging
from pydantic_settings import BaseSettings

# Set up logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class GroqSettings(BaseSettings):
    groq_api_key: str = ""
    groq_model: str = "llama-3.1-8b-instant"  # Default model

    class Config:
        env_file = ".env"


settings = GroqSettings()


class UserDetailsParser:
    def __init__(self):
        if not settings.groq_api_key:
            raise ValueError("GROQ_API_KEY environment variable is not set")
        
        self.api_key = settings.groq_api_key
        self.model = settings.groq_model
        self.api_url = "https://api.groq.com/openai/v1/chat/completions"

    async def parse_user_input(self, raw_input: str) -> dict:
        """
        Calls the Groq API to parse user input into structured user details.
        """
        # Prompt instructing the LLM to return JSON only
        system_prompt = """
        You are a highly accurate information extractor. 
        Extract the following fields from the provided input and return a **valid JSON object only**:

        - full_name
        - first_name
        - last_name
        - address_line
        - city
        - state
        - zip

        Example output:
        {
            "full_name": "John Smith",
            "first_name": "John",
            "last_name": "Smith",
            "address_line": "123 Main St",
            "city": "Dallas",
            "state": "TX",
            "zip": "75201"
        }

        Make sure the response is valid JSON with no additional text.
        """

        payload = {
            "model": self.model,
            "messages": [
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": f"Input: {raw_input}"}
            ],
            "temperature": 0.0
        }

        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }

        try:
            logger.info(f"Making Groq API call with input: {raw_input[:50]}...")
            logger.info(f"Using API URL: {self.api_url}")
            logger.info(f"Using model: {self.model}")
            
            async with httpx.AsyncClient() as client:
                response = await client.post(self.api_url, headers=headers, json=payload)
                response.raise_for_status()
                data = response.json()
                
            logger.info(f"Groq API response received: {len(str(data))} characters")

            # Extract model response content
            content = data["choices"][0]["message"]["content"].strip()
            logger.info(f"Groq API content: {content}")

            # Parse the JSON
            try:
                result = json.loads(content)
                logger.info(f"Successfully parsed JSON response: {result}")
            except json.JSONDecodeError:
                logger.error(f"Invalid JSON returned by Groq: {content}")
                raise ValueError(f"Invalid JSON returned by Groq: {content}")

            return result

        except httpx.HTTPStatusError as e:
            logger.error(f"Groq API HTTP error: {e.response.status_code} - {e.response.text}")
            raise ValueError(f"Groq API error: {e.response.status_code} - {e.response.text}")
        except Exception as e:
            logger.error(f"Unexpected error calling Groq API: {str(e)}")
            raise ValueError(f"Unexpected error calling Groq API: {str(e)}")


# Singleton instance
parser_instance = None

def get_parser() -> UserDetailsParser:
    global parser_instance
    if parser_instance is None:
        parser_instance = UserDetailsParser()
    return parser_instance
