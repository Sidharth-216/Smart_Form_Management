try:
    from sentence_transformers import SentenceTransformer, util
except Exception:  # pragma: no cover
    SentenceTransformer = None
    util = None

_MODEL = None


def _get_model():
    global _MODEL
    if _MODEL is None and SentenceTransformer is not None:
        _MODEL = SentenceTransformer('sentence-transformers/all-MiniLM-L6-v2')
    return _MODEL


def semantic_rank(query: str, candidates: list[str]) -> list[str]:
    model = _get_model()
    if model is None or util is None:
        return candidates
    query_embedding = model.encode(query, convert_to_tensor=True)
    candidate_embeddings = model.encode(candidates, convert_to_tensor=True)
    scores = util.cos_sim(query_embedding, candidate_embeddings)[0]
    ranked = [candidate for _, candidate in sorted(zip(scores.tolist(), candidates), reverse=True)]
    return ranked
