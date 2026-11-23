#!/usr/bin/env bash
set -euo pipefail

REPO_URL="https://github.com/Samedd6734/PCPartsBuild.git"
BRANCH="revizyon"
APP_DIR="/var/www/pcpartsbuild"
SRC_DIR="$APP_DIR/src"
BACKEND_DIR="$APP_DIR/backend"
FRONTEND_DIR="$APP_DIR/frontend"

if [ "$(id -u)" -ne 0 ]; then
    echo "Bu script root olarak calistirilmalidir."
    exit 1
fi

mkdir -p "$APP_DIR" "$BACKEND_DIR" "$FRONTEND_DIR"

if [ -d "$SRC_DIR/.git" ]; then
    git -C "$SRC_DIR" fetch origin "$BRANCH"
    git -C "$SRC_DIR" checkout "$BRANCH"
    git -C "$SRC_DIR" reset --hard "origin/$BRANCH"
else
    git clone --branch "$BRANCH" "$REPO_URL" "$SRC_DIR"
fi

echo "Backend publish ediliyor..."
dotnet publish "$SRC_DIR/PCPartsBuild-Backend/PCPartsAPI.csproj" -c Release -o "$BACKEND_DIR"

echo "Frontend dosyalari kopyalaniyor..."
rsync -a --delete \
    --exclude node_modules \
    --exclude .next \
    --exclude '.env*' \
    "$SRC_DIR/PCPartsBuild-Frontend/" "$FRONTEND_DIR/"

echo "Frontend build aliniyor..."
cd "$FRONTEND_DIR"
npm ci
npm run build

if [ ! -f "$BACKEND_DIR/appsettings.Production.json" ]; then
    echo "UYARI: $BACKEND_DIR/appsettings.Production.json bulunamadi."
    echo "Ornek dosyayi kopyalayip gercek degerleri girin:"
    echo "  cp $SRC_DIR/PCPartsBuild-Backend/appsettings.Production.example.json $BACKEND_DIR/appsettings.Production.json"
fi

chown -R www-data:www-data "$APP_DIR"

systemctl restart pcp-api
systemctl restart pcp-web
systemctl --no-pager status pcp-api pcp-web

echo "Deploy tamamlandi."
