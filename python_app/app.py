from __future__ import annotations

import base64
import io
import os
from dataclasses import dataclass
from typing import Dict, Tuple

from flask import Flask, jsonify, render_template, request
from PIL import Image, ImageStat
from werkzeug.utils import secure_filename


ALLOWED_EXTENSIONS = {".jpg", ".jpeg", ".png", ".gif"}
MAX_UPLOAD_SIZE = 5 * 1024 * 1024  # 5 MB


def create_app() -> Flask:
    app = Flask(__name__)
    app.config["MAX_CONTENT_LENGTH"] = MAX_UPLOAD_SIZE

    @app.route("/")
    def index() -> str:
        return render_template("index.html")

    @app.post("/analyze")
    def analyze() -> Tuple[str, int]:
        if "image" not in request.files:
            return jsonify({"error": "No se ha enviado ninguna imagen."}), 400

        file_storage = request.files["image"]
        filename = secure_filename(file_storage.filename)
        if filename == "":
            return jsonify({"error": "Debes seleccionar un archivo de imagen."}), 400

        extension = os.path.splitext(filename)[1].lower()
        if extension not in ALLOWED_EXTENSIONS:
            return (
                jsonify(
                    {
                        "error": "Formato no soportado. Usa JPG, PNG o GIF.",
                    }
                ),
                400,
            )

        try:
            image_bytes = file_storage.read()
            file_storage.stream.seek(0)
            image = Image.open(io.BytesIO(image_bytes))
            image.verify()
            file_storage.stream.seek(0)
            image = Image.open(file_storage.stream).convert("RGB")
        except Exception:  # pragma: no cover - Pillow raises various subclasses
            return jsonify({"error": "No se pudo procesar la imagen proporcionada."}), 400

        analysis = analyze_image(image)

        buffered = io.BytesIO()
        image.save(buffered, format="JPEG", quality=80)
        encoded_image = base64.b64encode(buffered.getvalue()).decode("ascii")

        response = {
            "category": analysis.category,
            "confidence": analysis.confidence,
            "recipe": analysis.recipe,
            "portions": analysis.portions,
            "image": f"data:image/jpeg;base64,{encoded_image}",
        }
        return jsonify(response)

    return app


@dataclass
class RecipeSuggestion:
    category: str
    confidence: int
    recipe: Dict[str, object]
    portions: Dict[str, object]


def analyze_image(image: Image.Image) -> RecipeSuggestion:
    """Generate a lightweight AI-inspired analysis for the uploaded food image."""
    resized = image.resize((128, 128))
    stat = ImageStat.Stat(resized)
    r, g, b = stat.mean
    brightness = sum(stat.mean) / 3

    dominant_color = max((r, "rojo"), (g, "verde"), (b, "azul"), key=lambda x: x[0])[1]

    if dominant_color == "verde" and brightness > 90:
        category = "Ensalada fresca"
        recipe = {
            "titulo": "Ensalada mediterránea",
            "ingredientes": [
                "2 tazas de hojas verdes mixtas",
                "1/2 taza de tomates cherry",
                "1/4 taza de pepino en cubos",
                "Queso feta desmenuzado",
                "Aceite de oliva, limón y orégano",
            ],
            "pasos": [
                "Mezcla las hojas verdes con el pepino y los tomates.",
                "Añade el queso feta y aliña con aceite de oliva, limón y orégano.",
            ],
        }
        portions = {
            "porcion_recomendada": "1 bol individual",
            "calorias_estimadas": 320,
        }
        confidence = 78
    elif dominant_color == "rojo" and brightness > 60:
        category = "Plato a base de tomate"
        recipe = {
            "titulo": "Pasta pomodoro rápida",
            "ingredientes": [
                "120 g de pasta larga",
                "1 taza de salsa de tomate natural",
                "1 diente de ajo",
                "Hojas de albahaca fresca",
                "Aceite de oliva y sal",
            ],
            "pasos": [
                "Cocina la pasta al dente.",
                "Saltea ajo en aceite, añade la salsa de tomate y cocina 5 minutos.",
                "Mezcla con la pasta y decora con albahaca.",
            ],
        }
        portions = {
            "porcion_recomendada": "1 plato (aprox. 2 tazas)",
            "calorias_estimadas": 540,
        }
        confidence = 74
    elif brightness < 70:
        category = "Guiso abundante"
        recipe = {
            "titulo": "Estofado rústico",
            "ingredientes": [
                "150 g de carne o proteína vegetal",
                "1 papa grande",
                "1 zanahoria",
                "1 taza de caldo",
                "Especias al gusto",
            ],
            "pasos": [
                "Dora la proteína en una olla.",
                "Agrega vegetales en cubos y el caldo.",
                "Cocina tapado 25 minutos hasta que espese.",
            ],
        }
        portions = {
            "porcion_recomendada": "1 plato hondo",
            "calorias_estimadas": 610,
        }
        confidence = 70
    else:
        category = "Postre o desayuno dulce"
        recipe = {
            "titulo": "Parfait de frutas y yogurt",
            "ingredientes": [
                "1 taza de yogurt natural",
                "1 taza de frutas frescas variadas",
                "1/4 taza de granola",
                "Miel al gusto",
            ],
            "pasos": [
                "En un vaso coloca capas de yogurt, frutas y granola.",
                "Repite hasta llenar y termina con un hilo de miel.",
            ],
        }
        portions = {
            "porcion_recomendada": "1 vaso (250 ml)",
            "calorias_estimadas": 380,
        }
        confidence = 65

    return RecipeSuggestion(
        category=category,
        confidence=confidence,
        recipe=recipe,
        portions=portions,
    )


if __name__ == "__main__":
    app = create_app()
    app.run(host="0.0.0.0", port=5000, debug=True)
