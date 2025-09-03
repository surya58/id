from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import RedirectResponse
from models import UserDetailsParseRequest, UserDetailsParseResponse
from groq_service import get_parser

app = FastAPI(title="User Details API", version="v1", docs_url="/swagger", redoc_url="/redoc")
app.title = "User Details API"
app.version = "v1"
app.description = "API to parse user input into structured details using Groq"

# Configure CORS to allow all origins (same as before)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  
    allow_credentials=True,
    allow_methods=["*"],  
    allow_headers=["*"],  
)

# Send interactive user to Swagger UI by default
@app.get("/")
async def redirect_to_swagger():
    return RedirectResponse(url="/swagger")

# User details parsing endpoint
@app.post("/api/userdetails/parse", response_model=UserDetailsParseResponse)
async def parse_user_details(request: UserDetailsParseRequest):
    """
    Parses a single raw input string into structured details:
    Full name, first name, last name, and address components.
    Uses Groq API internally for LLM parsing.
    """
    try:
        parser = get_parser()
        result = await parser.parse_user_input(request.input)

        return UserDetailsParseResponse(
            full_name=result["full_name"],
            first_name=result["first_name"],
            last_name=result["last_name"],
            address_line=result["address_line"],
            city=result["city"],
            state=result["state"],
            zip=result["zip"]
        )
    except ValueError as e:
        raise HTTPException(status_code=500, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Parsing failed: {str(e)}")
