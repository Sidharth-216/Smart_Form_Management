import os

from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from models.schemas import ClassifyRequest, ExtractTextRequest, SuggestTagsRequest
from services.classifier import classify_form
from services.ocr import extract_text_from_url
from services.tagger import suggest_tags

app = FastAPI(title='Smart Government Form AI', version='1.0.0')

allowed_origins = [origin.strip() for origin in os.getenv('AI_CORS_ORIGINS', '*').split(',') if origin.strip()]

app.add_middleware(
    CORSMiddleware,
    allow_origins=['*'] if allowed_origins == ['*'] else allowed_origins,
    allow_credentials=True,
    allow_methods=['*'],
    allow_headers=['*'],
)

@app.get('/health')
async def health():
    return {'status': 'healthy'}

@app.post('/extract-text')
async def extract_text(payload: ExtractTextRequest):
    try:
        text = extract_text_from_url(payload.file_url)
        return {'text': text}
    except Exception as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc

@app.post('/classify')
async def classify(payload: ClassifyRequest):
    try:
        return classify_form(payload.text)
    except Exception as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc

@app.post('/suggest-tags')
async def suggest(payload: SuggestTagsRequest):
    try:
        return {'tags': suggest_tags(payload.text)}
    except Exception as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
