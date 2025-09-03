from pydantic import BaseModel, Field
from typing import Optional


class UserDetailsParseRequest(BaseModel):
    """
    Request model for parsing a single raw input string
    containing both name and address.
    """
    input: str = Field(..., description="Raw text containing user full name and address")


class UserDetailsParseResponse(BaseModel):
    """
    Response model for structured user details returned by Groq API.
    """
    full_name: str = Field(..., description="Full name of the user")
    first_name: str = Field(..., description="First name of the user")
    last_name: str = Field(..., description="Last name of the user")
    address_line: str = Field(..., description="Street address line, e.g., '123 Main St'")
    city: str = Field(..., description="City name")
    state: str = Field(..., description="State code or name")
    zip: str = Field(..., description="Postal/ZIP code")

    # Optional fields in case Groq can't perfectly parse
    confidence: Optional[float] = Field(default=None, description="Confidence score for parsing accuracy")
    notes: Optional[str] = Field(default=None, description="Any additional notes or warnings")
