#!/bin/bash
# ============================================
# ТЕСТ: Swagger
# ОПИСАНИЕ: Проверка доступности Swagger UI и OpenAPI spec
# ЭНДПОИНТЫ: GET /swagger/index.html, /swagger/v1/swagger.json
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: 200 + валидный JSON
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

echo "============================================"
echo "ТЕСТ 10: Swagger"
echo "============================================"

# --- Тест 10.1: Swagger UI ---
echo ""
echo "=== Тест 10.1: GET /swagger/index.html ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/swagger/index.html")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Swagger UI доступен (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Тест 10.2: Swagger JSON ---
echo ""
echo "=== Тест 10.2: GET /swagger/v1/swagger.json ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/swagger/v1/swagger.json")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  # Проверяем, что это валидный JSON
  echo "$BODY" | jq empty 2>/dev/null
  if [ $? -eq 0 ]; then
    echo "✅ PASS: swagger.json доступен и валиден (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: swagger.json не является валидным JSON"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Итог ---
echo ""
echo "==========================================="
echo "ИТОГО ТЕСТ 10:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
