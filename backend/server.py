from fastapi import FastAPI, APIRouter, Depends
from dotenv import load_dotenv
from starlette.middleware.cors import CORSMiddleware
from motor.motor_asyncio import AsyncIOMotorClient, AsyncIOMotorDatabase
import os
import logging
from pathlib import Path

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

# MongoDB connection
mongo_url = os.environ['MONGO_URL']
client = AsyncIOMotorClient(mongo_url)
db = client[os.environ['DB_NAME']]

# Create the main app without a prefix
app = FastAPI(title="Football Team Shuffler API", version="1.0.0")

# Create a router with the /api prefix for basic routes
api_router = APIRouter(prefix="/api")

# Database dependency
async def get_database() -> AsyncIOMotorDatabase:
    return db

# Basic health check route
@api_router.get("/")
async def root():
    return {"message": "Football Team Shuffler API is running!"}

# Health check for database
@api_router.get("/health")
async def health_check(database: AsyncIOMotorDatabase = Depends(get_database)):
    try:
        # Test database connection
        await database.players.find_one({})
        return {"status": "healthy", "database": "connected"}
    except Exception as e:
        return {"status": "unhealthy", "database": "disconnected", "error": str(e)}

# Import and include routers after app creation
from routes import players, shuffle

# Include routers
app.include_router(api_router)
app.include_router(players.router)
app.include_router(shuffle.router)

app.add_middleware(
    CORSMiddleware,
    allow_credentials=True,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

@app.on_event("shutdown")
async def shutdown_db_client():
    client.close()