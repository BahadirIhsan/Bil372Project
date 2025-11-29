# meal_api.py
# =========================
# FastAPI ile RF meal öneri servisi
# =========================

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field
from typing import Literal, Dict, Any

import joblib
import pandas as pd
import os
import numpy as np


# ---------- 1) FastAPI app ----------
app = FastAPI(
    title="Fitol Meal Recommendation API",
    version="1.0.0",
    description="RF pipeline modelleriyle kahvaltı / öğle / akşam / atıştırmalık öneri servisi",
)


# ---------- 2) Model dosyalarını yükle ----------

# models klasörü: meal_api.py ile aynı dizinde "models" klasörü varsayıyoruz
MODELS_DIR = os.path.join(os.path.dirname(__file__), "models")

MEAL_MODEL_PATHS = {
    "breakfast": os.path.join(MODELS_DIR, "rf_breakfast.pkl"),
    "lunch": os.path.join(MODELS_DIR, "rf_lunch.pkl"),
    "dinner": os.path.join(MODELS_DIR, "rf_dinner.pkl"),
    "snack": os.path.join(MODELS_DIR, "rf_snack.pkl"),
}

models: Dict[str, Any] = {}

for meal, path in MEAL_MODEL_PATHS.items():
    if not os.path.exists(path):
        print(f"[WARN] Model dosyası bulunamadı: {path}")
    else:
        try:
            models[meal] = joblib.load(path)
            print(f"[INFO] {meal} modeli yüklendi: {path}")
        except Exception as e:
            print(f"[ERROR] {meal} modeli yüklenemedi ({path}): {e}")


# ---------- 3) İstek şeması (backend bize böyle JSON göndersin) ----------

class UserProfile(BaseModel):
    gender: Literal["Male", "Female"] = Field(..., description="Cinsiyet")
    activity_level: str = Field(..., description="Activity Level (Sedentary, Moderate, vb.)")
    dietary_preference: str = Field(..., description="Örn: Balanced, Vegetarian, Vegan")
    disease: str = Field(..., description="Örn: None, Diabetes, Hypertension")

    age: int = Field(..., ge=1, le=120)
    height: float = Field(..., description="Boy (cm)")
    weight: float = Field(..., description="Kilo (kg)")
    daily_calorie_target: float = Field(..., description="Günlük kalori hedefi (kcal)")

    protein: float = Field(..., description="Günlük protein hedefi (gram)")
    fat: float = Field(..., description="Günlük yağ hedefi (gram)")
    sugar: float = Field(..., description="Günlük şeker hedefi (gram)")
    sodium: float = Field(..., description="Günlük sodyum hedefi (mg)")


# ---------- 4) Modelin beklediği kolon isimlerine çeviren yardımcı fonksiyon ----------

# RF pipeline'ında kullandığımız orijinal feature kolonları:
MODEL_FEATURE_COLS = [
    "Gender",
    "Activity Level",
    "Dietary Preference",
    "Disease",
    "Ages",
    "Height",
    "Weight",
    "Daily Calorie Target",
    "Protein",
    "Fat",
    "Sugar",
    "Sodium",
]


def profile_to_dataframe(profile: UserProfile) -> pd.DataFrame:
    """
    API'den gelen UserProfile'ı,
    RF pipeline'ın beklediği kolon isimleriyle tek satırlık DataFrame'e çevirir.
    """
    data = {
        "Gender": [profile.gender],
        "Activity Level": [profile.activity_level],
        "Dietary Preference": [profile.dietary_preference],
        "Disease": [profile.disease],
        "Ages": [profile.age],
        "Height": [profile.height],
        "Weight": [profile.weight],
        "Daily Calorie Target": [profile.daily_calorie_target],
        "Protein": [profile.protein],
        "Fat": [profile.fat],
        "Sugar": [profile.sugar],
        "Sodium": [profile.sodium],
    }
    df = pd.DataFrame(data, columns=MODEL_FEATURE_COLS)
    return df


# ---------- 5) Root endpoint (test için) ----------

@app.get("/")
def root():
    return {
        "message": "Fitol Meal Recommendation API çalışıyor.",
        "available_meals": list(models.keys()),
    }


# ---------- 6) Tahmin endpoint'i ----------

@app.post("/predict/{meal_type}")
def predict_meal(
    meal_type: Literal["breakfast", "lunch", "dinner", "snack"],
    profile: UserProfile,
):
    """
    Belirtilen öğün için (breakfast/lunch/dinner/snack)
    kullanıcının profilinden yemek önerisi döndürür.
    """

    if meal_type not in models:
        raise HTTPException(status_code=404, detail=f"Model bulunamadı: {meal_type}")

    model = models[meal_type]

    # 1) Input'u DataFrame'e çevir
    X = profile_to_dataframe(profile)

    # 2) Tahmin ve sınıf bilgilerini al
    try:
        # Olasılık vektörü
        proba = model.predict_proba(X)[0]  # (n_classes,)

        # Sınıf etiketleri:
        # - Eğer model doğrudan classifier ise: model.classes_
        # - Pipeline ise: son adımın classes_ özelliğini kullan
        if hasattr(model, "classes_"):
            classes = model.classes_
        else:
            # Pipeline varsayımı: son step classifier
            last_step_name, last_estimator = list(model.named_steps.items())[-1]
            if not hasattr(last_estimator, "classes_"):
                raise RuntimeError(
                    f"Son adım ({last_step_name}) bir classifier değil veya classes_ yok."
                )
            classes = last_estimator.classes_

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Model tahmin hatası: {e}")

    # 3) NumPy → Python tiplerine çevir
    proba = np.array(proba, dtype=float)
    classes = np.array(classes, dtype=str)

    # 4) Top-1 ve Top-3 indeksleri
    sorted_idx = proba.argsort()[::-1]
    top1_idx = int(sorted_idx[0])
    top3_idx = sorted_idx[:3]

    top1 = {
        "label": classes[top1_idx],
        "prob": float(proba[top1_idx]),
    }

    top3 = [
        {
            "label": classes[int(i)],
            "prob": float(proba[int(i)]),
        }
        for i in top3_idx
    ]

    return {
        "meal_type": meal_type,
        "top1": top1,
        "top3": top3,
    }
