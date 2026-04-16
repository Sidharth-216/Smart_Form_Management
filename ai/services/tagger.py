from collections import Counter
import re

STOPWORDS = {
    'the', 'and', 'for', 'form', 'application', 'certificate', 'government', 'department',
    'district', 'state', 'india', 'odisha', 'to', 'of', 'in', 'with', 'by', 'on', 'a', 'an'
}


def suggest_tags(text: str, limit: int = 12):
    words = re.findall(r"[a-zA-Z][a-zA-Z-]{2,}", text.lower())
    filtered = [word for word in words if word not in STOPWORDS]
    counts = Counter(filtered)
    tags = [word for word, _ in counts.most_common(limit)]
    return tags or ['government-form', 'odisha', 'application']
