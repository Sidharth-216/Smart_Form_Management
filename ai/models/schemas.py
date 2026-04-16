from pydantic import BaseModel, Field


class ExtractTextRequest(BaseModel):
    file_url: str = Field(min_length=1, max_length=2048)

class ClassifyRequest(BaseModel):
    text: str = Field(min_length=1, max_length=10000)

class SuggestTagsRequest(BaseModel):
    text: str = Field(min_length=1, max_length=10000)

class ClassifyResponse(BaseModel):
    department: str
    category: str
    confidence: float

class SuggestTagsResponse(BaseModel):
    tags: list[str]
