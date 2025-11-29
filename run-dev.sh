#!/bin/zsh


# ÇALIŞTIRMAK İÇİN KOMUTLAR :
# chmod +x run-dev.sh
# ./run-dev.sh


# Script'in bulunduğu klasöre git
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

echo "[RUN] fitol-ml-api ve .NET web uygulaması birlikte başlatılıyor..."

# 1) Python AI servisini arka planda başlat
echo "[RUN] Python AI servisi başlatılıyor..."
cd "$SCRIPT_DIR/fitol-ml-api"

# venv aktif et
if [ -d "venv" ]; then
  source venv/bin/activate
else
  echo "[ERROR] venv klasörü bulunamadı. Önce 'python3 -m venv venv' ile oluştur."
  exit 1
fi

# uvicorn'u arka planda çalıştır
uvicorn meal_api:app --host 0.0.0.0 --port 8000 &
PYTHON_PID=$!
echo "[OK] Python AI servisi PID: $PYTHON_PID"

# 2) .NET web uygulamasını başlat
echo "[RUN] .NET web uygulaması başlatılıyor..."
cd "$SCRIPT_DIR/Bil372Project.PresentationLayer"
dotnet run

# 3) .NET uygulaması durduğunda Python'u da sonlandır
echo "[STOP] .NET durdu, Python AI servisi kapatılıyor..."
kill $PYTHON_PID || echo "[WARN] Python süreci zaten bitmiş olabilir."

echo "[DONE] run-dev.sh tamamlandı."

