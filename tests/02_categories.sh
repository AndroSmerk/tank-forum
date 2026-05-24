#!/bin/bash
# ============================================
# ТЕСТ: Категории
# ОПИСАНИЕ: Проверка получения списка категорий
# ЭНДПОИНТ: GET /api/categories
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: 200 + список с "Общие", "Техника", "Кланы"
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

echo "============================================"
echo "ТЕСТ 02: Категории"
echo "============================================"

# --- Тест 2.1: Список всех категорий ---
echo ""
echo "=== Тест 2.1: GET /api/categories ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/categories")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Список категорий получен (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 2.2: Проверка наличия ожидаемых категорий ---
echo ""
echo "=== Тест 2.2: Проверка наличия категорий ==="
NAME_COUNT=$(echo "$BODY" | jq -r '.[].name // empty' 2>/dev/null | grep -cE '(Общие|Техника|Кланы)')

if [ "$NAME_COUNT" -ge 3 ]; then
  echo "✅ PASS: Все 3 категории присутствуют"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидались категории 'Общие', 'Техника', 'Кланы'"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 2.3: Категории без авторизации ---
echo ""
echo "=== Тест 2.3: Доступ без токена ==="
if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Категории доступны без авторизации (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Категории должны быть доступны без токена"
  FAILED=$((FAILED + 1))
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 02:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
