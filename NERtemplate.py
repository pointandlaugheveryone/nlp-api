from enum import Enum
from typing import List, Literal
from pydantic import BaseModel


class EntityType(str, Enum):
   softSkill = "SOFTSKILL"
   domainSpecificSkill = "DOMAIN_SKILL"
   personality_trait = "PERSONALITY_TRAIT"
   jobtitle = "JOBTITLE"

class Label(BaseModel):
   start: int
   end: int
   score: float
   text: str
   label: EntityType

class NamedEntities(BaseModel):
   predictions: List[Label]


