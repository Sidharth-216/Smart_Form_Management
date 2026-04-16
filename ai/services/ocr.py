from io import BytesIO

import httpx
import fitz

try:
    import pytesseract
    from PIL import Image
except Exception:  # pragma: no cover
    pytesseract = None
    Image = None


def extract_text_from_pdf_bytes(data: bytes) -> str:
    text_parts = []
    document = fitz.open(stream=data, filetype='pdf')
    for page in document:
        text_parts.append(page.get_text())
    text = '\n'.join(text_parts).strip()
    if text:
        return text

    if pytesseract is None or Image is None:
        return text

    for page in document:
        pixmap = page.get_pixmap(matrix=fitz.Matrix(2, 2), alpha=False)
        image = Image.open(BytesIO(pixmap.tobytes('png')))
        text_parts.append(pytesseract.image_to_string(image))
    return '\n'.join(text_parts).strip()


def extract_text_from_url(file_url: str) -> str:
    response = httpx.get(file_url, timeout=30)
    response.raise_for_status()
    content_type = response.headers.get('content-type', '').lower()
    if 'pdf' in content_type or file_url.lower().endswith('.pdf'):
        return extract_text_from_pdf_bytes(response.content)
    if pytesseract is None or Image is None:
        return response.text
    image = Image.open(BytesIO(response.content))
    return pytesseract.image_to_string(image).strip()
