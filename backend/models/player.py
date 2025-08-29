from pydantic import BaseModel, Field
from typing import Optional
from datetime import datetime
import uuid

class PlayerSkills(BaseModel):
    pace: int = Field(..., ge=1, le=99)
    shooting: int = Field(..., ge=1, le=99)
    passing: int = Field(..., ge=1, le=99)
    defending: int = Field(..., ge=1, le=99)
    dribbling: int = Field(..., ge=1, le=99)
    physical: int = Field(..., ge=1, le=99)

class Player(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    name: str = Field(..., min_length=1, max_length=100)
    position: str = Field(..., pattern="^(DEF|MID|ATT)$")
    points: int = Field(..., ge=1, le=99)
    photo: str = Field(..., max_length=500)
    skills: PlayerSkills
    age: int = Field(..., ge=16, le=50)
    preferredFoot: str = Field(..., pattern="^(Left|Right)$")
    nationality: str = Field(..., min_length=1, max_length=50)
    isSubscribed: bool = Field(default=False)  # New field for subscription status
    created_at: datetime = Field(default_factory=datetime.utcnow)
    updated_at: datetime = Field(default_factory=datetime.utcnow)

class PlayerCreate(BaseModel):
    name: str = Field(..., min_length=1, max_length=100)
    position: str = Field(..., pattern="^(DEF|MID|ATT)$")
    points: int = Field(..., ge=1, le=99)
    photo: str = Field(..., max_length=500)
    skills: PlayerSkills
    age: int = Field(..., ge=16, le=50)
    preferredFoot: str = Field(..., pattern="^(Left|Right)$")
    nationality: str = Field(..., min_length=1, max_length=50)
    isSubscribed: bool = Field(default=False)

class PlayerUpdate(BaseModel):
    name: Optional[str] = Field(None, min_length=1, max_length=100)
    position: Optional[str] = Field(None, pattern="^(DEF|MID|ATT)$")
    points: Optional[int] = Field(None, ge=1, le=99)
    photo: Optional[str] = Field(None, max_length=500)
    skills: Optional[PlayerSkills] = None
    age: Optional[int] = Field(None, ge=16, le=50)
    preferredFoot: Optional[str] = Field(None, pattern="^(Left|Right)$")
    nationality: Optional[str] = Field(None, min_length=1, max_length=50)
    isSubscribed: Optional[bool] = None
    updated_at: datetime = Field(default_factory=datetime.utcnow)