from typing import List, Literal
from pydantic import BaseModel, Field


class Entity(BaseModel):
   text:str = Field()
   type: Literal["SOFT_SKILL","CAPABILITY","PERSONALITY_TRAIT","JOB_TITLE"]


class Labels(BaseModel):
   Entities: List[Entity]