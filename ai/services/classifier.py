DEPARTMENTS = {
    'revenue': 'Revenue',
    'transport': 'Transport',
    'health': 'Health',
    'education': 'Education',
    'labour': 'Labour',
    'municipal': 'Municipal Administration',
}

CATEGORIES = {
    'certificate': 'Certificate',
    'license': 'License',
    'permit': 'Permit',
    'application': 'Application',
    'registration': 'Registration',
}


def classify_form(text: str):
    lowered = text.lower()
    department = next((value for key, value in DEPARTMENTS.items() if key in lowered), 'General Administration')
    category = next((value for key, value in CATEGORIES.items() if key in lowered), 'Application')
    return {
        'department': department,
        'category': category,
        'confidence': 0.72,
    }
