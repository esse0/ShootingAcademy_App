#!/bin/bash

set -e  # остановить при ошибке

echo "⚙️  Применяем секреты..."
kubectl apply -f k8s/base/secrets.yaml

echo "🚀 Развёртываем проект (dev overlay)..."
kubectl apply -k k8s/overlays/dev

echo "✅ Деплой завершён."